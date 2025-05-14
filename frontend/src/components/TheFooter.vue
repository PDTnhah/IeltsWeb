<template>
    <footer class="main-footer">
      <div class="container">
        <p>© {{ currentYear }} Your IELTS Prep Site. All rights reserved.</p>
        <div class="footer-links">
          <router-link to="/terms">Điều khoản</router-link> |
          <router-link to="/privacy">Chính sách bảo mật</router-link>
        </div>
      </div>
    </footer>
  </template>
  
  <script setup>
  import { computed } from 'vue';
  const currentYear = computed(() => new Date().getFullYear());
  </script>
  
  <style scoped>
  .main-footer {
    background-color: #f1f1f1; /* Màu nền xám nhạt cho footer */
    color: #555;
    padding: 20px 0;
    margin-top: 40px; /* Khoảng cách với nội dung bên trên */
    border-top: 1px solid #ddd;
    font-size: 0.9rem;
  }
  
  .container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 20px;
    display: flex;
    justify-content: space-between;
    align-items: center;
     flex-wrap: wrap; /* Cho phép xuống dòng trên mobile */
     gap: 10px;
  }
  
   .footer-links a {
       color: #555;
       text-decoration: none;
       margin: 0 5px;
   }
    .footer-links a:hover {
       color: var(--primary-yellow, #FFC107); /* Dùng màu vàng nếu biến có sẵn */
       text-decoration: underline;
   }
  
    @media (max-width: 768px) {
      .container {
          flex-direction: column; /* Xếp chồng lên nhau trên mobile */
          text-align: center;
      }
  }
  </style>