## 📖 Giới thiệu

**eTestHub** là nền tảng thi trắc nghiệm online hiện đại được xây dựng trên ASP.NET Core 8.0 MVC, cung cấp giải pháp toàn diện cho việc tổ chức và quản lý các kỳ thi trực tuyến dành cho trường học và tổ chức giáo dục.

### ✨ Tính năng chính

- 🎯 **Giao diện hiện đại**: Thiết kế responsive với Bootstrap 5.3 và Font Awesome 6.4
- 🔐 **Xác thực an toàn**: Hệ thống authentication với BCrypt password hashing
- 👥 **Phân quyền rõ ràng**: 3 vai trò (Admin, Teacher, Student) với giao diện riêng biệt
- 📝 **Quản lý bài thi**: Tạo, quản lý và theo dõi bài thi trực tuyến
- 📊 **Dashboard trực quan**: Dashboard riêng biệt cho từng vai trò
- 🎨 **UI/UX tối ưu**: Thiết kế dựa trên Figma specifications
- 📱 **Responsive Design**: Tương thích đa thiết bị
- 🗂️ **Session Management**: Quản lý phiên đăng nhập an toàn

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
├── Controllers/              # MVC Controllers
│   ├── HomeController.cs     # Authentication & public pages
│   ├── StudentController.cs  # Student features
│   ├── TeacherController.cs  # Teacher features
│   ├── AdminController.cs    # Admin features
│   └── BaseController.cs     # Base controller with shared logic
├── Models/                   # Data Models
│   ├── UserModels.cs         # User, LoginViewModel, DashboardViewModel
│   └── ErrorViewModel.cs     # Error handling
├── Services/                 # Business Logic Layer
│   └── UserService.cs        # User authentication & management
├── Views/                    # Razor Views
│   ├── Home/
│   │   ├── Index.cshtml      # Landing page
│   │   └── Login.cshtml      # Login page
│   ├── Student/              # Student views
│   │   ├── Dashboard.cshtml  # Student dashboard
│   │   ├── Classes.cshtml    # Class list
│   │   ├── MyExams.cshtml    # Exam list with filters
│   │   ├── ExamInfo.cshtml   # Exam details
│   │   ├── TakeExam.cshtml   # Exam interface
│   │   ├── ViewResults.cshtml# Results viewer
│   │   └── Profile.cshtml    # Student profile
│   ├── Teacher/              # Teacher views
│   │   └── Dashboard.cshtml  # Teacher dashboard
│   ├── Admin/                # Admin views
│   │   └── Dashboard.cshtml  # Admin dashboard
│   └── Shared/               # Shared layouts
│       ├── _Layout.cshtml    # Main layout
│       └── _DashboardLayout.cshtml # Dashboard layout
├── wwwroot/                  # Static files
│   ├── css/
│   │   ├── auth/            # Authentication pages CSS
│   │   ├── public/          # Public pages CSS
│   │   ├── shared/          # Shared dashboard CSS
│   │   └── student/         # Student-specific CSS
│   ├── js/                  # JavaScript files
│   ├── images/              # Images and assets
│   └── lib/                 # Third-party libraries (Bootstrap, jQuery)
└── Program.cs               # Application entry point & configuration
```

## 🎨 Tính năng hiện tại

### 🏠 Trang chủ (Index)

- Hiển thị thông tin tổng quan về nền tảng
- Navigation menu với Bootstrap
- Hero section với call-to-action
- Features showcase với gradient backgrounds
- Analytics và statistics sections
- Footer với thông tin liên hệ và social links

### 🔐 Xác thực người dùng

**Đăng nhập (Login)**
- Form đăng nhập với validation
- Giao diện theo thiết kế Figma
- Hỗ trợ đăng nhập Google (UI placeholder)
- BCrypt password hashing cho bảo mật
- Session-based authentication
- Auto redirect theo role sau khi đăng nhập

**Đăng ký (Register)**
- Form đăng ký người dùng mới
- Validation dữ liệu đầu vào
- Xác nhận mật khẩu
- Hiển thị thông báo lỗi/thành công

**Demo Accounts** (cho testing):
- Student: `student@demo.com` / `student123`
- Teacher: `teacher@demo.com` / `teacher123`
- Admin: `admin@demo.com` / `admin123`

### 👨‍🎓 Chức năng Sinh Viên (Student)

**Dashboard**
- Hiển thị thông tin chào mừng
- Quick action cards (Danh sách lớp, Bài thi, Tài khoản)
- Lớp truy cập gần đây
- Bài thi sắp tới với countdown
- Lịch sử làm bài

**Quản lý bài thi**
- Danh sách bài thi với filters (Trạng thái, Môn học)
- Tìm kiếm bài thi
- Chi tiết bài thi (ExamInfo) với:
  - Thông tin bài thi (ngày giờ, thời gian, số câu hỏi)
  - Trạng thái (Sắp diễn ra, Đang diễn ra, Đã hoàn thành)
  - Hướng dẫn làm bài
- Giao diện làm bài (TakeExam) với:
  - Thanh đếm ngược thời gian
  - Navigation giữa các câu hỏi
  - Review câu trả lời trước khi nộp
- Xem kết quả (ViewResults) với:
  - Điểm số và thống kê
  - Review từng câu hỏi với đáp án đúng/sai
  - Phân tích chi tiết

**Khác**
- Danh sách lớp (Classes)
- Quản lý tài khoản (Profile)

### 👨‍🏫 Chức năng Giáo Viên (Teacher)

**Dashboard**
- Welcome section với tên giáo viên
- Action cards:
  - Ngân hàng câu hỏi (QuestionBank)
  - Quản lý bài thi (ExamManagement)
  - Xem kết quả (ViewResults)
  - Quản lý lớp (ManageClasses)
- Danh sách lớp đang giảng dạy
- Kỳ thi đã tạo gần đây
- Thống kê nhanh

**Các tính năng khác** (Controllers ready):
- Tạo câu hỏi (CreateQuestion)
- Tạo bài thi (CreateExam)
- Chấm điểm (GradeExams)

### 👨‍💼 Chức năng Quản Trị (Admin)

**Dashboard**
- Welcome section
- Thống kê tổng quan:
  - Tổng số sinh viên (1,250 sinh viên)
  - Tổng số giáo viên (85 giáo viên)
  - Thống kê 15 khoa khác nhau
- Hoạt động gần đây:
  - Tài khoản mới
  - Cập nhật hệ thống
  - Sao lưu dữ liệu

**Các tính năng khác** (Controllers ready):
- Quản lý người dùng (UserManagement)
- Tạo tài khoản (CreateUser)
- Cài đặt hệ thống (SystemSettings)
- Báo cáo (Reports)
- Quản lý trường học (SchoolManagement)
- Sao lưu & khôi phục (BackupRestore)
- Nhật ký hệ thống (AuditLogs)

## 🛠️ Công nghệ sử dụng

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# 12
- **Authentication**: BCrypt.Net-Next 4.0.3
- **Session Management**: ASP.NET Core Session

### Frontend
- **CSS Framework**: Bootstrap 5.3.0
- **Icons**: Font Awesome 6.4.0
- **JavaScript**: Vanilla JS, jQuery 3.x
- **Markup**: HTML5, Razor Views (.cshtml)
- **Styling**: CSS3 với modular organization

### Architecture
- **Pattern**: MVC (Model-View-Controller)
- **Dependency Injection**: Built-in ASP.NET Core DI
- **Service Layer**: Interface-based services (IUserService)
- **View Engine**: Razor
- **Routing**: Convention-based và Attribute routing

## 📝 API Endpoints

### Public Routes
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/` | Trang chủ |
| GET | `/Home/Login` | Trang đăng nhập |
| POST | `/Home/Login` | Xử lý đăng nhập |
| GET | `/Home/Register` | Trang đăng ký |
| POST | `/Home/Register` | Xử lý đăng ký |
| GET | `/Home/Logout` | Đăng xuất |

