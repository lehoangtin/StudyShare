# Sử dụng Python bản nhẹ
FROM python:3.11-slim

# Thiết lập thư mục làm việc
WORKDIR /app

# Copy requirements và cài đặt
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy toàn bộ mã nguồn
COPY . .

# Mở cổng 8000
EXPOSE 8000

# Lệnh chạy ứng dụng
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]