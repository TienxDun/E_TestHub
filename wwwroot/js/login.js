// eTestHub Login JavaScript

document.addEventListener("DOMContentLoaded", function () {
  // Form validation

  const loginForm = document.getElementById("loginForm");

  if (loginForm) {
    loginForm.addEventListener("submit", async function (e) {
      e.preventDefault();

      const emailInput = this.querySelector('input[name="Email"]');

      const passwordInput = this.querySelector('input[name="Password"]');

      const roleInput = this.querySelector("#selectedRole");

      const email = emailInput ? emailInput.value.trim() : "";

      const password = passwordInput ? passwordInput.value : "";

      const role = roleInput ? roleInput.value : "";

      if (!email || !password) {
        alert("Vui lòng nhập đầy đủ thông tin");

        return;
      }

      if (!isValidEmail(email)) {
        alert("Vui lòng nhập email hợp lệ");

        return;
      }

      if (!role) {
        alert(
          "Vui lòng chọn loại tài khoản: Sinh viên / Giáo viên / Quản trị viên"
        );

        return;
      }

      // Gọi API đăng nhập và điều hướng theo role

      try {
        await loginWithApi({ email, password, role });
      } catch (err) {
        console.error(err);

        alert(err && err.message ? err.message : "Đăng nhập thất bại");
      }
    });
  }

  // Google login button

  const googleBtn = document.querySelector(".etesthub-btn-google");

  if (googleBtn) {
    googleBtn.addEventListener("click", function () {
      console.log("Google login clicked");

      alert("Chức năng đăng nhập Google sẽ được triển khai sau");
    });
  }

  // Forgot password functionality

  const loginFormElement = document.getElementById("loginForm");

  const forgotPasswordForm = document.getElementById("forgotPasswordForm");

  const forgotPasswordLink = document.getElementById("forgotPasswordLink");

  const backToLoginLink = document.getElementById("backToLoginLink");

  if (forgotPasswordLink && loginFormElement && forgotPasswordForm) {
    forgotPasswordLink.addEventListener("click", function (e) {
      e.preventDefault();

      loginFormElement.style.display = "none";

      forgotPasswordForm.style.display = "block";
    });
  }

  if (backToLoginLink && loginFormElement && forgotPasswordForm) {
    backToLoginLink.addEventListener("click", function (e) {
      e.preventDefault();

      loginFormElement.style.display = "block";

      forgotPasswordForm.style.display = "none";
    });
  }

  // Xử lý click trên các button role
  // Loại bỏ onclick từ HTML và chỉ dùng event listener để tránh conflict
  function initializeRoleButtons() {
    const roleButtons = document.querySelectorAll("[data-role]");

    console.log("Tìm thấy", roleButtons.length, "button role");

    if (roleButtons.length === 0) {
      console.warn("Không tìm thấy button nào có data-role");
      // Thử lại sau 100ms
      setTimeout(initializeRoleButtons, 100);
      return;
    }

    roleButtons.forEach((btn, index) => {
      // Loại bỏ onclick attribute để tránh conflict
      btn.removeAttribute("onclick");

      // Đảm bảo button có thể click được
      btn.style.cursor = "pointer";
      btn.style.pointerEvents = "auto";

      // Thêm event listener
      btn.addEventListener("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        const role = (this.getAttribute("data-role") || "").toLowerCase();
        console.log("Button clicked, role:", role);

        if (!role || !/^(student|teacher|admin)$/.test(role)) {
          console.warn("Invalid role:", role);
          return;
        }

        console.log("Calling selectRole with:", role);
        selectRole(role);
      });

      console.log(
        `Button ${index + 1} initialized:`,
        btn.getAttribute("data-role")
      );
    });
  }

  // Khởi tạo ngay lập tức
  initializeRoleButtons();

  // Đảm bảo chạy lại nếu DOM chưa sẵn sàng
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initializeRoleButtons);
  }
});

// Email validation function

function isValidEmail(email) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  return emailRegex.test(email);
}

// Xóa các hàm demo và chọn role theo yêu cầu

