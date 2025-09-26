// eTestHub JavaScript functionality
document.addEventListener("DOMContentLoaded", () => {
  // Form validation
  const loginForm = document.getElementById("loginForm");
  if (loginForm) {
    loginForm.addEventListener("submit", function (e) {
      const email = this.querySelector('input[name="email"]').value
      const password = this.querySelector('input[name="password"]').value

      if (!email || !password) {
        e.preventDefault()
        alert("Vui lòng nhập đầy đủ thông tin")
        return
      }

      if (!isValidEmail(email)) {
        e.preventDefault()
        alert("Vui lòng nhập email hợp lệ")
        return
      }
    })
  }

  // Google login button
  const googleBtn = document.querySelector(".etesthub-btn-google")
  if (googleBtn) {
    googleBtn.addEventListener("click", () => {
      // TODO: Implement Google OAuth
      console.log("Google login clicked")
    })
  }

  // Sidebar navigation
  const sidebarIcons = document.querySelectorAll(".etesthub-sidebar-icon")
  sidebarIcons.forEach((icon) => {
    icon.addEventListener("click", function () {
      sidebarIcons.forEach((i) => i.classList.remove("active"))
      this.classList.add("active")
    })
  })

    const loginFormElement = document.getElementById('loginForm');
    const forgotPasswordForm = document.getElementById('forgotPasswordForm');
    const forgotPasswordLink = document.getElementById('forgotPasswordLink');
    const backToLoginLink = document.getElementById('backToLoginLink');

    if (forgotPasswordLink) {
        forgotPasswordLink.addEventListener('click', function (e) {
            e.preventDefault();
            loginFormElement.style.display = 'none';
            forgotPasswordForm.style.display = 'block';
        });
    }

    if (backToLoginLink) {
        backToLoginLink.addEventListener('click', function (e) {
            e.preventDefault();
            loginFormElement.style.display = 'block';
            forgotPasswordForm.style.display = 'none';
        });
    }
})

function isValidEmail(email) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}
