<template>
  <div class="auth-container">
    <div class="auth-box">
      <!-- Giả sử bạn đã đặt logo vào thư mục public hoặc assets và đã cấu hình đúng -->
      <!-- Ví dụ nếu logo trong public: -->
      <img src="/ielts_logo_yellow_theme.png" alt="IELTS Prep Logo" class="logo">
      <!-- Ví dụ nếu logo trong src/assets và dùng alias @ -->
      <!-- <img src="@/assets/ielts_logo_yellow_theme.png" alt="IELTS Prep Logo" class="logo"> -->

      <h2>{{ isLogin ? 'Đăng nhập' : 'Đăng ký' }} tài khoản</h2>

      <!-- Form Đăng nhập -->
      <form v-if="isLogin" @submit.prevent="handleLogin" class="auth-form">
        <div class="form-group">
          <label for="login-email">Email</label>
          <input type="email" id="login-email" v-model="loginData.email" required placeholder="Nhập email của bạn" :disabled="loading">
        </div>
        <div class="form-group">
          <label for="login-password">Mật khẩu</label>
          <input type="password" id="login-password" v-model="loginData.password" required placeholder="Nhập mật khẩu" :disabled="loading">
          <!-- Lưu ý: Mật khẩu không được kiểm tra trong chế độ local này -->
        </div>
        <p v-if="authError" class="error-message">{{ authError }}</p>
        <button type="submit" class="btn btn-primary" :disabled="loading">
          {{ loading ? 'Đang xử lý...' : 'Đăng nhập' }}
        </button>
        <p class="toggle-form">
          Chưa có tài khoản?
          <!-- Sử dụng router-link hoặc button để điều hướng đến /register -->
          <button type="button" @click="toggleForm" class="btn-link" :disabled="loading">Đăng ký ngay</button>
        </p>
      </form>

      <!-- Form Đăng ký -->
      <form v-else @submit.prevent="handleRegister" class="auth-form">
         <div class="form-group">
          <label for="register-name">Họ và Tên</label>
          <input type="text" id="register-name" v-model="registerData.name" required placeholder="Nhập họ tên của bạn" :disabled="loading">
        </div>
        <div class="form-group">
          <label for="register-email">Email</label>
          <input type="email" id="register-email" v-model="registerData.email" required placeholder="Nhập email của bạn" :disabled="loading">
        </div>
        <div class="form-group">
          <label for="register-password">Mật khẩu</label>
          <input type="password" id="register-password" v-model="registerData.password" required placeholder="Tạo mật khẩu (ít nhất 6 ký tự)" :disabled="loading">
        </div>
        <div class="form-group">
          <label for="register-confirm-password">Xác nhận Mật khẩu</label>
          <input type="password" id="register-confirm-password" v-model="registerData.confirmPassword" required placeholder="Nhập lại mật khẩu" :disabled="loading">
        </div>
         <p v-if="passwordMismatch" class="error-message">Mật khẩu xác nhận không khớp!</p>
         <p v-if="authError" class="error-message">{{ authError }}</p>
        <button type="submit" class="btn btn-primary" :disabled="loading || passwordMismatch">
           {{ loading ? 'Đang xử lý...' : 'Đăng ký' }}
        </button>
        <p class="toggle-form">
          Đã có tài khoản?
          <!-- Sử dụng router-link hoặc button để điều hướng đến /login -->
          <button type="button" @click="toggleForm" class="btn-link" :disabled="loading">Đăng nhập</button>
        </p>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import axios from 'axios';

const router = useRouter();
const route = useRoute(); // Lấy thông tin route hiện tại

const API_BASE_URL = 'http://localhost:5206/api/v1';
const AUTH_TOKEN_KEY = 'userToken';
// const USER_ROLE_KEY = 'userRole'; // Key để lưu role user

// ----- Trạng thái và Dữ liệu Form -----
const isLogin = ref(true); // Mặc định là form đăng nhập
const loading = ref(false);
const authError = ref('');

const loginData = reactive({ email: '', password: '' });
const registerData = reactive({ name: '', email: '', password: '', confirmPassword: '' });

// ----- localStorage Keys (QUAN TRỌNG: Phải khớp với router/index.js) -----
// const MOCK_USER_DB_KEY = 'mockUserDatabase';

// ----- Computed Properties -----
const passwordMismatch = computed(() =>
!isLogin.value && // Chỉ kiểm tra khi ở form đăng ký
registerData.password &&
registerData.confirmPassword &&
registerData.password !== registerData.confirmPassword
);

// ----- localStorage Helper Functions -----

