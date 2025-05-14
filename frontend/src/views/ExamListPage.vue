<template>
    <div class="exam-list-page container">
      <h1 class="page-title">Thi Thử IELTS</h1>
      <p class="page-description">Trải nghiệm các bài thi thử đầy đủ 4 kỹ năng với cấu trúc và thời gian giới hạn như thi thật để đánh giá năng lực của bạn.</p>
  
      <div class="exam-list">
        <h2>Danh sách bài thi</h2>
        <div v-if="exams.length > 0" class="exam-grid">
          <div v-for="exam in exams" :key="exam.slug" class="exam-card">
             <div class="card-header">
                 <span class="exam-type-badge">{{ exam.type }}</span> <!-- Ví dụ: Full Test, Listening Test -->
             </div>
             <div class="card-content">
               <h3>{{ exam.title }}</h3>
               <p>{{ exam.description }}</p>
               <div class="exam-meta">
                   <span><i class="icon-clock"></i> {{ exam.duration }} phút</span>
                   <span><i class="icon-question"></i> {{ exam.questionCount }} câu hỏi</span>
               </div>
               <router-link :to="`/exams/${exam.slug}`" class="btn btn-primary btn-small">Xem chi tiết</router-link>
               <!-- Hoặc <button @click="startExam(exam.slug)">Bắt đầu thi</button> -->
            </div>
          </div>
        </div>
        <p v-else>Chưa có bài thi nào.</p>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  
  // --- Dữ liệu Mock (Sẽ thay bằng API call) ---
  const exams = ref([
    {
      slug: 'full-practice-test-1',
      title: 'Bài Thi Thử Đầy Đủ số 1',
      description: 'Bài thi tổng hợp 4 kỹ năng theo format mới nhất.',
      type: 'Full Test',
      duration: 165, // Listening ~30 + 10, Reading 60, Writing 60, Speaking 11-14 -> làm tròn
      questionCount: 'Nhiều' // Listening 40, Reading 40, Writing 2, Speaking 3 parts
    },
    {
      slug: 'academic-reading-test-set-a',
      title: 'Bộ đề Reading Học thuật - A',
      description: 'Tập trung luyện tập kỹ năng Reading với 3 bài đọc học thuật.',
      type: 'Reading Test',
      duration: 60,
      questionCount: 40
    },
     {
      slug: 'general-writing-test-set-b',
      title: 'Bộ đề Writing Tổng quát - B',
      description: 'Luyện tập viết thư (Task 1) và bài luận ngắn (Task 2) cho General Training.',
      type: 'Writing Test (GT)',
      duration: 60,
      questionCount: 2
    },
    // Thêm các bài thi khác...
  ]);
  // --- Kết thúc dữ liệu Mock ---
  </script>
  
  <style scoped>
  /* Tái sử dụng nhiều style từ SkillPage */
  :root { /* ... Biến màu ... */ }
  .container { max-width: 1100px; margin: 30px auto; padding: 0 20px; }
  .page-title { color: var(--dark-yellow); margin-bottom: 10px; font-size: 2.2rem; }
  .page-description { color: var(--text-light); margin-bottom: 40px; font-size: 1.1rem; }
  .exam-list h2 { font-size: 1.8rem; margin-bottom: 25px; color: var(--text-dark); border-bottom: 2px solid var(--primary-yellow); padding-bottom: 10px; display: inline-block;}
  
  .exam-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); gap: 25px; }
  
  .exam-card {
      background-color: var(--white);
      border-radius: 8px;
      box-shadow: 0 3px 10px rgba(0,0,0,0.07);
      overflow: hidden;
      transition: transform 0.3s ease, box-shadow 0.3s ease;
      display: flex;
      flex-direction: column;
  }
  .exam-card:hover { transform: translateY(-5px); box-shadow: 0 6px 15px rgba(0,0,0,0.1); }
  
  /* Optional Header for badge */
  .card-header {
      background-color: var(--light-yellow); /* Hoặc màu khác */
      padding: 8px 20px;
      text-align: right;
  }
  .exam-type-badge {
      background-color: var(--primary-yellow);
      color: var(--text-dark);
      padding: 4px 12px;
      border-radius: 15px;
      font-size: 0.8rem;
      font-weight: 600;
      display: inline-block;
  }
  
  .card-content { padding: 20px 25px; display: flex; flex-direction: column; flex-grow: 1; }
  .exam-card h3 { font-size: 1.3rem; color: var(--text-dark); margin-bottom: 10px; }
  .exam-card p { color: var(--text-light); font-size: 0.95rem; line-height: 1.6; margin-bottom: 15px; flex-grow: 1; }
  
  .exam-meta {
      display: flex;
      justify-content: space-between; /* Hoặc dùng flex-start và gap */
      gap: 15px;
      font-size: 0.9rem;
      color: var(--text-light);
      margin-bottom: 20px;
      padding-top: 10px;
      border-top: 1px solid var(--border-color);
  }
  .exam-meta span { display: flex; align-items: center; gap: 5px; }
  .exam-meta i { /* Style cho icon nếu dùng font icon */ color: var(--dark-yellow); }
  
  .btn-small { padding: 8px 18px; font-size: 0.9rem; align-self: flex-start; }
  .btn-primary { /* ... styles ... */ }
  .btn-primary:hover { /* ... styles ... */ }
  
  @media (max-width: 768px) {
      .exam-grid { grid-template-columns: 1fr; }
  }
  </style>