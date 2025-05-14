<template>
    <div class="dashboard-page container">
      <h1 class="page-title">Chào mừng trở lại, {{ userName }}!</h1>
      <p class="page-description">Đây là khu vực học tập cá nhân của bạn.</p>
  
      <div class="dashboard-grid">
        <!-- Card Tổng quan tiến độ -->
        <div class="dashboard-card progress-card">
          <h3>Tiến độ học tập</h3>
          <div class="progress-overview">
              <div class="progress-item">
                  <label>Listening</label>
                  <progress max="100" value="30"></progress> <span>30%</span>
              </div>
               <div class="progress-item">
                  <label>Reading</label>
                  <progress max="100" value="55"></progress> <span>55%</span>
              </div>
               <div class="progress-item">
                  <label>Writing</label>
                  <progress max="100" value="15"></progress> <span>15%</span>
              </div>
               <div class="progress-item">
                  <label>Speaking</label>
                  <progress max="100" value="20"></progress> <span>20%</span>
              </div>
          </div>
          <p class="subtext">Tiếp tục cố gắng nhé!</p>
        </div>
  
        <!-- Card Bài học/Thi gần đây -->
        <div class="dashboard-card recent-activity-card">
          <h3>Hoạt động gần đây</h3>
          <ul>
            <li><router-link to="/lessons/listening/map-labelling-intro">Đã xem: Giới thiệu về Map Labelling</router-link></li>
            <li><router-link to="/exams/academic-reading-test-set-a">Đã xem: Bộ đề Reading Học thuật - A</router-link></li>
            <li><router-link to="/lessons/writing/task-1-line-graph">Đã xem: Phân tích biểu đồ đường (Line Graph)</router-link></li>
          </ul>
           <router-link to="/history" class="view-all-link">Xem tất cả lịch sử</router-link>
        </div>
  
        <!-- Card Gợi ý học tập -->
        <div class="dashboard-card recommendations-card">
           <h3>Gợi ý cho bạn</h3>
           <p>Dựa trên tiến độ của bạn, hãy thử:</p>
           <ul>
               <li><router-link to="/lessons/writing/task-2-essay-structure">Bài học: Cấu trúc bài luận Task 2</router-link></li>
               <li><router-link to="/lessons/speaking/part-2-cue-card-practice">Bài học: Luyện tập Part 2 Cue Card</router-link></li>
               <li><router-link to="/exams/full-practice-test-1">Thi thử: Bài Thi Thử Đầy Đủ số 1</router-link></li>
           </ul>
        </div>
  
         <!-- Card Truy cập nhanh -->
         <div class="dashboard-card quick-links-card">
             <h3>Truy cập nhanh</h3>
             <div class="quick-links-grid">
                  <router-link to="/listening" class="quick-link">🎧 Listening</router-link>
                  <router-link to="/reading" class="quick-link">📖 Reading</router-link>
                  <router-link to="/writing" class="quick-link">✍️ Writing</router-link>
                  <router-link to="/speaking" class="quick-link">🗣️ Speaking</router-link>
                  <router-link to="/exams" class="quick-link">⏱️ Thi thử</router-link>
                   <router-link to="/profile" class="quick-link">👤 Hồ sơ</router-link>
             </div>
         </div>
  
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  // import { RouterLink } from 'vue-router'; // Không cần
  
  // --- Dữ liệu Mock Người dùng (Sẽ lấy từ state/API) ---
  const userName = ref('Thí sinh IELTS');
  
  // --- Logic Lấy tên thật/dữ liệu khác khi mount ---
  onMounted(() => {
      const storedUser = localStorage.getItem('userData'); // Ví dụ lấy từ localStorage
      if (storedUser) {
          try {
              const userData = JSON.parse(storedUser);
              userName.value = userData.name || 'Bạn'; // Lấy tên nếu có
          } catch (e) {
              console.error("Lỗi parse user data");
          }
      }
      // Gọi API để lấy dữ liệu dashboard thực tế
      // fetchDashboardData();
  });
  
  </script>
  
  <style scoped>
  :root { /* ... Biến màu ... */ --card-bg: var(--white); --progress-bar-bg: #e9ecef; --progress-bar-fill: var(--primary-yellow); --link-color: #007bff; }
  
  .container { max-width: 1200px; margin: 30px auto; padding: 0 20px; }
  .page-title { color: var(--dark-yellow); margin-bottom: 5px; font-size: 2rem; }
  .page-description { color: var(--text-light); margin-bottom: 30px; font-size: 1.1rem; }
  
  .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(350px, 1fr)); /* Responsive grid */
      gap: 25px;
  }
  
  .dashboard-card {
      background-color: var(--card-bg);
      padding: 25px 30px;
      border-radius: 8px;
      box-shadow: 0 3px 12px rgba(0,0,0,0.07);
  }
  
  .dashboard-card h3 {
      font-size: 1.4rem;
      color: var(--dark-yellow);
      margin-top: 0;
      margin-bottom: 20px;
      padding-bottom: 10px;
      border-bottom: 1px solid var(--border-color);
  }
  
  /* Progress Card */
  .progress-overview .progress-item {
      margin-bottom: 15px;
      display: flex;
      align-items: center;
      gap: 10px;
      font-size: 0.95rem;
  }
  .progress-item label {
      width: 80px; /* Căn chỉnh label */
      color: var(--text-light);
  }
  .progress-item progress {
      flex-grow: 1;
      height: 10px;
      border-radius: 5px;
      overflow: hidden; /* Cho border-radius hoạt động trên Chrome */
      border: none; /* Bỏ border mặc định */
      background-color: var(--progress-bar-bg);
  }
  /* Style cho thanh progress */
  .progress-item progress::-webkit-progress-bar { background-color: var(--progress-bar-bg); border-radius: 5px;}
  .progress-item progress::-webkit-progress-value { background-color: var(--progress-bar-fill); border-radius: 5px; transition: width 0.5s ease;}
  .progress-item progress::-moz-progress-bar { background-color: var(--progress-bar-fill); border-radius: 5px; transition: width 0.5s ease;}
  .progress-item span { font-weight: 500; width: 40px; text-align: right; }
  .progress-card .subtext { text-align: center; color: var(--text-light); font-size: 0.9rem; margin-top: 15px; }
  
  /* Recent Activity & Recommendations Card */
  .recent-activity-card ul, .recommendations-card ul {
      list-style: none;
      padding: 0;
      margin: 0 0 15px 0;
  }
  .recent-activity-card li, .recommendations-card li {
      margin-bottom: 10px;
      font-size: 0.95rem;
  }
  .recent-activity-card a, .recommendations-card a {
      color: var(--link-color);
      text-decoration: none;
  }
  .recent-activity-card a:hover, .recommendations-card a:hover {
      text-decoration: underline;
      color: var(--dark-yellow);
  }
  .view-all-link {
      display: inline-block;
      margin-top: 10px;
      font-size: 0.9rem;
      color: var(--text-light);
  }
  .view-all-link:hover {
      color: var(--dark-yellow);
  }
  
  
  /* Quick Links Card */
  .quick-links-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr); /* 2 cột */
      gap: 15px;
  }
  .quick-link {
      display: block;
      background-color: var(--light-yellow);
      color: var(--dark-yellow);
      padding: 12px 15px;
      border-radius: 5px;
      text-align: center;
      font-weight: 500;
      text-decoration: none;
      transition: background-color 0.3s ease, color 0.3s ease;
  }
  .quick-link:hover {
      background-color: var(--primary-yellow);
      color: var(--text-dark);
  }
  
  @media (max-width: 992px) {
       .dashboard-grid {
          grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
       }
  }
  @media (max-width: 768px) {
      .page-title { font-size: 1.8rem; }
      .dashboard-grid { grid-template-columns: 1fr; } /* 1 cột */
       .quick-links-grid { grid-template-columns: 1fr; } /* 1 cột */
  }
  </style>