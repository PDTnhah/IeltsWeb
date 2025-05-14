<template>
    <header class="main-header">
      <nav class="container">
        <router-link to="/" class="logo-link">
           <img src="../assets/ietls.png" alt="IELTS Prep Logo" class="logo">
        </router-link>
        <ul class="nav-links">
          <li><router-link to="/" active-class="active-link" exact-active-class="active-link">Trang chủ</router-link></li>
          <!-- Chỉ hiển thị các link kỹ năng nếu đã đăng nhập -->
          <template v-if="isLoggedIn">
              <li><router-link to="/listening" active-class="active-link">Listening</router-link></li>
              <li><router-link to="/reading" active-class="active-link">Reading</router-link></li>
              <li><router-link to="/writing" active-class="active-link">Writing</router-link></li>
              <li><router-link to="/speaking" active-class="active-link">Speaking</router-link></li>
              <li><router-link to="/exams" active-class="active-link">Thi thử</router-link></li>
          </template>
        </ul>
  
        <div class="auth-links">
            <!-- Nếu chưa đăng nhập -->
            <router-link v-if="!isLoggedIn" to="/login" class="btn btn-login">Đăng nhập</router-link>
            <!-- Nếu đã đăng nhập -->
            <template v-else>
                <router-link to="/dashboard" class="nav-user-link">
                    <i class="icon-user"></i>
                    <span>{{ userDisplayName }}</span>
                </router-link>
                <button @click="logout" class="btn btn-logout">Đăng xuất</button>
            </template>
        </div>
        <!-- Có thể thêm nút menu cho mobile ở đây -->
      </nav>
    </header>
  </template>
  
  <script setup>
  import { ref, onMounted, watch } from 'vue';
  import { useRouter, useRoute } from 'vue-router';
  
  const router = useRouter();
  const route = useRoute(); // Dùng để theo dõi thay đổi route
  
  const isLoggedIn = ref(false);
  const userDisplayName = ref('');
  
  // Hàm kiểm tra trạng thái đăng nhập (đơn giản)
  const checkLoginStatus = () => {
      const token = localStorage.getItem('userToken');
      isLoggedIn.value = !!token; // Chuyển token thành boolean
      if (isLoggedIn.value) {
          const userData = JSON.parse(localStorage.getItem('userData') || '{}');
          userDisplayName.value = userData.name || 'Tài khoản'; // Lấy tên hiển thị
      } else {
          userDisplayName.value = '';
      }
       console.log("Header check login status:", isLoggedIn.value);
  };
  
  // Hàm đăng xuất
  const logout = () => {
    localStorage.removeItem('userToken');
    localStorage.removeItem('userData');
    isLoggedIn.value = false;
    userDisplayName.value = '';
    // Chuyển hướng về trang chủ hoặc trang login
    router.push('/'); // Hoặc '/login'
    console.log("Đã đăng xuất");
  };
  
  // Kiểm tra khi component mount
  onMounted(checkLoginStatus);
  
  // Theo dõi thay đổi route để cập nhật trạng thái header
  // (Quan trọng khi người dùng login/logout mà không F5)
  watch(() => route.path, () => {
       console.log("Route changed, checking login status in header");
      checkLoginStatus();
  });
  
  // Cũng có thể dùng event bus hoặc Pinia/Vuex để cập nhật header
  // thay vì watch route.path
  </script>
  
  <style scoped>
  /* Sử dụng lại biến màu từ AuthPage nếu có thể */
  :root {
    --primary-yellow: #FFC107;
    --dark-yellow: #FFA000;
    --text-dark: #333;
    --white: #fff;
    --header-bg: var(--white); /* Hoặc màu nền khác */
    --header-shadow: rgba(0, 0, 0, 0.1);
    --link-hover-bg: #fff3cd; /* Màu vàng nhạt hơn khi hover */
  }
  
  .main-header {
    background-color: var(--header-bg);
    box-shadow: 0 2px 4px var(--header-shadow);
    padding: 10px 0;
    position: sticky; /* Giữ header cố định khi cuộn */
    top: 0;
    z-index: 1000; /* Đảm bảo header nằm trên */
  }
  
  .container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 20px;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  .logo-link {
      display: flex;
      align-items: center;
      text-decoration: none;
      color: var(--text-dark);
  }
  
  .logo {
    height: 40px; /* Điều chỉnh kích thước logo */
    margin-right: 10px; /* Khoảng cách nếu có text logo */
  }
  .logo-text {
      font-size: 1.5rem;
      font-weight: bold;
      color: var(--primary-yellow);
  }
  
  
  .nav-links {
    list-style: none;
    padding: 0;
    margin: 0;
    display: flex;
    align-items: center; /* Căn giữa các mục nav */
    gap: 15px; /* Khoảng cách giữa các mục nav */
  }
  
  .nav-links li a {
    text-decoration: none;
    color: var(--text-dark);
    padding: 10px 15px;
    border-radius: 4px;
    transition: background-color 0.3s ease, color 0.3s ease;
    font-weight: 500;
  }
  
  .nav-links li a:hover,
  .nav-links li a.active-link { /* active-class và exact-active-class */
    background-color: var(--link-hover-bg);
    color: var(--dark-yellow);
  }
  
  /* Style cho nút Đăng nhập đặc biệt hơn */
  .btn-login {
      background-color: var(--primary-yellow);
      color: var(--text-dark) !important; /* Ghi đè màu chữ */
      border: none;
      padding: 8px 18px !important; /* Điều chỉnh padding cho nút */
      border-radius: 20px; /* Bo tròn hơn */
      font-weight: 600;
      transition: background-color 0.3s ease, box-shadow 0.3s ease;
  }
  
  .btn-login:hover {
      background-color: var(--dark-yellow) !important; /* Màu đậm hơn khi hover */
      box-shadow: 0 2px 5px rgba(0, 0, 0, 0.15);
  }

  /* Auth Links Container */
.auth-links {
    display: flex;
    align-items: center;
    gap: 15px;
}

/* User Link when logged in */
.nav-user-link {
    display: flex;
    align-items: center;
    gap: 8px;
    text-decoration: none;
    color: var(--text-dark);
    font-weight: 500;
    padding: 8px 10px;
    border-radius: 4px;
    transition: background-color 0.3s ease;
}
.nav-user-link:hover {
     background-color: var(--light-yellow);
     color: var(--dark-yellow);
}
.nav-user-link i { /* Style icon user */ font-size: 1.2rem; }

/* Logout button style */
.btn-logout {
    background-color: #f8f9fa; /* Màu nền sáng */
    color: var(--text-light);
    border: 1px solid #dee2e6;
    padding: 8px 18px;
    border-radius: 20px;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.3s, color 0.3s, border-color 0.3s;
}
.btn-logout:hover {
    background-color: #e2e6ea;
    border-color: #adb5bd;
    color: var(--text-dark);
}

.btn-login { /* Giữ style cũ hoặc điều chỉnh */
    background-color: var(--primary-yellow);
    color: var(--text-dark) !important;
    border: none;
    padding: 8px 18px !important;
    border-radius: 20px;
    font-weight: 600;
    transition: background-color 0.3s ease, box-shadow 0.3s ease;
    text-decoration: none;
}
.btn-login:hover { background-color: var(--dark-yellow) !important; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.15); }
  
  /* TODO: Add styles for mobile menu toggle */
  </style>