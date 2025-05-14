<template>
    <aside class="admin-sidebar">
      <div class="sidebar-header">
        <h3>Admin Panel</h3>
      </div>
      <nav class="sidebar-nav">
        <ul>
          <li>
            <router-link to="/admin/dashboard" active-class="active-link">
              <i class="fas fa-tachometer-alt"></i>
              <span>Tổng quan</span>
            </router-link>
          </li>
          <li>
            <router-link to="/admin/users" active-class="active-link">
              <i class="fas fa-users"></i>
              <span>Quản lý Người dùng</span>
            </router-link>
          </li>
          <li>
            <router-link to="/admin/lessons" active-class="active-link">
              <i class="fas fa-book-open"></i>
              <span>Quản lý Bài học</span>
            </router-link>
          </li>
          <li>
            <router-link to="/admin/exams" active-class="active-link">
              <i class="fas fa-file-alt"></i>
              <span>Quản lý Bài thi</span>
            </router-link>
          </li>
        </ul>
      </nav>
      <div class="sidebar-footer">
        <button @click="handleLogout" class="btn-logout">
          <i class="fas fa-sign-out-alt"></i>
          <span>Đăng xuất</span>
        </button>
      </div>
    </aside>
  </template>
  
  <script setup>
  import { useRouter } from 'vue-router';
  
  const router = useRouter();
  const AUTH_TOKEN_KEY = 'adminUserToken'; // Hoặc key bạn dùng cho admin
  
  const handleLogout = () => {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    // Xóa các thông tin admin khác nếu có
    router.push('/admin/login'); // Chuyển về trang login admin
  };
  </script>
  
  <style scoped>
  .admin-sidebar {
    width: 250px;
    background-color: #2c3e50; /* Màu nền tối cho sidebar */
    color: #ecf0f1; /* Màu chữ sáng */
    display: flex;
    flex-direction: column;
    height: 100vh; /* Chiều cao cố định */
    position: sticky; /* Giữ sidebar cố định khi scroll */
    top: 0;
  }
  
  .sidebar-header {
    padding: 20px;
    text-align: center;
    border-bottom: 1px solid #34495e;
  }
  
  .sidebar-header h3 {
    margin: 0;
    color: #ffffff;
  }
  
  .sidebar-nav {
    flex-grow: 1;
    padding-top: 20px;
  }
  
  .sidebar-nav ul {
    list-style: none;
    padding: 0;
    margin: 0;
  }
  
  .sidebar-nav li a {
    display: flex;
    align-items: center;
    padding: 12px 20px;
    color: #bdc3c7; /* Màu chữ nhạt hơn */
    text-decoration: none;
    transition: background-color 0.2s ease, color 0.2s ease;
    font-size: 0.95rem;
  }
  
  .sidebar-nav li a i {
    margin-right: 12px;
    width: 20px; /* Đảm bảo icon thẳng hàng */
    text-align: center;
  }
  
  .sidebar-nav li a:hover {
    background-color: #34495e;
    color: #ffffff;
  }
  
  .sidebar-nav li a.active-link {
    background-color: #1abc9c; /* Màu highlight khi active */
    color: #ffffff;
    font-weight: 600;
  }
  
  .sidebar-footer {
    padding: 20px;
    border-top: 1px solid #34495e;
  }
  
  .btn-logout {
    background: none;
    border: 1px solid #e74c3c;
    color: #e74c3c;
    padding: 8px 15px;
    border-radius: 4px;
    cursor: pointer;
    width: 100%;
    text-align: center;
    transition: background-color 0.2s ease, color 0.2s ease;
    font-size: 0.9rem;
    display: flex;
    align-items: center;
    justify-content: center;
  }
  .btn-logout i {
      margin-right: 8px;
  }
  
  .btn-logout:hover {
    background-color: #e74c3c;
    color: #ffffff;
  }
  </style>