const storeAuthToken = (token) => {
  if (!token) {
      console.error("Cố gắng lưu token không hợp lệ.");
      return;
  }
  try {
      localStorage.setItem(AUTH_TOKEN_KEY, token);
      console.log(`Đã lưu key '${AUTH_TOKEN_KEY}' vào localStorage.`);
      // Không cần set header mặc định ở đây trừ khi bạn muốn mọi request axios đều có token
  } catch (e) {
      console.error("Lỗi lưu token vào localStorage:", e);
      authError.value = "Không thể lưu phiên đăng nhập. Bộ nhớ trình duyệt có thể đã đầy.";
  }
};

// Hàm lấy danh sách người dùng từ localStorage
// const getUsersFromStorage = () => {
// const usersJson = localStorage.getItem(MOCK_USER_DB_KEY);
// try {
//   return usersJson ? JSON.parse(usersJson) : [];
// } catch (e) {
//   console.error("Lỗi parse dữ liệu người dùng từ localStorage:", e);
//   localStorage.removeItem(MOCK_USER_DB_KEY); // Xóa dữ liệu hỏng nếu có lỗi parse
//   return [];
// }
// };

// Hàm lưu danh sách người dùng vào localStorage
// const saveUsersToStorage = (users) => {
// try {
//     localStorage.setItem(MOCK_USER_DB_KEY, JSON.stringify(users));
// } catch (e) {
//     console.error("Lỗi lưu dữ liệu người dùng vào localStorage:", e);
//     // Có thể thông báo lỗi cho người dùng nếu localStorage đầy
//     authError.value = "Không thể lưu dữ liệu người dùng. Bộ nhớ trình duyệt có thể đã đầy.";
// }
// };

// Hàm "đăng nhập" người dùng (lưu token/dấu hiệu vào localStorage)
// const logInUserLocally = (user) => {
//  // Chỉ cần lưu một giá trị bất kỳ vào key AUTH_TOKEN_KEY để router guard nhận biết
//  // Lưu email để tiện debug, hoặc lưu 'true', hoặc một mock token
//  localStorage.setItem(AUTH_TOKEN_KEY, user.email); // Hoặc localStorage.setItem(AUTH_TOKEN_KEY, 'mock-token-value');
//  console.log(`Đã lưu key '${AUTH_TOKEN_KEY}' vào localStorage cho user:`, user.email);
// };

// ----- Form Actions -----

// Chuyển đổi giữa form Đăng nhập và Đăng ký
const toggleForm = () => {
isLogin.value = !isLogin.value;
// Reset form data và lỗi khi chuyển
Object.keys(loginData).forEach(key => loginData[key] = '');
Object.keys(registerData).forEach(key => registerData[key] = '');
authError.value = '';

// Cập nhật URL (tùy chọn, nếu muốn URL thay đổi giữa /login và /register)
// const nextRouteName = isLogin.value ? 'Auth' : 'Register'; // Giả sử bạn đặt tên route là Auth và Register
// router.replace({ name: nextRouteName }); // Dùng replace để không thêm vào history
// HOẶC đơn giản là giữ nguyên component nhưng thay đổi trạng thái isLogin
};

// Xử lý Đăng nhập
// const handleLogin = () => {
// authError.value = '';
// loading.value = true;

// // Mô phỏng độ trễ mạng
// setTimeout(() => {
//   const users = getUsersFromStorage();
//   const foundUser = users.find(user => user.email === loginData.email);

//   // Quan trọng: Không kiểm tra mật khẩu thực tế trong local storage mode!
//   if (foundUser) {
//     console.log("Đăng nhập thành công (local) cho:", loginData.email);
//     logInUserLocally(foundUser); // Lưu trạng thái đăng nhập vào localStorage với key 'userToken'

//     // ---> CHUYỂN HƯỚNG ĐẾN DASHBOARD <---
//     router.push({ name: 'Dashboard' }); // Sử dụng tên route đã định nghĩa trong router/index.js

//   } else {
//     authError.value = 'Email không tồn tại hoặc mật khẩu không đúng.'; // Thông báo lỗi chung chung
//     console.log("Đăng nhập thất bại (local) - Email không tồn tại:", loginData.email);
//   }
//   loading.value = false;
// }, 500);
// };

