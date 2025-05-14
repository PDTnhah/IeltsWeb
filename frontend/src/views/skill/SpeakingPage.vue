<template>
    <div class="skill-page container">
      <h1 class="page-title">Luyện Nói - Speaking</h1>
      <p class="page-description">Nâng cao sự tự tin và lưu loát khi giao tiếp, luyện tập trả lời các câu hỏi trong cả 3 phần thi Speaking, cải thiện phát âm và sử dụng từ vựng.</p>
  
      <div class="lesson-list">
        <h2>Danh sách bài học</h2>
        <div v-if="lessons.length > 0" class="lesson-grid">
          <div v-for="lesson in lessons" :key="lesson.slug" class="lesson-card">
            <div class="card-content">
               <span class="lesson-type-badge">{{ lesson.type }}</span> <!-- Ví dụ: Part 1, Part 2, Pronunciation -->
               <h3>{{ lesson.title }}</h3>
               <p>{{ lesson.description }}</p>
               <router-link :to="`/lessons/speaking/${lesson.slug}`" class="btn btn-primary btn-small">Bắt đầu học</router-link>
            </div>
          </div>
        </div>
        <p v-else>Chưa có bài học nào cho kỹ năng Speaking.</p>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  
  // --- Dữ liệu Mock Speaking (Sẽ thay bằng API call) ---
  const lessons = ref([
    {
      slug: 'part-1-common-topics',
      title: 'Các chủ đề thường gặp Part 1',
      description: 'Luyện tập trả lời các câu hỏi về bản thân, gia đình, quê hương, sở thích, công việc/học tập.',
      type: 'Part 1'
    },
    {
      slug: 'part-2-cue-card-practice',
      title: 'Thực hành nói theo Cue Card (Part 2)',
      description: 'Học cách chuẩn bị ý tưởng trong 1 phút và trình bày bài nói trong 1-2 phút theo yêu cầu.',
      type: 'Part 2'
    },
    {
      slug: 'part-3-discussion-strategies',
      title: 'Chiến lược trả lời câu hỏi thảo luận Part 3',
      description: 'Phát triển khả năng diễn đạt ý kiến, so sánh, dự đoán về các chủ đề trừu tượng hơn.',
      type: 'Part 3'
     },
     {
      slug: 'pronunciation-intonation-basics',
      title: 'Phát âm và Ngữ điệu cơ bản',
      description: 'Tập trung vào các âm khó, trọng âm từ, trọng âm câu và ngữ điệu tự nhiên.',
      type: 'Pronunciation'
    }
    // Thêm các bài học Speaking khác...
  ]);
  // --- Kết thúc dữ liệu Mock ---
  
  // --- Logic gọi API sẽ ở đây ---
  // onMounted(async () => {
  //   try {
  //     const response = await axios.get('/api/lessons/speaking');
  //     lessons.value = response.data;
  //   } catch (error) {
  //     console.error("Không thể tải danh sách bài học Speaking:", error);
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