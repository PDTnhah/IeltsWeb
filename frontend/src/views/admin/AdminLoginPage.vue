<template>
    <div class="admin-login-page">
      <div class="login-box">
        <h2>Admin Login</h2>
        <form @submit.prevent="handleAdminLogin">
          <div class="form-group">
            <label for="admin-email">Email</label>
            <input type="email" id="admin-email" v-model="loginData.email" required :disabled="loading">
          </div>
          <div class="form-group">
            <label for="admin-password">Mật khẩu</label>
            <input type="password" id="admin-password" v-model="loginData.password" required :disabled="loading">
          </div>
          <p v-if="authError" class="error-message">{{ authError }}</p>
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Đang xử lý...' : 'Đăng nhập Admin' }}
          </button>
        </form>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, reactive } from 'vue';
  import { useRouter } from 'vue-router';
//   import axios from 'axios';
  
  const router = useRouter();
  const loading = ref(false);
  const authError = ref('');
  const loginData = reactive({ email: '', password: '' });
  
//   const API_BASE_URL = 'http://localhost:5206/api/v1'; // Hoặc URL API admin
//   const AUTH_TOKEN_KEY = 'adminUserToken'; // Key riêng cho admin token
//   const ADMIN_ROLE_KEY = 'adminUserRole'; // Key để lưu role
const ADMIN_TOKEN_KEY = 'adminUserToken';
const ADMIN_ROLE_KEY = 'adminRole';

  // *** Thông tin đăng nhập mô phỏng ***
const SIMULATED_ADMIN_EMAIL = 'manager@gmail.com';
const SIMULATED_ADMIN_PASSWORD = '123123123';

  
//   const handleAdminLogin = async () => {
//     authError.value = '';
//     loading.value = true;
//     try {
//       // Gọi API đăng nhập (có thể là endpoint chung hoặc riêng cho admin)
//       const response = await axios.post(`${API_BASE_URL}/auth/login`, { // Hoặc /admin/auth/login
//         email: loginData.email,
//         password: loginData.password,
//       });
  
//       // Kiểm tra xem response có token và có phải là admin không
//       // Backend cần trả về role hoặc endpoint này chỉ admin mới login được
//       const token = response.data.token;
//       const role = response.data.role; // Backend PHẢI trả về role
  
//       if (token && role === 'Admin') { // Kiểm tra role
//         localStorage.setItem(AUTH_TOKEN_KEY, token);
//         localStorage.setItem(ADMIN_ROLE_KEY, role); // Lưu role
//         console.log('Admin login successful');
//         router.push('/admin/dashboard'); // Điều hướng đến dashboard admin
//       } else {
//         // Nếu login thành công nhưng không phải role Admin
//         authError.value = 'Tài khoản không có quyền truy cập Admin.';
//         // Không lưu token nếu không phải admin
//       }
//     } catch (error) {
//       console.error('Admin login error:', error.response ? error.response.data : error.message);
//       authError.value = error.response?.data?.message || 'Đăng nhập Admin thất bại.';
//     } finally {
//       loading.value = false;
//     }
//   };

const handleAdminLogin = () => { // Không cần async nữa
  authError.value = '';
  loading.value = true;

  localStorage.removeItem('userToken');
  localStorage.removeItem('userRole'); // Nếu bạn có dùng key này trước đó
  localStorage.removeItem('adminUserToken');
  localStorage.removeItem('adminRole');

  // --- Mô phỏng kiểm tra đăng nhập ---
  setTimeout(() => { // Thêm độ trễ nhỏ cho giống thật
    if (loginData.email === SIMULATED_ADMIN_EMAIL && loginData.password === SIMULATED_ADMIN_PASSWORD) {
      // --- Đăng nhập thành công (mô phỏng) ---
      console.log('Simulated Admin login successful');

      // Tạo mock token (không cần gọi API)
      const mockToken = `mock-admin-token-${Date.now()}`;

      // Lưu thông tin admin vào localStorage
      localStorage.setItem(ADMIN_TOKEN_KEY, mockToken);
      localStorage.setItem(ADMIN_ROLE_KEY, 'Admin'); // Quan trọng: Lưu đúng role 'Admin'

      // Xóa thông tin user cũ nếu có (tránh xung đột)
      localStorage.removeItem('userToken');
      localStorage.removeItem('userRole');


      // Điều hướng đến dashboard admin
      router.push({ name: 'AdminDashboard' });

    } else {
      // --- Đăng nhập thất bại (mô phỏng) ---
      console.log('Simulated Admin login failed');
      authError.value = 'Simulated login failed: Invalid credentials.';
      // Đảm bảo xóa token/role cũ nếu đăng nhập sai
      localStorage.removeItem(ADMIN_TOKEN_KEY);
      localStorage.removeItem(ADMIN_ROLE_KEY);
    }
    loading.value = false;
  }, 300); // Độ trễ 0.3 giây
};

  </script>
  
  <style scoped>
  .admin-login-page {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background-color: #ecf0f1;
  }
  .login-box {
    background-color: white;
    padding: 40px 30px;
    border-radius: 8px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
    width: 100%;
    max-width: 400px;
    text-align: center;
  }
  /* Style cho form, button, error tương tự AuthPage */
  .form-group { margin-bottom: 20px; text-align: left; }
  label { display: block; margin-bottom: 8px; color: #555; font-weight: 500; }
  input[type="email"], input[type="password"] { width: 100%; padding: 12px 15px; border: 1px solid #ddd; border-radius: 4px; box-sizing: border-box; font-size: 1rem; }
  input:disabled { background-color: #e9ecef; cursor: not-allowed; }
  .btn { padding: 12px 20px; border: none; border-radius: 4px; cursor: pointer; font-size: 1rem; font-weight: 600; width: 100%; margin-top: 10px; }
  .btn-primary { background-color: #3498db; color: white; }
  .btn-primary:hover:not(:disabled) { background-color: #2980b9; }
  .btn:disabled { background-color: #a9cce3; cursor: not-allowed; }
  .error-message { color: #e74c3c; font-size: 0.85rem; margin-bottom: 15px; text-align: left; min-height: 1.2em; }
  </style>