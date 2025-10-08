# 🎓 E_TestHub - Nền tảng Thi Trắc Nghiệm Online

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue.svg)
![C#](https://img.shields.io/badge/C%23-12.0-purple.svg)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3.0-blueviolet.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## 📖 Giới thiệu

**E_TestHub** là nền tảng thi trắc nghiệm online hiện đại được xây dựng trên ASP.NET Core 8.0 MVC, cung cấp giải pháp toàn diện cho việc tổ chức và quản lý các kỳ thi trực tuyến với 3 role chính: **Sinh viên**, **Giảng viên**, và **Quản trị viên**.

### ✨ Tính năng chính

#### 👨‍� Dành cho Sinh viên
- 📋 **Dashboard cá nhân**: Tổng quan bài thi và tiến độ học tập
- 📝 **Quản lý bài thi**: Xem danh sách, lọc và tìm kiếm bài thi
- ⏰ **Theo dõi trạng thái**: Bài thi sắp diễn ra, đang diễn ra, đã hoàn thành
- � **Xem kết quả**: Xem chi tiết đáp án và điểm số
- 👤 **Quản lý hồ sơ**: Cập nhật thông tin cá nhân

#### 👨‍🏫 Dành cho Giảng viên
- 📝 **Tạo đề thi**: Quản lý câu hỏi và đề thi
- 📊 **Theo dõi kết quả**: Xem điểm và phân tích thống kê
- � **Quản lý lớp học**: Danh sách sinh viên và bài thi

#### 🔧 Dành cho Quản trị viên
- 👥 **Quản lý người dùng**: Thêm, sửa, xóa tài khoản
- 📊 **Thống kê hệ thống**: Báo cáo tổng quan về hoạt động
- ⚙️ **Cấu hình hệ thống**: Thiết lập và quản lý

#### 🎨 Tính năng kỹ thuật
- 🔐 **Bảo mật**: Mã hóa mật khẩu với BCrypt
- 🎯 **MVC Architecture**: Kiến trúc rõ ràng, dễ bảo trì
- 📱 **Responsive Design**: Tương thích mọi thiết bị
- 🎨 **Modern UI**: Thiết kế hiện đại với CSS modular
- ⚡ **Performance**: Tối ưu tốc độ tải trang

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
├── 📂 Controllers/              # MVC Controllers
│   ├── AdminController.cs       # Quản lý Admin
│   ├── BaseController.cs        # Base controller cho authentication
│   ├── HomeController.cs        # Public pages (Index, Login)
│   ├── StudentController.cs     # Quản lý Sinh viên
│   └── TeacherController.cs     # Quản lý Giảng viên
│
├── 📂 Models/                   # Data Models
│   ├── ErrorViewModel.cs        # Error handling model
│   └── UserModels.cs            # User authentication models
│
├── 📂 Services/                 # Business Logic Services
│   └── UserService.cs           # User authentication service
│
├── 📂 Views/                    # Razor Views
│   ├── 📁 Admin/                # Admin views
│   │   └── Dashboard.cshtml     # Admin dashboard
│   │
│   ├── 📁 Home/                 # Public views
│   │   ├── Index.cshtml         # Landing page
│   │   └── Login.cshtml         # Login page
│   │
│   ├── 📁 Student/              # Student views
│   │   ├── Dashboard.cshtml     # Student dashboard
│   │   ├── MyExams.cshtml       # Danh sách bài thi
│   │   ├── ExamInfo.cshtml      # Thông tin chi tiết bài thi
│   │   ├── TakeExam.cshtml      # Trang làm bài thi
│   │   ├── ViewResults.cshtml   # Xem kết quả bài thi
│   │   ├── Classes.cshtml       # Quản lý lớp học
│   │   └── Profile.cshtml       # Thông tin cá nhân
│   │
│   ├── 📁 Teacher/              # Teacher views
│   │   └── Dashboard.cshtml     # Teacher dashboard
│   │
│   └── 📁 Shared/               # Shared layouts & partials
│       ├── _Layout.cshtml       # Public layout
│       ├── _DashboardLayout.cshtml  # Dashboard layout
│       ├── _ValidationScriptsPartial.cshtml
│       └── Error.cshtml         # Error page
│
├── 📂 wwwroot/                  # Static files
│   ├── 📁 css/                  # Stylesheets (Organized)
│   │   ├── 📁 shared/           # Shared CSS
│   │   │   └── dashboard.css    # Common dashboard styles
│   │   ├── 📁 student/          # Student-specific CSS
│   │   │   ├── my-exams.css
│   │   │   ├── exam-info.css
│   │   │   ├── take-exam.css
│   │   │   ├── view-results.css
│   │   │   └── profile.css
│   │   ├── 📁 auth/             # Authentication CSS
│   │   │   └── login.css
│   │   └── 📁 public/           # Public CSS
│   │       └── index.css
│   │
│   ├── 📁 js/                   # JavaScript files
│   │   └── login.js             # Login page scripts
│   │
│   ├── 📁 images/               # Image assets
│   │   └── dashboard-mockup.png
│   │
│   └── 📁 lib/                  # Third-party libraries
│       ├── bootstrap/           # Bootstrap 5.3
│       ├── jquery/              # jQuery
│       ├── jquery-validation/   # Form validation
│       └── jquery-validation-unobtrusive/
│
├── 📄 Program.cs                # Application entry point
├── 📄 appsettings.json          # Configuration
├── 📄 E_TestHub.csproj          # Project file
└── 📄 README.md                 # Documentation
```

## 🎨 Tính năng đã hoàn thành

### 🔐 Authentication & Authorization

- ✅ **Login System**: Đăng nhập với BCrypt password hashing
- ✅ **Session Management**: Quản lý phiên đăng nhập
- ✅ **Role-based Access**: Phân quyền theo 3 roles (Student, Teacher, Admin)
- ✅ **Base Controller**: Authentication middleware

### 👨‍🎓 Student Module

#### Dashboard
- ✅ Welcome section với tên người dùng
- ✅ Quick action cards (Vào lớp học, Làm bài thi, Xem kết quả)
- ✅ Danh sách bài thi sắp tới
- ✅ Lịch sử bài thi gần đây

#### Quản lý bài thi (My Exams)
- ✅ **Danh sách bài thi**: Hiển thị tất cả bài thi
- ✅ **Bộ lọc**: Lọc theo trạng thái (Sắp diễn ra, Đang diễn ra, Đã hoàn thành)
- ✅ **Tìm kiếm**: Search bài thi theo tên hoặc môn học
- ✅ **Sắp xếp tự động**: Bài thi gần nhất lên đầu
- ✅ **Trạng thái động**: Badge màu sắc theo trạng thái

#### Thông tin bài thi (Exam Info)
- ✅ **Chi tiết bài thi**: Thời gian, số câu hỏi, loại đề
- ✅ **Hướng dẫn làm bài**: Instructions rõ ràng
- ✅ **Status Banner**: Hiển thị trạng thái bài thi hiện tại
- ✅ **Smart Actions**: 
  - Nút "Bắt đầu thi" chỉ hiện khi bài thi đang diễn ra
  - Nút "Chưa đến giờ thi" khi bài thi chưa bắt đầu
  - Nút "Xem kết quả" khi bài thi đã hoàn thành

#### Làm bài thi (Take Exam)
- ✅ **Giao diện làm bài**: Clean và tập trung
- ✅ **Navigation**: Chuyển câu hỏi dễ dàng
- ✅ **Timer**: Đếm ngược thời gian (UI ready)
- ✅ **Question Grid**: Hiển thị tất cả câu hỏi

#### Xem kết quả (View Results)
- ✅ **Kết quả chi tiết**: Điểm số và phần trăm
- ✅ **Question Review**: Xem lại từng câu hỏi
- ✅ **Answer Analysis**: 
  - Hiển thị đáp án đã chọn
  - Hiển thị đáp án đúng
  - Status: Đúng/Sai với màu sắc
- ✅ **Navigation Grid**: Click để xem từng câu
- ✅ **Statistics Sidebar**: 
  - Tổng điểm
  - Số câu đúng/sai
  - Thời gian làm bài
- ✅ **Demo Data**: 12 câu hỏi Tiếng Anh hoàn chỉnh

#### Profile
- ✅ **Thông tin cá nhân**: Hiển thị và chỉnh sửa
- ✅ **Avatar management**: Upload ảnh đại diện
- ✅ **Security settings**: Đổi mật khẩu

### 🎨 UI/UX Features

- ✅ **Modular CSS**: Tổ chức CSS theo module
- ✅ **Responsive Design**: Tương thích mobile/tablet/desktop
- ✅ **Modern Color Scheme**: Professional blue theme
- ✅ **Font Awesome Icons**: Icon system hoàn chỉnh
- ✅ **Smooth Animations**: Hover effects và transitions
- ✅ **Status Badges**: Color-coded status indicators

## 🛠️ Công nghệ sử dụng

### Backend

- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# 12.0
- **Architecture**: Model-View-Controller (MVC)
- **Authentication**: BCrypt.Net-Next 4.0.3
- **Session Management**: ASP.NET Core Session

### Frontend

- **CSS Framework**: Bootstrap 5.3.0
- **Icons**: Font Awesome 6.x
- **JavaScript**: Vanilla JS (ES6+)
- **jQuery**: 3.7.0
- **Validation**: jQuery Validation + Unobtrusive

### Development Tools

- **IDE**: Visual Studio 2022 / VS Code
- **Version Control**: Git & GitHub
- **Design**: Figma (UI/UX mockups)

## 📝 Routes & Endpoints

### Public Routes

| Method | Endpoint | Controller | Action | Mô tả |
|--------|----------|------------|--------|-------|
| GET | `/` | Home | Index | Trang chủ |
| GET | `/Home/Login` | Home | Login | Trang đăng nhập |
| POST | `/Home/Login` | Home | Login | Xử lý đăng nhập |

### Student Routes (Requires Authentication)

| Method | Endpoint | Controller | Action | Mô tả |
|--------|----------|------------|--------|-------|
| GET | `/Student/Dashboard` | Student | Dashboard | Dashboard sinh viên |
| GET | `/Student/Classes` | Student | Classes | Danh sách lớp học |
| GET | `/Student/MyExams` | Student | MyExams | Danh sách bài thi |
| GET | `/Student/ExamInfo/{id}` | Student | ExamInfo | Thông tin bài thi |
| GET | `/Student/TakeExam/{id}` | Student | TakeExam | Làm bài thi |
| GET | `/Student/ViewResults` | Student | ViewResults | Xem kết quả |
| GET | `/Student/Profile` | Student | Profile | Thông tin cá nhân |

### Teacher Routes (Requires Authentication)

| Method | Endpoint | Controller | Action | Mô tả |
|--------|----------|------------|--------|-------|
| GET | `/Teacher/Dashboard` | Teacher | Dashboard | Dashboard giảng viên |

### Admin Routes (Requires Authentication)

| Method | Endpoint | Controller | Action | Mô tả |
|--------|----------|------------|--------|-------|
| GET | `/Admin/Dashboard` | Admin | Dashboard | Dashboard quản trị |

## 👨‍💻 Development Guidelines
### CSS Organization

```
css/
├── shared/      # CSS dùng chung cho tất cả modules
├── student/     # CSS riêng cho Student pages
├── teacher/     # CSS riêng cho Teacher pages (future)
├── admin/       # CSS riêng cho Admin pages (future)
├── auth/        # CSS cho authentication pages
└── public/      # CSS cho public pages
```
## � Project Status

| Component | Status | Progress |
|-----------|--------|----------|
| Student Module | ✅ Completed | 100% |
| Teacher Module | 🚧 In Progress | 10% |
| Admin Module | 🚧 In Progress | 10% |
| Database Integration | ⏳ Planned | 0% |
| API Development | ⏳ Planned | 0% |
| Testing | ⏳ Planned | 0% |

## 📄 License

Dự án này được phân phối dưới **MIT License**. Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

```
MIT License

Copyright (c) 2025 TienxDun

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
```

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Bootstrap Documentation](https://getbootstrap.com/docs/)
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) (Planned)

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  (Views - Razor Pages, CSS, JavaScript)                 │
└───────────────────────────┬─────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────┐
│                   Application Layer                      │
│  (Controllers - MVC Pattern, Routing)                   │
└───────────────────────────┬─────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────┐
│                    Business Layer                        │
│  (Services - Business Logic, Validation)                │
└───────────────────────────┬─────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────┐
│                      Data Layer                          │
│  (Models - Data Transfer Objects)                       │
└─────────────────────────────────────────────────────────┘

