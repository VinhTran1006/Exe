// Sign Up Form Submission
document.getElementById('signupForm').addEventListener('submit', async function (e) {
    e.preventDefault();

// Clear previous errors
    clearErrors();

    const formData = {
     username: document.getElementById('regUsername').value.trim(),
        email: document.getElementById('regEmail').value.trim(),
        name: document.getElementById('regName').value.trim(),
 phone: document.getElementById('regPhone').value.trim() || null,
        gender: document.getElementById('regGender').value || null,
      dateOfBirth: document.getElementById('regDateOfBirth').value || null,
      address: document.getElementById('regAddress').value.trim() || null,
        password: document.getElementById('regPassword').value,
      confirmPassword: document.getElementById('regConfirmPassword').value
    };

    // Client-side validation
    if (!formData.username || !formData.email || !formData.name || !formData.password) {
        showSignupError('All required fields must be filled.');
        return;
    }

    if (!isValidEmail(formData.email)) {
        document.getElementById('emailError').textContent = 'Please enter a valid email address.';
    return;
    }

    if (formData.password.length < 6) {
        document.getElementById('passwordError').textContent = 'Password must be at least 6 characters long.';
        return;
    }

    if (formData.password !== formData.confirmPassword) {
        document.getElementById('passwordError').textContent = 'Passwords do not match.';
        return;
    }

    try {
    const response = await fetch('/api/auth/signup', {
            method: 'POST',
            headers: {
            'Content-Type': 'application/json'
            },
     body: JSON.stringify(formData)
        });

        const data = await response.json();

        if (data.success) {
   showSignupSuccess('Account created successfully! Please sign in.');
            document.getElementById('signupForm').reset();
            
// Redirect to sign in after 2 seconds
         setTimeout(() => {
                document.getElementById('signIn').click();
 }, 2000);
        } else {
    showSignupError(data.message || 'Sign up failed. Please try again.');
      }
    } catch (error) {
    console.error('Error:', error);
        showSignupError('An error occurred. Please try again later.');
  }
});

// Sign In Form Submission
document.querySelector('.login-form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const formData = {
        username: document.getElementById('loginUsername').value.trim(),
        password: document.getElementById('loginPassword').value
    };

    if (!formData.username || !formData.password) {
   showLoginError('Username and password are required.');
   return;
    }

    try {
const response = await fetch('/api/auth/signin', {
            method: 'POST',
    headers: {
       'Content-Type': 'application/json'
         },
    body: JSON.stringify(formData)
  });

        const data = await response.json();

        if (data.success) {
    showLoginSuccess('Login successful! Redirecting...');
            
       // Store user info in localStorage (optional)
   localStorage.setItem('userId', data.user.userId);
          localStorage.setItem('username', data.user.username);
            localStorage.setItem('userEmail', data.user.email);

// Redirect to dashboard or home page after 2 seconds
            setTimeout(() => {
    window.location.href = '/dashboard'; // Change to your desired redirect URL
            }, 2000);
        } else {
  showLoginError(data.message || 'Invalid username or password.');
        }
    } catch (error) {
        console.error('Error:', error);
        showLoginError('An error occurred. Please try again later.');
    }
});

// Utility Functions
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function showSignupError(message) {
    const alertDiv = document.getElementById('signupAlert');
    alertDiv.innerHTML = `<div class="alert alert-danger" role="alert">${message}</div>`;
}

function showSignupSuccess(message) {
    const alertDiv = document.getElementById('signupAlert');
    alertDiv.innerHTML = `<div class="alert alert-success" role="alert">${message}</div>`;
}

function showLoginError(message) {
 const alertDiv = document.getElementById('loginAlert');
    alertDiv.innerHTML = `<div class="alert alert-danger" role="alert">${message}</div>`;
}

function showLoginSuccess(message) {
    const alertDiv = document.getElementById('loginAlert');
    alertDiv.innerHTML = `<div class="alert alert-success" role="alert">${message}</div>`;
}

function clearErrors() {
    document.getElementById('usernameError').textContent = '';
    document.getElementById('emailError').textContent = '';
    document.getElementById('phoneError').textContent = '';
    document.getElementById('passwordError').textContent = '';
}

// Home button - optional
const homeButton = document.getElementById('btnHome');
if (homeButton) {
    homeButton.addEventListener('click', function () {
     window.location.href = '/';
    });
}
