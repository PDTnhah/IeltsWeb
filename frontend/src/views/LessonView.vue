<template>
    <div class="lesson-view-page container">
      <div v-if="loading" class="loading-state">Đang tải nội dung bài học...</div>
      <div v-else-if="error" class="error-state">
        <h2>Lỗi tải bài học</h2>
        <p>{{ error }}</p>
        <router-link to="/" class="btn btn-primary">Về trang chủ</router-link>
      </div>
      <div v-else-if="lesson" class="lesson-content-wrapper">
        <nav aria-label="breadcrumb" class="breadcrumbs">
          <ol>
            <li><router-link to="/">Trang chủ</router-link></li>
            <li><router-link :to="`/${skillNameFromRoute}`">{{ skillDisplayName }}</router-link></li>
            <li aria-current="page">{{ lesson.title }}</li>
          </ol>
        </nav>
  
        <h1>{{ lesson.title }}</h1>
        <p v-if="lesson.description" class="lesson-description">{{ lesson.description }}</p>
        <hr>
  
        {/* **TODO**: Hiển thị Video nếu có lesson.video_url */}
        <div v-if="lesson.video_url" class="video-player-wrapper">
          {/* Logic nhúng video YouTube/Vimeo */}
          <iframe
            width="100%"
            height="450"
            :src="embedVideoUrl(lesson.video_url)"
            frameborder="0"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowfullscreen
          ></iframe>
        </div>
  
        {/* Hiển thị nội dung HTML từ Rich Text Editor */}
        {/* **QUAN TRỌNG**: Đảm bảo nội dung này an toàn trước khi dùng v-html */}
        {/* Nếu nội dung được tạo bởi admin tin cậy thì có thể chấp nhận được */}
        <div class="lesson-main-content" v-html="lesson.content"></div>
  
        {/* **TODO**: Hiển thị hình ảnh của bài học (lesson.images) nếu có */}
        {/* <div v-if="lesson.images && lesson.images.length > 0" class="lesson-images">
          <h3>Hình ảnh kèm theo:</h3>
          <div v-for="image in lesson.images" :key="image.id" class="lesson-image-item">
            <img :src="`${API_IMAGE_BASE_URL}/${image.imageUrl}`" :alt="lesson.title">
          </div>
        </div> */}
  
        {/* **TODO**: Thêm các phần tương tác: Bài tập, Quiz... */}
        <div class="lesson-actions">
           <button @click="markAsComplete" v-if="!isCompleted" class="btn btn-success">Đánh dấu hoàn thành</button>
           <p v-if="isCompleted" class="completed-message">Bạn đã hoàn thành bài học này!</p>
           <router-link :to="`/${skillNameFromRoute}`" class="btn btn-secondary">Quay lại danh sách</router-link>
        </div>
  
      </div>
      <div v-else class="not-found">
        <h2>Bài học không tồn tại</h2>
        <router-link to="/" class="btn btn-primary">Về trang chủ</router-link>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted, computed } from 'vue';
  import { useRoute } from 'vue-router';
  import axios from 'axios';
  
  const route = useRoute();
