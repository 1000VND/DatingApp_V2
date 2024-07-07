
# tạo một lớp build-env từ image sdk của microsoft
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env 
# Thiết lập thư mục làm việc trong container là /app.
WORKDIR /app
EXPOSE 8080

# sao chép các file csproj vào thư mục /app
COPY *.csproj ./
#restore các gói nuget
RUN dotnet restore

# sao chép toàn bộ nội dung từ thư mục local vào thư mục làm việc trong container. Điều này bao gồm cả mã nguồn của ứng dụng
COPY . ./
#build ứng dụng trong chế độ Release và đặt output vào thư mục out
RUN dotnet publish -c Release -o out

# Đây là stage thứ hai, sử dụng image mcr.microsoft.com/dotnet/aspnet:7.0 làm base image để tạo ra runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:7.0
#Thiết lập thư mục làm việc trong container runtime là /app.
WORKDIR /app
# sao chép nội dung từ stage build-env (từ thư mục /app/out trong stage trước) vào thư mục làm việc trong container runtime (/app).
COPY --from=build-env /app/out .
# chạy ứng dụng
ENTRYPOINT [ "dotnet", "API.dll" ]