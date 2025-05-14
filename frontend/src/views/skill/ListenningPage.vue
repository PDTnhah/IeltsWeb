<template>
  <div class="skill-page container">
    <h1 class="page-title">Luyện Nghe - Listening</h1>
    <p class="page-description">Nâng cao kỹ năng nghe hiểu qua các bài tập đa dạng, từ cơ bản đến nâng cao, bao gồm nhiều giọng đọc và tình huống khác nhau.</p>

    <div v-if="loading" class="loading-state">Đang tải bài học...</div>
    <div v-else-if="error" class="error-state">Lỗi: {{ error }}</div>
    <div class="lesson-list" v-else>
      <h2>Danh sách bài học</h2>
      <div v-if="lessons.length > 0" class="lesson-grid">
        {/* API của bạn trả về id, title, description, is_published, skill_id, video_url, content ... */}
        {/* LessionImage không có trong list, chỉ có trong chi tiết lesson hoặc API riêng */}
        <div v-for="lesson in lessons" :key="lesson.id" class="lesson-card">
          <div class="card-content">
             {/* **TODO**: Hiển thị "type" hoặc "difficulty" nếu có trong dữ liệu lesson từ API */}
             {/* <span class="lesson-type-badge">{{ lesson.type || 'Practice' }}</span> */}
             <span v-if="lesson.difficultyLevel" class="lesson-type-badge">{{ lesson.difficultyLevel }}</span>
             <h3>{{ lesson.title }}</h3>
             <p>{{ lesson.description || 'Không có mô tả.' }}</p>
             {/* Link đến bài học chi tiết, dùng lesson.id */}
             <router-link :to="{ name: 'Lesson', params: { skill: 'listening', lessonSlug: lesson.id.toString() } }" class="btn btn-primary btn-small">
               Bắt đầu học
             </router-link>
          </div>
        </div>
      </div>
      <p v-else>Chưa có bài học nào cho kỹ năng này được xuất bản.</p>
    </div>

    {/* Phân trang nếu API hỗ trợ và có nhiều bài */}
    <div class="pagination" v-if="totalPages > 1 && !loading && !error">
      <button @click="changePage(currentPage - 1)" :disabled="currentPage === 1">Trước</button>
      <span>Trang {{ currentPage }} / {{ totalPages }}</span>
      <button @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages">Sau</button>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
// import { RouterLink } from 'vue-router'; // Không cần

const lessons = ref([]);
const loading = ref(true);
const error = ref('');

// Phân trang cho user
const currentPage = ref(1);
const totalPages = ref(0);
const limitPerPage = ref(9); // Số bài học mỗi trang cho user

const API_BASE_URL = 'http://localhost:5206/api/v1';
const LISTENING_SKILL_ID = 1; // **QUAN TRỌNG**: Đặt ID chính xác cho kỹ năng Listening từ DB/API Skills của bạn

const fetchListeningLessons = async (page = 1) => {
loading.value = true;
error.value = '';
try {
  // Gọi API GET /api/v1/lessions và lọc theo skillId cho Listening
  // API cần trả về các bài đã is_published = true
  const response = await axios.get(`${API_BASE_URL}/lessions`, {
    params: {
      skillId: LISTENING_SKILL_ID,
      page: page,
      limit: limitPerPage.value,
      // **TODO**: Thêm param isPublished=true nếu API hỗ trợ,
      // nếu không, bạn cần lọc ở frontend (không khuyến khích nếu nhiều dữ liệu)
    }
  });
  // Lọc các bài đã publish ở frontend NẾU API chưa hỗ trợ lọc
  lessons.value = response.data.lessions.filter(lesson => lesson.is_published);
  totalPages.value = response.data.totalPages; // API đã tính totalPages dựa trên số lượng trước khi filter is_published ở frontend
                                            // Nếu filter is_published ở frontend, totalPages này có thể không chính xác nữa.
                                            // Tốt nhất là API backend hỗ trợ lọc is_published.
  currentPage.value = response.data.currentPage;

  if (lessons.value.length === 0 && response.data.lessions.length > 0) {
      // Có bài nhưng không bài nào published
  }

} catch (err) {
  console.error("Không thể tải danh sách bài học Listening:", err.response?.data || err.message);
  error.value = "Không thể tải bài học. Vui lòng thử lại sau.";
} finally {
  loading.value = false;
}
};

const changePage = (newPage) => {
if (newPage >= 1 && newPage <= totalPages.value) {
  fetchListeningLessons(newPage);
}
};

onMounted(() => {
fetchListeningLessons(currentPage.value);
});
</script>

<style scoped>
/* ... (Giữ nguyên style của ListeningPage và thêm/sửa nếu cần) ... */
.loading-state, .error-state { padding: 20px; text-align: center; color: #555; margin-top: 30px; }
.error-state { color: #e74c3c; }
.pagination { margin-top: 30px; display: flex; justify-content: center; align-items: center; gap: 10px; }
.pagination button { padding: 8px 15px; /* ... style nút ... */ }
</style>