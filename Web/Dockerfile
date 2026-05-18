# Sử dụng SDK của .NET (Bạn hãy đổi phiên bản 10.0 thành phiên bản .NET bạn đang dùng, vd 8.0 hoặc 9.0)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy file .csproj và restore các thư viện
COPY ["PBL3demo.csproj", "./"]
RUN dotnet restore "PBL3demo.csproj"

# Copy toàn bộ code và build
COPY . .
RUN dotnet publish "PBL3demo.csproj" -c Release -o /app/publish

# Môi trường chạy (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PBL3demo.dll"]