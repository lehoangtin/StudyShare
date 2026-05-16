import os
import json
from typing import Dict, Optional
from fastapi import FastAPI, HTTPException, Security, Depends
from fastapi.security.api_key import APIKeyHeader
from starlette import status
from pydantic import BaseModel, Field
from dotenv import load_dotenv
from google import genai
from google.genai import types
from cachetools import TTLCache # Cần cài thêm: pip install cachetools

# Tải biến môi trường
load_dotenv()
api_key_gemini = os.getenv("GEMINI_API_KEY")
internal_api_key = os.getenv("PBL3_INTERNAL_API_KEY")

# Cảnh báo cấu hình nghiêm trọng nếu thiếu API Key nội bộ
if not internal_api_key:
    raise ValueError("LỖI NGHIÊM TRỌNG: Thiếu PBL3_INTERNAL_API_KEY trong file .env")

# Cấu hình bảo mật API Key cho FastAPI
API_KEY_NAME = "X-PBL3-API-KEY"
api_key_header = APIKeyHeader(name=API_KEY_NAME, auto_error=False)

async def get_api_key(header_value: Optional[str] = Security(api_key_header)):
    """Hàm kiểm tra mã bí mật giữa C# (Backend) và Python (Microservice)"""
    print(f"Key trong file .env là : '{internal_api_key}'")
    print(f"Key nhập trên web là   : '{header_value}'")
    if header_value and header_value == internal_api_key:
        return header_value
    raise HTTPException(
        status_code=status.HTTP_403_FORBIDDEN,
        detail="Quyền truy cập bị từ chối. API Key không hợp lệ hoặc bị thiếu."
    )

# Khởi tạo Client Gemini
client = None
if api_key_gemini:
    client = genai.Client(api_key=api_key_gemini)
else:
    print("CẢNH BÁO: Chưa tìm thấy GEMINI_API_KEY! Các endpoint AI sẽ trả về lỗi.")

app = FastAPI(
    title="Secure AI Microservice for PBL3",
    swagger_ui_parameters={"persistAuthorization": True}
)

# --- MODELS ---
class TextRequest(BaseModel):
    text: str

class ModerationResult(BaseModel):
    isFlagged: bool = Field(description="True nếu văn bản vi phạm tiêu chuẩn cộng đồng.")
    reason: str = Field(description="Lý do cụ thể.")

class ChatRequest(BaseModel):
    session_id: str  
    message: str

# --- CHAT SESSIONS ---
# Tối ưu: Sử dụng TTLCache để tự động dọn dẹp bộ nhớ sau 2 giờ không hoạt động
# Tránh Memory Leak nếu user tắt trình duyệt mà không gọi API DELETE
chat_sessions = TTLCache(maxsize=1000, ttl=7200) 

system_instruction = """
Bạn là trợ lý AI tên 'Studybot học tập' của hệ thống PBL3. 
Hãy giải đáp thắc mắc về tài liệu, điểm số và diễn đàn một cách ngắn gọn.
"""

# ==========================================
# ENDPOINT: KIỂM DUYỆT 
# ==========================================
@app.post("/api/moderate", response_model=ModerationResult)
async def check_content(request: TextRequest, authenticated: str = Depends(get_api_key)):
    if not client:
        raise HTTPException(status_code=503, detail="Thiếu cấu hình Gemini API Key.")

    moderation_prompt = f"Kiểm duyệt nội dung sau: \"{request.text}\""

    try:
        # Tối ưu: Dùng client.aio để không chặn event loop của FastAPI
        response = await client.aio.models.generate_content(
            model='gemini-2.5-flash',
            contents=moderation_prompt,
            config=types.GenerateContentConfig(
                response_mime_type="application/json",
                response_schema=ModerationResult,
                temperature=0.0 
            )
        )
        return json.loads(response.text)
    except Exception as e:
        print(f"Lỗi Kiểm duyệt: {e}")
        # Tối ưu: Trả về HTTP 500 thay vì 200 OK để server C# biết xử lý fallback
        raise HTTPException(status_code=500, detail="Lỗi hệ thống AI khi kiểm duyệt.")

# ==========================================
# ENDPOINT: CHATBOT 
# ==========================================
@app.post("/api/chat")
async def chat_bot(request: ChatRequest, authenticated: str = Depends(get_api_key)):
    if not client:
        raise HTTPException(status_code=503, detail="Thiếu cấu hình Gemini API Key.")

    try:
        if request.session_id not in chat_sessions:
            # Tối ưu: Khởi tạo chat session bất đồng bộ
            chat_sessions[request.session_id] = client.aio.chats.create(
                model='gemini-2.5-flash',
                config=types.GenerateContentConfig(
                    system_instruction=system_instruction,
                    temperature=0.7 
                )
            )
        
        current_chat = chat_sessions[request.session_id]
        # Tối ưu: Dùng await để gửi tin nhắn
        response = await current_chat.send_message(request.message)
        
        # Cập nhật lại cache để reset thời gian TTL
        chat_sessions[request.session_id] = current_chat 
        
        return {"reply": response.text}
        
    except Exception as e:
        print(f"Lỗi Chatbot: {e}")
        raise HTTPException(status_code=500, detail="Dịch vụ AI đang bận hoặc gặp lỗi.")

# Endpoint dọn dẹp bộ nhớ chủ động
@app.delete("/api/chat/{session_id}")
async def clear_chat(session_id: str, authenticated: str = Depends(get_api_key)):
    if session_id in chat_sessions:
        del chat_sessions[session_id]
        return {"message": "Đã xóa lịch sử trò chuyện thành công"}
    return {"message": "Không tìm thấy session"}

if __name__ == "__main__":
    import uvicorn
    # Gợi ý: Khi chạy thực tế, không dùng reload=True
    uvicorn.run("main:app", host="127.0.0.1", port=8000, reload=True)