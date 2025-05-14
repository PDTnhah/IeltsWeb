<template>
    <div class="skill-page container">
      <h1 class="page-title">Luyện Đọc - Reading</h1>
      <p class="page-description">Nâng cao tốc độ đọc hiểu, phân tích các loại văn bản học thuật và làm quen với các dạng câu hỏi thường gặp trong IELTS Reading.</p>
  
      <div class="lesson-list">
        <h2>Danh sách bài học</h2>
        <div v-if="lessons.length > 0" class="lesson-grid">
          <div v-for="lesson in lessons" :key="lesson.slug" class="lesson-card">
            <div class="card-content">
               <span class="lesson-type-badge">{{ lesson.type }}</span> <!-- Ví dụ: Skimming, Matching Headings -->
               <h3>{{ lesson.title }}</h3>
               <p>{{ lesson.description }}</p>
               <router-link :to="`/lessons/reading/${lesson.slug}`" class="btn btn-primary btn-small">Bắt đầu học</router-link>
            </div>
          </div>
        </div>
        <p v-else>Chưa có bài học nào cho kỹ năng Reading.</p>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  // import { RouterLink } from 'vue-router'; // Không cần import với <script setup>
  
  // --- Dữ liệu Mock Reading (Sẽ thay bằng API call) ---
  const lessons = ref([
    {
      slug: 'skimming-scanning',
      title: 'Kỹ năng Skimming và Scanning',
      description: 'Nắm vững hai kỹ thuật đọc nhanh cốt lõi để xử lý hiệu quả các bài đọc dài.',
      type: 'Core Skills'
    },
    {
      slug: 'true-false-not-given-basics',
      title: 'Dạng bài True/False/Not Given Cơ bản',
      description: 'Hiểu rõ sự khác biệt và cách xác định đáp án chính xác cho dạng bài phổ biến này.',
      type: 'Question Type'
    },
    {
      slug: 'matching-headings-strategy',
      title: 'Chiến lược làm bài Matching Headings',
      description: 'Học cách xác định ý chính của từng đoạn văn và nối với tiêu đề phù hợp.',
      type: 'Question Type'
     },
     {
      slug: 'summary-completion-tips',
      title: 'Mẹo làm bài Summary Completion',
      description: 'Luyện tập điền từ vào bản tóm tắt dựa trên thông tin trong bài đọc.',
      type: 'Question Type'
    }
    // Thêm các bài học Reading khác...
  ]);
  // --- Kết thúc dữ liệu Mock ---
  
  // --- Logic gọi API sẽ ở đây ---
  // onMounted(async () => {
  //   try {
  //     const response = await axios.get('/api/lessons/reading');
  //     lessons.value = response.data;
  //   } catch (error) {
  //     console.error("Không thể tải danh sách bài học Reading:", error);
  //   }
  // });
  </script>
  
  <style scoped>
  /* Styles giống hệt ListeningPage.vue - Đảm bảo các biến CSS được định nghĩa */
  :root { --primary-yellow: #FFC107; --dark-yellow: #FFA000; --text-dark: #333; --text-light: #555; --white: #fff; --light-yellow: #FFF8E1; --border-color: #eee; }
  
  .container { max-width: 1100px; margin: 30px auto; padding: 0 20px; }
  .page-title { color: var(--dark-yellow); margin-bottom: 10px; font-size: 2.2rem; }
  .page-description { color: var(--text-light); margin-bottom: 40px; font-size: 1.1rem; }
  
  .lesson-list h2 { font-size: 1.8rem; margin-bottom: 25px; color: var(--text-dark); border-bottom: 2px solid var(--primary-yellow); padding-bottom: 10px; display: inline-block;}
  
  .lesson-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 25px;
  }
  
  .lesson-card {
    background-color: var(--white);
    border-radius: 8px;
    box-shadow: 0 3px 10px rgba(0,0,0,0.07);
    overflow: hidden;
    transition: transform 0.3s ease, box-shadow 0.3s ease;
    display: flex;
    flex-direction: column;
  }
  .lesson-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 6px 15px rgba(0,0,0,0.1);
  }
  
  .card-content {
    padding: 20px 25px;
    display: flex;
    flex-direction: column;
    flex-grow: 1;
  }
  
  .lesson-card h3 {
    font-size: 1.3rem;
    color: var(--text-dark);
    margin-bottom: 10px;
    margin-top: 8px;
  }
  
  .lesson-card p {
    color: var(--text-light);
    font-size: 0.95rem;
    line-height: 1.6;
    margin-bottom: 20px;
    flex-grow: 1;
  }
  
  .lesson-type-badge {
      background-color: var(--light-yellow);
      color: var(--dark-yellow);
      padding: 4px 10px;
      border-radius: 15px;
      font-size: 0.8rem;
      font-weight: 600;
      display: inline-block;
      align-self: flex-start;
  }
  
  .btn-small {
    padding: 8px 18px;
    font-size: 0.9rem;
    align-self: flex-start;
  }
  .btn-primary { background-color: var(--primary-yellow); color: var(--text-dark); border: none; border-radius: 4px; cursor: pointer; text-decoration: none; display: inline-block; transition: background-color 0.3s; font-weight: 500;}
  .btn-primary:hover { background-color: var(--dark-yellow); }
  
  @media (max-width: 768px) {
      .lesson-grid { grid-template-columns: 1fr; }
      .page-title { font-size: 1.8rem; }
      .page-description { font-size: 1rem; }
  }
  </style>