const handleLogin = async () => {
  authError.value = '';
  loading.value = true;

  localStorage.removeItem('userToken');
  localStorage.removeItem('userRole'); // Nếu bạn có dùng key này trước đó
  localStorage.removeItem('adminUserToken');
  localStorage.removeItem('adminRole');

  try {
    const response = await axios.post(`${API_BASE_URL}/auth/login`, {
      email: loginData.email,
      password: loginData.password
    });

    console.log('Login response:', response.data);
    const token = response.data.token; // Giả sử API trả về { token: '...' }

    if (token) {
      storeAuthToken(token); // Lưu token
      router.push({ name: 'Dashboard' }); // Chuyển hướng
    } else {
      console.error('Lỗi đăng nhập: API response không chứa token.');
      authError.value = 'Đã xảy ra lỗi không mong đợi khi đăng nhập.';
    }

  } catch (error) {
    console.error('Lỗi đăng nhập API:', error.response ? error.response.data : error.message);
    // Lấy lỗi từ backend hoặc báo lỗi chung
    authError.value = error.response?.data?.message || 'Email hoặc mật khẩu không đúng.';
  } finally {
    loading.value = false;
  }
};

// Xử lý Đăng ký
// const handleRegister = () => {
// authError.value = '';

// if (passwordMismatch.value) {
//    authError.value = 'Mật khẩu xác nhận không khớp!';
//    return;
// }
//  if (registerData.password.length < 6) {
//       authError.value = 'Mật khẩu phải có ít nhất 6 ký tự.';
//       return;
//  }

// loading.value = true;

// // Mô phỏng độ trễ mạng
//  setTimeout(() => {
//     let users = getUsersFromStorage(); // Dùng let để có thể gán lại nếu xóa dữ liệu hỏng
//     const emailExists = users.some(user => user.email === registerData.email);

//     if (emailExists) {
//         authError.value = 'Email này đã được đăng ký (local).';
//         loading.value = false;
//         console.log("Đăng ký thất bại (local) - Email đã tồn tại:", registerData.email);
//         return;
//     }

//     // Chỉ lưu tên và email, **KHÔNG LƯU MẬT KHẨU** vào localStorage
//     const newUser = {
//         // Có thể thêm id đơn giản nếu cần, nhưng không bắt buộc với cách làm này
//         // id: Date.now().toString(),
//         name: registerData.name,
//         email: registerData.email
//     };

//     users.push(newUser);
//     saveUsersToStorage(users); // Lưu lại danh sách users

//     // Kiểm tra lại nếu saveUsersToStorage có báo lỗi
//     if (authError.value) {
//         loading.value = false;
//         return; // Dừng nếu không lưu được vào localStorage
//     }

//     console.log("Đăng ký thành công (local):", newUser);
//     logInUserLocally(newUser); // Tự động đăng nhập sau khi đăng ký (lưu token)

//     // ---> CHUYỂN HƯỚNG ĐẾN DASHBOARD <---
//     router.push({ name: 'Dashboard' }); // Sử dụng tên route

//     loading.value = false;
// }, 500);
// };

const handleRegister = async () => {
  authError.value = '';

  if (passwordMismatch.value) {
     authError.value = 'Mật khẩu xác nhận không khớp!';
     return;
  }
   if (registerData.password.length < 6) {
        authError.value = 'Mật khẩu phải có ít nhất 6 ký tự.';
        return;
   }

  loading.value = true;

  try {
    // Chuẩn bị payload đúng với yêu cầu API
    const payload = {
        name: registerData.name,
        email: registerData.email,
        password: registerData.password,
        confirm_password: registerData.confirm_password // Gửi confirm_password
    };

    console.log('Register payload:', payload); // Debug payload

    const response = await axios.post(`${API_BASE_URL}/auth/register`, payload);

    console.log('Register response:', response.data);

    // ---> GIẢ ĐỊNH API Register cũng trả token để tự động đăng nhập <---
    // Nếu API của bạn không trả token, bạn cần bỏ phần xử lý token dưới đây
    // và thay bằng thông báo thành công / chuyển sang form login.
    const token = response.data.token; // Kiểm tra xem có token không

    if (token) {
        console.log("Đăng ký thành công và tự động đăng nhập.");
        storeAuthToken(token); // Lưu token
        router.push({ name: 'Dashboard' }); // Chuyển hướng
    } else {
         console.log("Đăng ký thành công nhưng API không trả token. Yêu cầu đăng nhập.");
         authError.value = 'Đăng ký thành công! Vui lòng đăng nhập.';
         isLogin.value = true; // Chuyển sang form đăng nhập
         Object.keys(registerData).forEach(key => registerData[key] = ''); // Reset form đăng ký
    }

  } catch (error) {
    console.error('Lỗi đăng ký API:', error.response ? error.response.data : error.message);
    // Lấy lỗi từ backend hoặc báo lỗi chung
    authError.value = error.response?.data?.message || 'Đăng ký thất bại. Email có thể đã tồn tại.';
  } finally {
    loading.value = false;
  }
};