### Student Routes
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/Student/Dashboard` | Dashboard sinh viên |
| GET | `/Student/Classes` | Danh sách lớp |
| GET | `/Student/MyExams` | Danh sách bài thi |
| GET | `/Student/ExamInfo/{examId}` | Chi tiết bài thi |
| GET | `/Student/TakeExam/{examId}` | Làm bài thi |
| GET | `/Student/ViewResults` | Xem kết quả |
| GET | `/Student/Profile` | Thông tin cá nhân |

### Teacher Routes
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/Teacher/Dashboard` | Dashboard giáo viên |
| GET | `/Teacher/QuestionBank` | Ngân hàng câu hỏi |
| GET | `/Teacher/CreateQuestion` | Tạo câu hỏi mới |
| GET | `/Teacher/ExamManagement` | Quản lý bài thi |
| GET | `/Teacher/CreateExam` | Tạo bài thi mới |
| GET | `/Teacher/ViewResults` | Xem kết quả |
| GET | `/Teacher/ManageClasses` | Quản lý lớp học |
| GET | `/Teacher/GradeExams` | Chấm điểm |

### Admin Routes
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/Admin/Dashboard` | Dashboard quản trị |
| GET | `/Admin/UserManagement` | Quản lý người dùng |
| GET | `/Admin/CreateUser` | Tạo tài khoản mới |
| GET | `/Admin/SystemSettings` | Cài đặt hệ thống |
| GET | `/Admin/Reports` | Báo cáo thống kê |
| GET | `/Admin/SchoolManagement` | Quản lý trường học |
| GET | `/Admin/BackupRestore` | Sao lưu & khôi phục |
| GET | `/Admin/AuditLogs` | Nhật ký hệ thống |

## 🎨 CSS Organization

Dự án sử dụng cấu trúc CSS modular được tổ chức theo vai trò và chức năng:

```
wwwroot/css/
├── auth/              # Authentication pages
│   └── login.css      # Login page styles
├── public/            # Public pages
│   └── index.css      # Homepage styles
├── shared/            # Shared across roles
│   └── dashboard.css  # Common dashboard layout
└── student/           # Student-specific
    ├── my-exams.css   # Exam list page
    ├── exam-info.css  # Exam details page
    ├── take-exam.css  # Exam taking interface
    ├── view-results.css # Results viewer
    └── profile.css    # Student profile
```

**Lợi ích:**
- ✅ Modularity: CSS được tổ chức theo chức năng và role
- ✅ Maintainability: Dễ dàng tìm và chỉnh sửa CSS cho từng module
- ✅ Scalability: Dễ dàng mở rộng cho Teacher, Admin roles
- ✅ Performance: Có thể optimize loading CSS theo từng role

## 📧 Liên hệ

- **Author**: TienxDun
- **GitHub**: [@TienxDun](https://github.com/TienxDun)
- **Project Link**: [https://github.com/TienxDun/E_TestHub](https://github.com/TienxDun/E_TestHub)

## 📄 License

Dự án này được phân phối dưới MIT License. Xem file `LICENSE` để biết thêm chi tiết.

---

⭐ Nếu dự án này hữu ích, hãy để lại một star trên GitHub! ⭐