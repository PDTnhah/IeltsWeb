<template>
    <div class="admin-dashboard">
      <h1>Tổng quan hệ thống</h1>
      <div v-if="loading" class="loading-state">Đang tải dữ liệu...</div>
      <div v-else-if="error" class="error-state">Lỗi: {{ error }}</div>
      <div v-else class="stats-grid">
        <div class="stat-card users">
          <i class="fas fa-users"></i>
          <div class="stat-value">{{ stats.totalUsers }}</div>
          <div class="stat-label">Người dùng</div>
        </div>
        <div class="stat-card lessons">
          <i class="fas fa-book"></i>
          <div class="stat-value">{{ stats.totalLessons }}</div>
          <div class="stat-label">Bài học</div>
        </div>
        <div class="stat-card exams">
          <i class="fas fa-file-signature"></i>
          <div class="stat-value">{{ stats.totalExams }}</div>
          <div class="stat-label">Bài thi</div>
        </div>
        <div class="stat-card activity">
          <i class="fas fa-chart-line"></i>
          <div class="stat-value">{{ stats.recentLogins }}</div>
          <div class="stat-label">Lượt truy cập (24h)</div>
        </div>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted, reactive } from 'vue';
  import axios from 'axios';
  
  const loading = ref(true);
  const error = ref('');
  const stats = reactive({
    totalUsers: 0,
    totalLessons: 0,
    totalExams: 0,
    recentLogins: 0,
  });
  
  const API_BASE_URL = 'http://localhost:5206/api/v1';
  const AUTH_TOKEN_KEY = 'adminUserToken'; // Key token admin
  
  const fetchStats = async () => {
    loading.value = true;
    error.value = '';
    const token = localStorage.getItem(AUTH_TOKEN_KEY);
    if (!token) {
        error.value = "Không tìm thấy token xác thực admin.";
        loading.value = false;
        // Có thể điều hướng về login
        return;
    }
  
    try {
      // **Backend Cần Endpoint Này**: GET /api/v1/admin/stats
      const response = await axios.get(`${API_BASE_URL}/admin/stats`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      // Cập nhật stats dựa trên response.data
      Object.assign(stats, response.data); // Giả sử API trả về đúng cấu trúc
    } catch (err) {
      console.error("Error fetching admin stats:", err.response || err);
      error.value = err.response?.data?.message || "Không thể tải dữ liệu tổng quan.";
      if (err.response?.status === 401 || err.response?.status === 403) {
          // Token hết hạn hoặc không hợp lệ
          error.value += " Vui lòng đăng nhập lại.";
          // Có thể xử lý logout tự động
      }
    } finally {
      loading.value = false;
    }
  };
  
  onMounted(fetchStats);
  </script>
  
  <style scoped>
  .admin-dashboard h1 {
    margin-bottom: 30px;
    color: #2c3e50;
  }
  .stats-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
    gap: 25px;
  }
  .stat-card {
    background-color: #ffffff;
    padding: 25px;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.07);
    text-align: center;
    border-left: 5px solid; /* Viền màu bên trái */
  }
  .stat-card i {
    font-size: 2.5rem;
    margin-bottom: 15px;
    display: block;
  }
  .stat-value {
    font-size: 2.2rem;
    font-weight: 700;
    margin-bottom: 5px;
  }
  .stat-label {
    font-size: 0.95rem;
    color: #555;
  }
  /* Màu sắc cho từng card */
  .stat-card.users { border-color: #3498db; color: #3498db; }
  .stat-card.lessons { border-color: #2ecc71; color: #2ecc71; }
  .stat-card.exams { border-color: #f39c12; color: #f39c12; }
  .stat-card.activity { border-color: #9b59b6; color: #9b59b6; }
  
  .loading-state, .error-state {
    padding: 20px;
    text-align: center;
    color: #555;
  }
  .error-state { color: #e74c3c; }
  </style>