<template>
    <div class="exam-view container">
       <div v-if="exam">
          <h1 class="exam-title">Bài Thi: {{ exam.title }}</h1>
          <div class="exam-details-box">
              <h2>Thông tin bài thi</h2>
              <p><strong>Loại bài thi:</strong> {{ exam.type }}</p>
              <p><strong>Thời gian làm bài:</strong> {{ exam.duration }} phút</p>
              <p><strong>Số lượng câu hỏi/phần:</strong> {{ exam.questionCount }}</p>
              <p><strong>Mô tả:</strong> {{ exam.description }}</p>
  
               <h3>Hướng dẫn làm bài:</h3>
               <ul class="instructions">
                   <li>Đảm bảo kết nối mạng ổn định trong suốt quá trình làm bài.</li>
                   <li>Chuẩn bị tai nghe (cho phần Listening) và giấy nháp nếu cần.</li>
                   <li>Bài thi sẽ tự động nộp khi hết giờ.</li>
                   <li>Bạn không thể quay lại phần thi trước đó sau khi đã chuyển sang phần mới (đối với bài Full Test).</li>
               </ul>
  
              <button class="btn btn-start-exam" @click="startExam">Bắt đầu làm bài</button>
              <p v-if="startError" class="error-message">{{ startError }}</p>
          </div>
       </div>
        <div v-else-if="loading">
          <p>Đang tải thông tin bài thi...</p>
      </div>
       <div v-else>
         <p>Không tìm thấy bài thi.</p>
       </div>
  
       <!-- Giao diện làm bài thi thực tế sẽ cần component riêng -->
       <!-- Ví dụ: <ExamInterface v-if="isExamStarted" :examData="exam" @submit="handleExamSubmit" /> -->
  
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import { useRoute /*, useRouter*/ } from 'vue-router';
  
  const route = useRoute();
//   const router = useRouter();
  const exam = ref(null);
  const loading = ref(true);
  const startError = ref('');
  // const isExamStarted = ref(false); // Trạng thái bắt đầu thi
  
  // --- Dữ liệu Mock chi tiết (Sẽ thay bằng API call) ---
  const allMockExams = {
      'full-practice-test-1': { slug: 'full-practice-test-1', title: 'Bài Thi Thử Đầy Đủ số 1', description: 'Bài thi tổng hợp 4 kỹ năng theo format mới nhất.', type: 'Full Test', duration: 165, questionCount: '4 Kỹ năng' },
      'academic-reading-test-set-a': { slug: 'academic-reading-test-set-a', title: 'Bộ đề Reading Học thuật - A', description: 'Tập trung luyện tập kỹ năng Reading với 3 bài đọc học thuật.', type: 'Reading Test', duration: 60, questionCount: '40 Câu' },
       'general-writing-test-set-b': { slug: 'general-writing-test-set-b', title: 'Bộ đề Writing Tổng quát - B', description: 'Luyện tập viết thư (Task 1) và bài luận ngắn (Task 2) cho General Training.', type: 'Writing Test (GT)', duration: 60, questionCount: '2 Task' },
       // Thêm mock data khác
  };
  // --- Kết thúc dữ liệu Mock ---
  
  const fetchExamDetails = (slug) => {
    loading.value = true;
    exam.value = null;
    console.log(`Đang tìm chi tiết bài thi: slug=${slug}`);
  
    // --- Logic gọi API sẽ ở đây ---
    // setTimeout(async () => {
    //   try {
    //      // const response = await axios.get(`/api/exams/${slug}`);
           // exam.value = response.data;
  
           // --- Sử dụng Mock Data ---
           if (allMockExams[slug]) {
               exam.value = allMockExams[slug];
           } else {
               console.error("Không tìm thấy bài thi mock!");
           }
           // --- Kết thúc Mock Data ---
    //   } catch (error) {
    //      console.error("Không thể tải chi tiết bài thi:", error);
    //   } finally {
    //       loading.value = false;
    //   }
    // }, 300);
  
     // --- Sử dụng Mock Data trực tiếp ---
      if (allMockExams[slug]) {
         exam.value = allMockExams[slug];
      } else {
          console.error("Không tìm thấy bài thi mock!");
      }
      loading.value = false;
      // --- Kết thúc Mock Data ---
  };
  
  const startExam = () => {
    // --- Logic kiểm tra điều kiện bắt đầu thi (ví dụ: đã đăng nhập, chưa làm bài này...) ---
    const isLoggedIn = !!localStorage.getItem('userToken'); // Kiểm tra đăng nhập đơn giản
    if (!isLoggedIn) {
        startError.value = "Bạn cần đăng nhập để bắt đầu làm bài.";
        // Có thể chuyển hướng tới trang đăng nhập
        // setTimeout(() => router.push('/login'), 2000);
        return;
    }
  
    startError.value = ''; // Xóa lỗi cũ
    console.log('Bắt đầu bài thi:', exam.value.slug);
    // isExamStarted.value = true; // Chuyển sang giao diện làm bài
    // Hoặc chuyển hướng sang một route làm bài riêng
    // router.push(`/exams/${exam.value.slug}/attempt`);
     alert('Chức năng làm bài thi đang được phát triển!'); // Thông báo tạm thời
  };
  
  
  onMounted(() => {
    fetchExamDetails(route.params.examSlug);
  });
  
  </script>
  
  <style scoped>
  :root { /* ... Biến màu ... */ }
  .container { max-width: 800px; margin: 30px auto; padding: 0 20px; }
  
  .exam-title {
      color: var(--dark-yellow);
      margin-bottom: 30px;
      font-size: 2.4rem;
      text-align: center;
  }
  
  .exam-details-box {
      background-color: var(--white);
      padding: 30px 40px;
      border-radius: 8px;
      box-shadow: 0 4px 15px rgba(0,0,0,0.08);
      line-height: 1.7;
  }
  .exam-details-box h2 {
      font-size: 1.6rem;
      color: var(--dark-yellow);
      margin-bottom: 20px;
      padding-bottom: 10px;
      border-bottom: 1px solid var(--border-color);
  }
  .exam-details-box p {
      margin-bottom: 1em;
      color: var(--text-dark);
  }
  .exam-details-box p strong {
      min-width: 150px; /* Căn chỉnh lề cho đẹp */
      display: inline-block;
      color: var(--text-light);
      font-weight: 500;
  }
  
  .instructions {
      margin-top: 20px;
      margin-bottom: 30px;
      padding-left: 20px; /* Thụt lề cho list */
      font-size: 0.95rem;
      color: var(--text-light);
  }
  .instructions li {
      margin-bottom: 8px;
  }
  
  .btn-start-exam {
      display: block; /* Chiếm toàn bộ chiều rộng */
      width: 100%;
      padding: 15px 20px;
      font-size: 1.2rem;
      font-weight: 600;
      background-color: var(--primary-yellow);
      color: var(--text-dark);
      border: none;
      border-radius: 5px;
      cursor: pointer;
      transition: background-color 0.3s ease, box-shadow 0.3s ease;
  }
  .btn-start-exam:hover {
      background-color: var(--dark-yellow);
      box-shadow: 0 4px 10px rgba(0,0,0,0.1);
  }
  
  .error-message {
      color: #dc3545; /* Màu đỏ báo lỗi */
      font-size: 0.9rem;
      margin-top: 15px;
      text-align: center;
  }
  </style>