// ----- Lifecycle Hook -----
onMounted(() => {
  // Dựa vào route hiện tại để xác định nên hiển thị form nào ban đầu
  // Điều này hữu ích nếu bạn điều hướng trực tiếp đến /register
  if (route.name === 'Register') { // Giả sử route đăng ký có name là 'Register' trong router/index.js
      isLogin.value = false;
  } else { // Mặc định hoặc nếu route là 'Auth' (cho /login)
      isLogin.value = true;
  }
  // Reset lỗi khi component được mount lại
  authError.value = '';
});

</script>

<style scoped>
/* Sao chép toàn bộ style từ phiên bản trước đó của AuthPage.vue */
/* Biến màu sắc */
:root {
  --primary-yellow: #FFC107; /* Màu vàng Amber */
  --dark-yellow: #FFA000;
  --light-yellow: #FFF8E1;
  --text-dark: #333;
  --text-light: #555;
  --border-color: #ddd;
  --error-color: #dc3545;
  --white: #fff;
  --hover-button: #e0a800; /* Màu vàng đậm hơn khi hover */
}

.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh; /* Toàn màn hình chiều cao */
  background-color: var(--light-yellow); /* Nền vàng nhạt */
  padding: 20px;
  font-family: 'Arial', sans-serif; /* Chọn font phù hợp */
}

.auth-box {
  background-color: var(--white);
  padding: 40px 30px;
  border-radius: 8px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px; /* Giới hạn chiều rộng */
  text-align: center;
}

.logo {
    max-width: 150px; /* Điều chỉnh kích thước logo */
    margin-bottom: 20px;
}

.auth-box h2 {
  color: var(--text-dark);
  margin-bottom: 25px;
  font-weight: 600;
}

.auth-form .form-group {
  margin-bottom: 20px;
  text-align: left; /* Căn lề trái cho label và input */
}

.auth-form label {
  display: block;
  margin-bottom: 8px;
  color: var(--text-light);
  font-weight: 500;
  font-size: 0.9rem;
}

.auth-form input[type="text"],
.auth-form input[type="email"],
.auth-form input[type="password"] {
  width: 100%;
  padding: 12px 15px;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  box-sizing: border-box; /* Quan trọng để padding không làm tăng kích thước */
  font-size: 1rem;
  transition: border-color 0.3s ease;
}

.auth-form input[type="text"]:focus,
.auth-form input[type="email"]:focus,
.auth-form input[type="password"]:focus {
  border-color: var(--primary-yellow);
  outline: none; /* Bỏ outline mặc định */
  box-shadow: 0 0 0 2px rgba(255, 193, 7, 0.2); /* Hiệu ứng focus nhẹ */
}
/* Style khi input bị disable */
.auth-form input:disabled {
    background-color: #e9ecef;
    cursor: not-allowed;
}


.btn {
  padding: 12px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: background-color 0.3s ease, box-shadow 0.3s ease, opacity 0.3s ease;
  width: 100%; /* Nút chiếm toàn bộ chiều rộng */
  margin-top: 10px; /* Khoảng cách với input phía trên */
}

.btn-primary {
  background-color: var(--primary-yellow);
  color: var(--text-dark); /* Chữ màu tối trên nền vàng */
}

.btn-primary:hover:not(:disabled) { /* Chỉ hover khi không bị disable */
  background-color: var(--hover-button);
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.15);
}

/* Style khi nút bị disable */
.btn:disabled {
  background-color: #FFF8E1; /* Màu vàng nhạt hơn khi disable */
  color: #AE8E44; /* Màu chữ nhạt hơn */
  cursor: not-allowed;
  opacity: 0.7;
}


.toggle-form {
  margin-top: 25px;
  font-size: 0.9rem;
  color: var(--text-light);
}

.btn-link {
  background: none;
  border: none;
  color: var(--dark-yellow); /* Màu vàng đậm cho link */
  font-weight: 600;
  cursor: pointer;
  padding: 0;
  font-size: 0.9rem;
  margin-left: 5px; /* Thêm khoảng cách nhỏ */
}

.btn-link:hover:not(:disabled) {
  text-decoration: underline;
}
.btn-link:disabled {
    color: #AE8E44;
    cursor: not-allowed;
    text-decoration: none;
    opacity: 0.7;
}


.error-message {
    color: var(--error-color);
    font-size: 0.85rem;
    margin-top: 5px;
    margin-bottom: 15px;
    text-align: left;
    min-height: 1.2em; /* Giữ khoảng trống để layout không bị nhảy */
}

/* Responsive (Tùy chọn) */
@media (max-width: 480px) {
  .auth-box {
    padding: 30px 20px;
  }
   .auth-box h2 {
     font-size: 1.3rem;
   }
}
</style>