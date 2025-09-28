# CSS Organization Structure

## 📁 Cấu trúc thư mục CSS đã được tổ chức lại theo module và role

### 🗂️ **Shared** (`/css/shared/`)
CSS dùng chung cho tất cả các role và layout:
- `dashboard.css` - Layout dashboard chung cho Student, Teacher, Admin

### 👨‍🎓 **Student** (`/css/student/`)
CSS riêng cho các trang của Student:
- `my-exams.css` - Trang danh sách bài thi
- `exam-info.css` - Trang thông tin chi tiết bài thi
- `take-exam.css` - Trang làm bài thi
- `view-results.css` - Trang xem kết quả bài thi
- `profile.css` - Trang thông tin cá nhân

### 🔐 **Auth** (`/css/auth/`)
CSS cho các trang authentication:
- `login.css` - Trang đăng nhập

### 🌐 **Public** (`/css/public/`)
CSS cho các trang công khai:
- `index.css` - Trang chủ

## 📋 **Migration Status**

✅ **Completed**:
- Tạo cấu trúc thư mục mới
- Di chuyển tất cả file CSS vào thư mục phù hợp
- Cập nhật đường dẫn CSS trong tất cả View files:
  - Student Views: 6 files updated
  - Home Views: 2 files updated  
  - Shared Views: 1 file updated

✅ **Files Moved**:
- `dashboard.css` → `shared/dashboard.css`
- `my-exams.css` → `student/my-exams.css`
- `exam-info.css` → `student/exam-info.css`
- `take-exam.css` → `student/take-exam.css`
- `view-results.css` → `student/view-results.css`
- `profile.css` → `student/profile.css`
- `Login.css` → `auth/login.css`
- `Index.css` → `public/index.css`

## 🎯 **Benefits of New Structure**

1. **Modularity**: CSS được tổ chức theo chức năng và role
2. **Maintainability**: Dễ dàng tìm và chỉnh sửa CSS cho từng module
3. **Scalability**: Dễ dàng thêm CSS mới cho Teacher, Admin roles
4. **Organization**: Cấu trúc rõ ràng, khoa học
5. **Performance**: Có thể optimize loading CSS theo từng role

## 🔄 **Next Steps**

- [ ] Có thể thêm thư mục `teacher/` và `admin/` khi cần
- [ ] Xem xét tạo file CSS variables chung trong `shared/`
- [ ] Có thể minify CSS files cho production

## ⚠️ **Legacy Files**

Các file CSS cũ ở thư mục gốc sẽ được giữ lại để đảm bảo tương thích, nhưng không còn được sử dụng:
- ~~dashboard.css~~ → `shared/dashboard.css`
- ~~my-exams.css~~ → `student/my-exams.css`
- ~~exam-info.css~~ → `student/exam-info.css`
- ~~take-exam.css~~ → `student/take-exam.css`
- ~~view-results.css~~ → `student/view-results.css`
- ~~profile.css~~ → `student/profile.css`
- ~~Login.css~~ → `auth/login.css`
- ~~Index.css~~ → `public/index.css`