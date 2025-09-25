// eTestHub JavaScript functionality
document.addEventListener("DOMContentLoaded", () => {
  // Form validation
  const loginForm = document.querySelector(".etesthub-form")
  if (loginForm) {
    loginForm.addEventListener("submit", function (e) {
      e.preventDefault()

      const email = this.querySelector('input[type="email"]').value
      const password = this.querySelector('input[type="password"]').value

      if (!email || !password) {
        alert("Vui lòng nhập đầy đủ thông tin")
        return
      }

      if (!isValidEmail(email)) {
        alert("Vui lòng nhập email hợp lệ")
        return
      }

      // Submit form or make AJAX call
      this.submit()
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
})

function isValidEmail(email) {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

// Utility functions for future use
const ETestHub = {
  showLoading: () => {
    // TODO: Implement loading spinner
  },

  hideLoading: () => {
    // TODO: Hide loading spinner
  },

  showMessage: (message, type = "info") => {
    // TODO: Implement toast notifications
    console.log(`${type}: ${message}`)
  },
}