// Chọn role từ các nút
// Định nghĩa ở global scope
function selectRole(role) {
  console.log("selectRole called with:", role);

  // Đảm bảo role là lowercase
  role = (role || "").toLowerCase();

  if (!role || !/^(student|teacher|admin)$/.test(role)) {
    console.warn("Invalid role:", role);
    return;
  }

  let hidden = document.getElementById("selectedRole");

  // Nếu chưa có hidden, tạo bổ sung để tránh lỗi không set được giá trị
  if (!hidden) {
    const form = document.getElementById("loginForm");

    if (form) {
      hidden = document.createElement("input");
      hidden.type = "hidden";
      hidden.name = "selectedRole";
      hidden.id = "selectedRole";
      form.appendChild(hidden);
      console.log("Created hidden input for selectedRole");
    } else {
      console.error("Không tìm thấy form loginForm");
      return;
    }
  }

  const current = hidden ? hidden.value : "";
  const willSelect = current !== role; // click lại để bỏ chọn

  console.log(
    "Current role:",
    current,
    "New role:",
    role,
    "willSelect:",
    willSelect
  );

  if (hidden) {
    hidden.value = willSelect ? role : "";
    console.log("Set hidden input value to:", hidden.value);
  }

  // highlight nút đang chọn (đổi outline -> solid theo role)
  const map = {
    student: { outline: "btn-outline-primary", solid: "btn-primary" },
    teacher: { outline: "btn-outline-success", solid: "btn-success" },
    admin: { outline: "btn-outline-warning", solid: "btn-warning" },
  };

  const buttons = document.querySelectorAll("[data-role]");
  console.log("Found", buttons.length, "buttons with data-role");

  if (buttons.length === 0) {
    console.warn("Không tìm thấy button nào có data-role");
    return;
  }

  buttons.forEach((btn, index) => {
    const r = (btn.getAttribute("data-role") || "").toLowerCase();
    console.log(`Button ${index + 1}: role="${r}"`);

    if (!map[r]) {
      console.warn(`Button ${index + 1} has invalid role:`, r);
      return; // bỏ qua phần tử không hợp lệ
    }

    const isActive = willSelect && r === role;
    console.log(`Button ${index + 1} (${r}): isActive=${isActive}`);

    // reset tất cả class màu trước
    const classesToRemove = [
      "btn-primary",
      "btn-success",
      "btn-warning",
      "text-white",
      "btn-outline-primary",
      "btn-outline-success",
      "btn-outline-warning",
    ];

    classesToRemove.forEach((cls) => btn.classList.remove(cls));
    console.log(`Button ${index + 1} classes after remove:`, btn.className);

    // áp lại class phù hợp
    if (isActive) {
      btn.classList.add(map[r].solid, "text-white");
      console.log(
        `Button ${index + 1} added classes: ${map[r].solid}, text-white`
      );
    } else {
      btn.classList.add(map[r].outline);
      console.log(`Button ${index + 1} added class: ${map[r].outline}`);
    }

    console.log(`Button ${index + 1} final classes:`, btn.className);
  });

  console.log(
    "Role selection completed. Selected role:",
    willSelect ? role : "(none)"
  );
}

// Đăng nhập qua API và điều hướng theo role

async function loginWithApi({ email, password, role }) {
  // Gọi ASP.NET endpoint (sẽ proxy đến Node.js API và set session)

  const API_URL = "/api/auth/login";

  const res = await fetch(API_URL, {
    method: "POST",

    headers: {
      "Content-Type": "application/json",
    },

    body: JSON.stringify({ email, password, role }),
  });

  if (!res.ok) {
    // cố gắng đọc lỗi từ server nếu có

    let message = "Đăng nhập thất bại";

    try {
      const errJson = await res.json();

      if (errJson && errJson.message) message = errJson.message;
    } catch (_) {}

    throw new Error(message);
  }

  const data = await res.json();

  // Kỳ vọng server trả về { success, role, token, redirectUrl }

  if (!data || data.success === false) {
    throw new Error(
      (data && data.message) || "Tài khoản hoặc mật khẩu không đúng"
    );
  }

  if (data.role && data.role !== role) {
    throw new Error("Bạn không có quyền đăng nhập với loại tài khoản đã chọn");
  }

  // Lưu token (nếu có)

  if (data.token) {
    try {
      localStorage.setItem("etesthub_token", data.token);
    } catch (_) {}
  }

  // Điều hướng

  const redirectUrl =
    data.redirectUrl ||
    (role === "student"
      ? "/Student/Dashboard"
      : role === "teacher"
      ? "/Teacher/Dashboard"
      : "/Admin/Dashboard");

  window.location.href = redirectUrl;
}
