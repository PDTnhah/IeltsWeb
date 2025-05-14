<template>
    <div class="home-page">
      <!-- 1. Hero Section -->
      <section class="hero-section">
        <div class="container hero-content">
          <h1>Chinh phục IELTS cùng chúng tôi</h1>
          <p>Lộ trình học cá nhân hóa, tài liệu phong phú, luyện tập không giới hạn.</p>
          <router-link to="/register" class="btn btn-cta">Bắt đầu ngay</router-link>
        </div>
      </section>
  
      <!-- 2. Features Section -->
      <section class="features-section">
        <div class="container">
          <h2>Tại sao chọn chúng tôi?</h2>
          <div class="features-grid">
            <div class="feature-item">
              <i class="feature-icon">📚</i> <!-- Thay bằng icon thật -->
              <h3>Tài liệu độc quyền</h3>
              <p>Cập nhật liên tục các dạng đề thi mới nhất và mẹo làm bài hiệu quả.</p>
            </div>
            <div class="feature-item">
              <i class="feature-icon">🎯</i> <!-- Thay bằng icon thật -->
              <h3>Lộ trình cá nhân hóa</h3>
              <p>Xác định điểm mạnh, yếu và xây dựng kế hoạch học tập phù hợp.</p>
            </div>
            <div class="feature-item">
              <i class="feature-icon">🗣️</i> <!-- Thay bằng icon thật -->
              <h3>Luyện tập kỹ năng</h3>
              <p>Thực hành chuyên sâu 4 kỹ năng Nghe, Nói, Đọc, Viết với phản hồi chi tiết.</p>
            </div>
             <div class="feature-item">
              <i class="feature-icon">⏱️</i> <!-- Thay bằng icon thật -->
              <h3>Thi thử như thật</h3>
              <p>Trải nghiệm các bài thi thử định kỳ với cấu trúc và thời gian chuẩn.</p>
            </div>
          </div>
        </div>
      </section>
  
       <!-- 3. Sections for Skills (Optional) -->
       <section class="skills-overview">
           <div class="container">
               <h2>Khám phá các kỹ năng</h2>
               <div class="skills-grid">
                   <router-link to="/listening" class="skill-card listening">
                       <h3>Listening</h3>
                       <p>Luyện nghe đa dạng giọng đọc và chủ đề.</p>
                   </router-link>
                   <router-link to="/reading" class="skill-card reading">
                       <h3>Reading</h3>
                       <p>Nâng cao tốc độ và khả năng đọc hiểu học thuật.</p>
                   </router-link>
                   <router-link to="/writing" class="skill-card writing">
                       <h3>Writing</h3>
                       <p>Thực hành viết Task 1 & Task 2 chuẩn format.</p>
                   </router-link>
                    <router-link to="/speaking" class="skill-card speaking">
                       <h3>Speaking</h3>
                       <p>Cải thiện phát âm, từ vựng và sự lưu loát.</p>
                   </router-link>
               </div>
           </div>
       </section>
  
      <!-- Thêm các section khác nếu muốn: Testimonials, Blog Preview, ... -->
  
    </div>
  </template>
  
  <script setup>
  // Có thể thêm logic nếu cần (ví dụ: fetch dữ liệu động)
  </script>
  
  <style scoped>
  :root {
    --primary-yellow: #FFC107;
    --dark-yellow: #FFA000;
    --light-yellow: #FFF8E1;
    --text-dark: #333;
    --text-light: #555;
    --white: #fff;
    --section-bg: #f9f9f9; /* Màu nền cho các section */
  }
  
  .home-page {
    /* Không cần style gì đặc biệt ở đây, các section sẽ tự định dạng */
  }
  
  /* --- Hero Section --- */
  .hero-section {
    background: linear-gradient(rgba(255, 193, 7, 0.8), rgba(255, 160, 0, 0.7));
    background-size: cover;
    background-position: center;
    color: var(--white);
    text-align: center;
    padding: 100px 20px;
    text-shadow: 1px 1px 3px rgba(0,0,0,0.3);
  }
  
  .hero-content h1 {
    font-size: 2.8rem;
    margin-bottom: 15px;
    font-weight: 700;
  }
  
  .hero-content p {
    font-size: 1.2rem;
    margin-bottom: 30px;
    max-width: 600px;
    margin-left: auto;
    margin-right: auto;
    opacity: 0.9;
  }
  
  .btn-cta {
    background-color: var(--white);
    color: var(--dark-yellow);
    padding: 12px 30px;
    border: none;
    border-radius: 25px;
    font-size: 1.1rem;
    font-weight: 600;
    text-decoration: none;
    transition: background-color 0.3s ease, color 0.3s ease, transform 0.2s ease;
  }
  
  .btn-cta:hover {
    background-color: var(--light-yellow);
    color: var(--dark-yellow);
    transform: translateY(-2px);
     box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  }
  
  /* --- Features Section --- */
  .features-section, .skills-overview {
    padding: 60px 0;
  }
  .features-section {
      background-color: var(--section-bg); /* Nền xám nhạt */
  }
  .skills-overview {
       background-color: var(--white);
  }
  
  .container {
    max-width: 1100px;
    margin: 0 auto;
    padding: 0 20px;
  }
  
  h2 {
    text-align: center;
    font-size: 2.2rem;
    color: var(--text-dark);
    margin-bottom: 40px;
    font-weight: 600;
  }
  
  .features-grid, .skills-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); /* Responsive grid */
    gap: 30px;
  }
  
  .feature-item {
    background-color: var(--white);
    padding: 30px 25px;
    border-radius: 8px;
    text-align: center;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
    transition: transform 0.3s ease, box-shadow 0.3s ease;
  }
  .feature-item:hover {
      transform: translateY(-5px);
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.12);
  }
  
  .feature-icon {
    font-size: 2.5rem; /* Điều chỉnh kích thước icon */
    color: var(--primary-yellow);
    display: block;
    margin-bottom: 15px;
  }
  
  .feature-item h3 {
    font-size: 1.3rem;
    color: var(--text-dark);
    margin-bottom: 10px;
  }
  
  .feature-item p {
    color: var(--text-light);
    font-size: 0.95rem;
    line-height: 1.6;
  }
  
  /* --- Skills Overview Section --- */
   .skill-card {
      border-radius: 8px;
      padding: 40px 25px;
      text-align: center;
      color: var(--white);
      text-decoration: none;
      transition: transform 0.3s ease, box-shadow 0.3s ease;
      display: block; /* Để link chiếm toàn bộ card */
       box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
   }
    .skill-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
   }
  
   .skill-card h3 {
       font-size: 1.5rem;
       margin-bottom: 10px;
   }
   .skill-card p {
       opacity: 0.9;
   }
  
   /* Màu nền riêng cho từng skill card */
   .skill-card.listening { background-color: #29B6F6; } /* Blue */
   .skill-card.reading { background-color: #66BB6A; } /* Green */
   .skill-card.writing { background-color: #AB47BC; } /* Purple */
   .skill-card.speaking { background-color: #FFA726; } /* Orange */
  
  
  /* Responsive adjustments */
  @media (max-width: 768px) {
      .hero-content h1 {
          font-size: 2.2rem;
      }
       .hero-content p {
          font-size: 1rem;
      }
       h2 {
          font-size: 1.8rem;
       }
      .features-grid, .skills-grid {
           grid-template-columns: 1fr; /* 1 cột trên mobile */
      }
  }
  </style>