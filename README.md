# eTestHub - Nền tảng thi trắc nghiệm online

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue.svg)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3.0-purple.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## 📖 Giới thiệu

**eTestHub** là nền tảng thi trắc nghiệm online hiện đại được xây dựng trên ASP.NET Core 8.0, cung cấp giải pháp toàn diện cho việc tổ chức và quản lý các kỳ thi trực tuyến.

### ✨ Tính năng chính

- 🎯 **Giao diện hiện đại**: Thiết kế responsive với Bootstrap 5.3
- 🔐 **Xác thực người dùng**: Hệ thống đăng nhập/đăng ký an toàn
- 📝 **Quản lý đề thi**: Tạo và quản lý câu hỏi trắc nghiệm
- 📊 **Dashboard trực quan**: Theo dõi tiến độ và kết quả
- 🎨 **UI/UX tối ưu**: Thiết kế dựa trên Figma specifications
- 📱 **Responsive Design**: Tương thích đa thiết bị

## 🚀 Cài đặt và chạy

### Yêu cầu hệ thống

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) hoặc cao hơn
- Visual Studio 2022 / VS Code (khuyến nghị)
- Git

### Cài đặt

1. **Clone repository:**

   ```bash
   git clone https://github.com/TienxDun/E_TestHub.git
   cd E_TestHub
   ```

2. **Restore packages:**

   ```bash
   dotnet restore
   ```

3. **Build project:**

   ```bash
   dotnet build
   ```

4. **Chạy ứng dụng:**

   ```bash
   cd E_TestHub
   dotnet run
   ```

5. **Truy cập ứng dụng:**
   - Mở trình duyệt và truy cập: `http://localhost:5230`

## 📁 Cấu trúc dự án

```text
E_TestHub/
├── Controllers/           # MVC Controllers
│   └── HomeController.cs  # Controller chính
├── Models/               # Data Models
│   └── ErrorViewModel.cs # Error handling
├── Views/                # Razor Views
│   ├── Home/
│   │   ├── Index.cshtml  # Trang chủ
│   │   ├── Login.cshtml  # Trang đăng nhập
│   │   └── Register.cshtml # Trang đăng ký
│   └── Shared/           # Shared layouts
├── wwwroot/              # Static files
│   ├── css/              # Stylesheets
│   ├── js/               # JavaScript files
│   └── lib/              # Third-party libraries
└── Program.cs            # Application entry point
```

## 🎨 Tính năng hiện tại

### 🏠 Trang chủ (Index)

- Hiển thị thông tin tổng quan về nền tảng
- Navigation menu với Bootstrap

### 🔐 Đăng nhập (Login)

- Form đăng nhập với validation
- Giao diện theo thiết kế Figma
- Hỗ trợ đăng nhập Google (UI placeholder)

### 📝 Đăng ký (Register)

- Form đăng ký người dùng mới
- Validation dữ liệu đầu vào
- Xác nhận mật khẩu

### 📊 Dashboard

- Giao diện quản lý sau khi đăng nhập
- Hiển thị thống kê và thông tin người dùng

## 🛠️ Công nghệ sử dụng

- **Backend**: ASP.NET Core 8.0 MVC
- **Frontend**: Bootstrap 5.3.0, Font Awesome 6.4.0
- **Language**: C# 12, HTML5, CSS3, JavaScript

## 📝 API Endpoints

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/` | Trang chủ |
| GET | `/Home/Login` | Trang đăng nhập |
| POST | `/Home/Login` | Xử lý đăng nhập |
| GET | `/Home/Register` | Trang đăng ký |
| POST | `/Home/Register` | Xử lý đăng ký |
| GET | `/Home/Dashboard` | Dashboard (yêu cầu đăng nhập) |

## 🎯 Tính năng sắp tới

- [ ] Tích hợp cơ sở dữ liệu (Entity Framework Core)
- [ ] Hệ thống phân quyền (Identity)
- [ ] Tạo và quản lý đề thi
- [ ] Hệ thống thi trực tuyến
- [ ] Báo cáo và thống kê
- [ ] Tích hợp thanh toán
- [ ] API RESTful
- [ ] Mobile app support

## 🤝 Đóng góp

Chúng tôi hoan nghênh mọi đóng góp! Để đóng góp:

1. Fork dự án
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit thay đổi (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

## 📧 Liên hệ

- **Author**: TienxDun
- **GitHub**: [@TienxDun](https://github.com/TienxDun)
- **Project Link**: [https://github.com/TienxDun/E_TestHub](https://github.com/TienxDun/E_TestHub)

## 📄 License

Dự án này được phân phối dưới MIT License. Xem file `LICENSE` để biết thêm chi tiết.

---

⭐ Nếu dự án này hữu ích, hãy để lại một star trên GitHub! ⭐