//   const router = useRouter(); // Nếu cần điều hướng
  
  const lesson = ref(null);
  const loading = ref(true);
  const error = ref('');
  const isCompleted = ref(false); // **TODO**: Lấy trạng thái này từ API hoặc localStorage
  
  const API_BASE_URL = 'http://localhost:5206/api/v1';
  // const API_IMAGE_BASE_URL = 'http://localhost:5206'; // Base URL cho ảnh nếu cần
  
  const skillNameFromRoute = computed(() => route.params.skill);
  const skillDisplayName = computed(() => {
      const name = skillNameFromRoute.value;
      return name.charAt(0).toUpperCase() + name.slice(1);
  });
  const lessonId = computed(() => route.params.lessonSlug); // lessonSlug đang là ID
  
  const fetchLessonContent = async () => {
    loading.value = true;
    error.value = '';
    if (!lessonId.value) {
      error.value = "Không tìm thấy ID bài học.";
      loading.value = false;
      return;
    }
  
    try {
      // Gọi API GET /api/v1/lessions/{id}
      // API này cần trả về đầy đủ: id, title, description, content (HTML), video_url, images (nếu có)
      const response = await axios.get(`${API_BASE_URL}/lessions/${lessonId.value}`);
      lesson.value = response.data;
  
      // **TODO**: Kiểm tra xem bài học này đã hoàn thành chưa
      // checkCompletionStatus(lessonId.value);
  
    } catch (err) {
      console.error("Không thể tải nội dung bài học:", err.response?.data || err.message);
      if (err.response?.status === 404) {
          error.value = "Bài học không tồn tại hoặc đã bị xóa.";
      } else {
          error.value = "Lỗi tải nội dung bài học. Vui lòng thử lại.";
      }
      lesson.value = null; // Đảm bảo không hiển thị nội dung cũ nếu lỗi
    } finally {
      loading.value = false;
    }
  };
  
  // Hàm chuyển đổi URL video thành URL nhúng
  const embedVideoUrl = (url) => {
      if (!url) return '';
      try {
          const urlObj = new URL(url);
          if (urlObj.hostname.includes('youtube.com') || urlObj.hostname.includes('youtu.be')) {
              const videoId = urlObj.searchParams.get('v') || urlObj.pathname.split('/').pop();
              return `https://www.youtube.com/embed/${videoId}`;
          } else if (urlObj.hostname.includes('vimeo.com')) {
              const videoId = urlObj.pathname.split('/').pop();
              return `https://player.vimeo.com/video/${videoId}`;
          }
      } catch (e) {
          console.error("Invalid video URL:", e);
          return ''; // Trả về rỗng nếu URL không hợp lệ
      }
      return url; // Trả về URL gốc nếu không phải YouTube/Vimeo (có thể không hoạt động)
  };
  
  
  const markAsComplete = () => {
      // **TODO**: Gọi API để đánh dấu bài học này đã hoàn thành cho user hiện tại
      console.log(`Đánh dấu bài học ${lessonId.value} đã hoàn thành.`);
      isCompleted.value = true;
      // Lưu trạng thái vào localStorage hoặc gọi API
      // localStorage.setItem(`lesson_completed_${lessonId.value}`, 'true');
      alert('Chúc mừng bạn đã hoàn thành bài học!');
  };
  
  // const checkCompletionStatus = (id) => {
  //    // isCompleted.value = !!localStorage.getItem(`lesson_completed_${id}`);
  //    // Hoặc gọi API
  // };
  
  
  onMounted(() => {
    fetchLessonContent();
  });
  </script>
  
  <style scoped>
  .lesson-view-page { padding-top: 20px; padding-bottom: 50px; }
  .loading-state, .error-state, .not-found { text-align: center; margin-top: 50px; }
  .error-state h2, .not-found h2 { margin-bottom: 20px; color: #dc3545; }
  .error-state p { margin-bottom: 20px; }
  
  .breadcrumbs { margin-bottom: 25px; font-size: 0.9rem; }
  .breadcrumbs ol { list-style: none; padding: 0; margin: 0; display: flex; gap: 5px; }
  .breadcrumbs li::after { content: '/'; margin-left: 5px; color: #6c757d; }
  .breadcrumbs li:last-child::after { content: ''; }
  .breadcrumbs a { color: #007bff; text-decoration: none; }
  .breadcrumbs a:hover { text-decoration: underline; }
  .breadcrumbs li[aria-current="page"] { color: #495057; font-weight: 500; }
  
  
  .lesson-content-wrapper h1 {
    font-size: 2rem;
    color: #333;
    margin-bottom: 15px;
  }
  .lesson-description {
      font-size: 1.1rem;
      color: #555;
      margin-bottom: 20px;
      font-style: italic;
  }
  hr { margin: 25px 0; border: 0; border-top: 1px solid #eee; }
  
  .video-player-wrapper {
      margin-bottom: 30px;
      position: relative;
      padding-bottom: 56.25%; /* 16:9 Aspect Ratio */
      height: 0;
      overflow: hidden;
      background-color: #000; /* Màu nền khi video đang tải */
      border-radius: 8px;
  }
  .video-player-wrapper iframe {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      border: none;
  }
  
  .lesson-main-content {
    font-size: 1rem;
    line-height: 1.7;
    color: #333;
    /* Style cho nội dung từ Rich Text Editor */
  }
  .lesson-main-content :deep(h2) { font-size: 1.6rem; margin-top: 30px; margin-bottom: 15px; color: #2c3e50; }
  .lesson-main-content :deep(h3) { font-size: 1.4rem; margin-top: 25px; margin-bottom: 10px; color: #34495e; }
  .lesson-main-content :deep(p) { margin-bottom: 15px; }
  .lesson-main-content :deep(ul), .lesson-main-content :deep(ol) { margin-left: 20px; margin-bottom: 15px; }
  .lesson-main-content :deep(img) { max-width: 100%; height: auto; border-radius: 4px; margin: 10px 0; }
  .lesson-main-content :deep(a) { color: #007bff; text-decoration: underline; }
  .lesson-main-content :deep(blockquote) { border-left: 4px solid #eee; padding-left: 15px; margin-left: 0; font-style: italic; color: #555;}
  
  .lesson-images { margin-top: 30px; }
  .lesson-image-item img { max-width: 100%; border-radius: 4px; margin-bottom: 10px; }
  
  .lesson-actions { margin-top: 40px; padding-top: 20px; border-top: 1px solid #eee; display: flex; gap: 15px; align-items: center; }
  .btn { /* style chung cho nút */ }
  .btn-success { background-color: #28a745; color: white; }
  .btn-secondary { background-color: #6c757d; color: white; }
  .completed-message { color: #28a745; font-weight: bold; }
  
  .btn-primary { background-color: var(--primary-yellow, #FFC107); color: var(--text-dark, #333); /* ... */ }
  </style>