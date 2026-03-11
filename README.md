# HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY ỨNG DỤNG
## Hệ Thống Quản Lý Kho Hàng (OOP Demo)

Tài liệu này hướng dẫn cách chuẩn bị môi trường, biên dịch và khởi chạy ứng dụng một cách nhanh nhất.

---

### 1. Yêu cầu hệ thống (Prerequisites)
Để chạy được ứng dụng này, máy tính của bạn cần cài đặt:
*   **Hệ điều hành**: Windows 10/11 (Do ứng dụng sử dụng Windows Forms).
*   **Bộ công cụ**: [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (Đây là yêu cầu bắt buộc để thực hiện lệnh `dotnet`).

### 2. Hướng dẫn Biên dịch (Build)
Mở terminal (PowerShell hoặc Command Prompt) tại thư mục gốc của dự án và chạy lệnh sau:

```bash
dotnet build QuanLyKho/QuanLyKho.csproj
```

*Lệnh này sẽ kiểm tra lỗi cú pháp và chuẩn bị các thư viện cần thiết.*

### 3. Hướng dẫn Chạy ứng dụng (Run)
Sau khi build thành công, hãy chạy lệnh sau để mở giao diện ứng dụng:

```bash
dotnet run --project QuanLyKho/QuanLyKho.csproj
```

---

### 4. Cấu trúc dữ liệu
*   Dữ liệu được lưu trữ tự động dưới dạng tệp tin `.json` trong thư mục `Data/`.
*   Ứng dụng sẽ tự động khởi tạo dữ liệu mẫu nếu đây là lần đầu tiên bạn chạy chương trình.

### 5. Lưu ý quan trọng
*   Nếu gặp lỗi liên quan đến đường dẫn, hãy đảm bảo bạn đang đứng đúng tại thư mục: `Object-oriented application development`.
*   Nếu không có màn hình hiển thị, vui lòng kiểm tra lại xem .NET 8 SDK đã được cài đặt thành công hay chưa bằng lệnh: `dotnet --version`.

---
*Chúc bạn có trải nghiệm tốt nhất với ứng dụng!*
