# Sử dụng Python
FROM python:3.10-slim

WORKDIR /app

# Copy các file của Python vào container
COPY . .

# Cài đặt các thư viện theo README
RUN pip install --no-cache-dir google-genai uvicorn python-dotenv fastapi

# Mở port 8000
EXPOSE 8000

# Chạy lệnh uvicorn (thêm --host 0.0.0.0 để cho phép truy cập từ ngoài container)
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000", "--reload"]