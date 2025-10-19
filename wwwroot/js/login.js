// eTestHub Login JavaScript
document.addEventListener("DOMContentLoaded", function() {
    // Form validation
    const loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", function(e) {
            const emailInput = this.querySelector('input[name="Email"]');
            const passwordInput = this.querySelector('input[name="Password"]');
            
            const email = emailInput ? emailInput.value : '';
            const password = passwordInput ? passwordInput.value : '';

            if (!email || !password) {
                e.preventDefault();
                alert("Vui lòng nhập đầy đủ thông tin");
                return;
            }

            if (!isValidEmail(email)) {
                e.preventDefault();
                alert("Vui lòng nhập email hợp lệ");
                return;
            }
        });
    }

    // Google login button
    const googleBtn = document.querySelector(".etesthub-btn-google");
    if (googleBtn) {
        googleBtn.addEventListener("click", function() {
            console.log("Google login clicked");
            alert("Chức năng đăng nhập Google sẽ được triển khai sau");
        });
    }

    // Forgot password functionality
    const loginFormElement = document.getElementById('loginForm');
    const forgotPasswordForm = document.getElementById('forgotPasswordForm');
    const forgotPasswordLink = document.getElementById('forgotPasswordLink');
    const backToLoginLink = document.getElementById('backToLoginLink');

    if (forgotPasswordLink && loginFormElement && forgotPasswordForm) {
        forgotPasswordLink.addEventListener('click', function(e) {
            e.preventDefault();
            loginFormElement.style.display = 'none';
            forgotPasswordForm.style.display = 'block';
        });
    }

    if (backToLoginLink && loginFormElement && forgotPasswordForm) {
        backToLoginLink.addEventListener('click', function(e) {
            e.preventDefault();
            loginFormElement.style.display = 'block';
            forgotPasswordForm.style.display = 'none';
        });
    }
});

// Email validation function
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Quick login function for demo accounts
function quickLogin(email, password) {
    const emailInput = document.querySelector('input[name="Email"]');
    const passwordInput = document.querySelector('input[name="Password"]');
    
    if (emailInput && passwordInput) {
        emailInput.value = email;
        passwordInput.value = password;
        
        // Add visual feedback
        emailInput.classList.add('is-valid');
        passwordInput.classList.add('is-valid');
        
        // Focus to show the fields are filled
        emailInput.focus();
        setTimeout(function() {
            passwordInput.focus();
        }, 100);
    } else {
        console.error('Could not find email or password input fields');
    }
}