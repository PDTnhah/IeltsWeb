<template>
    <div class="admin-layout">
      <AdminSidebar />
      <main class="admin-main-content">
        <router-view /> 
      </main>
    </div>
  </template>
  
  <script setup>
  import AdminSidebar from '@/components/admin/AdminSidebar.vue';
  // Có thể thêm logic kiểm tra đăng nhập admin ở đây nếu cần
  </script>
  
  <style scoped>
  .admin-layout {
    display: flex;
    min-height: 100vh;
    background-color: #f4f7fc; /* Màu nền nhẹ cho admin */
  }
  
  .admin-main-content {
    flex-grow: 1;
    padding: 30px;
    overflow-x: hidden; /* Tránh scroll ngang không mong muốn */
  }
  </style>