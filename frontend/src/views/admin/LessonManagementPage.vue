<template>
    <div class="lesson-management">
      <h1>Quản lý Bài học</h1>
      <div class="actions">
        <button @click="openAddLessonModal" class="btn btn-success">
          <i class="fas fa-plus"></i> Thêm bài học mới
        </button>
      </div>
  
      <div v-if="loading">Đang tải danh sách bài học...</div>
      <div v-else-if="error">{{ error }}</div>
      <table v-else class="lessons-table">
        <thead>
          <tr>
            <th>Tiêu đề</th>
            <th>Kỹ năng</th>
            <th>Độ khó</th>
            <th>Ngày tạo</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="lesson in lessons" :key="lesson.id">
            <td>{{ lesson.title }}</td>
            <td>{{ lesson.skill }}</td>
            <td>{{ lesson.difficultyLevel }}</td>
            <td>{{ formatDate(lesson.createdAt) }}</td>
            <td>
              <button @click="openEditLessonModal(lesson)" class="btn btn-sm btn-warning">
                <i class="fas fa-edit"></i> Sửa
              </button>
              <button @click="confirmDeleteLesson(lesson.id)" class="btn btn-sm btn-danger">
                <i class="fas fa-trash"></i> Xóa
              </button>
            </td>
          </tr>
          <tr v-if="lessons.length === 0">
            <td colspan="5" style="text-align: center;">Chưa có bài học nào.</td>
          </tr>
        </tbody>
      </table>
  
      <LessonFormModal
        v-if="showLessonModal"
        :lessonData="selectedLesson"
        @close="closeLessonModal"
        @saved="handleLessonSaved"
      />
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import LessonFormModal from '@/components/admin/LessonForm.vue'; // Tạo component này
  
  const loading = ref(true);
  const error = ref('');
  const lessons = ref([]);
  const showLessonModal = ref(false);
  const selectedLesson = ref(null); // null: add new, object: edit
  
  const API_BASE_URL = 'http://localhost:5206/api/v1';
  const AUTH_TOKEN_KEY = 'adminUserToken';
  
  // Hàm lấy danh sách bài học
  const fetchLessons = async () => {
    loading.value = true; error.value = '';
    const token = localStorage.getItem(AUTH_TOKEN_KEY);
    try {
      const response = await axios.get(`${API_BASE_URL}/admin/lessons`, {
          headers: { Authorization: `Bearer ${token}` }
      });
      lessons.value = response.data; 
    } catch (err) {
      console.error("Error fetching lessons:", err);
      error.value = "Không thể tải danh sách bài học.";
    } finally { loading.value = false; }
  };
  
  // Hàm mở modal thêm bài học
  const openAddLessonModal = () => {
    selectedLesson.value = null; 
    showLessonModal.value = true;
  };
  
  // Hàm mở modal sửa bài học
  const openEditLessonModal = (lesson) => {
    selectedLesson.value = { ...lesson }; // Tạo bản sao để edit
    showLessonModal.value = true;
  };
  
  // Hàm đóng modal
  const closeLessonModal = () => {
    showLessonModal.value = false;
    selectedLesson.value = null;
  };
  
  // Hàm xử lý sau khi lưu (thêm/sửa) thành công
  const handleLessonSaved = () => {
    closeLessonModal();
    fetchLessons(); // Tải lại danh sách
  };
  
  // Hàm xác nhận xóa
  const confirmDeleteLesson = async (lessonId) => {
    if (!confirm('Bạn có chắc chắn muốn xóa bài học này?')) return;
  
    const token = localStorage.getItem(AUTH_TOKEN_KEY);
    try {
      // **Backend Cần Endpoint Này**: DELETE /api/v1/admin/lessons/{lessonId}
      await axios.delete(`${API_BASE_URL}/admin/lessons/${lessonId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      alert('Xóa bài học thành công!');
      fetchLessons(); // Tải lại danh sách
    } catch (err) {
      console.error("Error deleting lesson:", err);
      alert('Xóa bài học thất bại.');
    }
  };
  
  // Hàm format ngày (ví dụ)
  const formatDate = (dateString) => {
    if (!dateString) return '';
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString('vi-VN', options);
  };
  
  onMounted(fetchLessons);
  </script>
  
  <style scoped>
  .lesson-management h1 { margin-bottom: 20px; }
  .actions { margin-bottom: 20px; }
  .lessons-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
    background-color: #fff;
    box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  }
  .lessons-table th, .lessons-table td {
    border: 1px solid #e0e0e0;
    padding: 12px 15px;
    text-align: left;
  }
  .lessons-table th {
    background-color: #f8f9fa;
    font-weight: 600;
    color: #333;
  }
  .lessons-table tbody tr:nth-child(even) { background-color: #fdfdfd; }
  .lessons-table tbody tr:hover { background-color: #f1f1f1; }
  .btn { padding: 5px 10px; font-size: 0.85rem; margin-right: 5px; border-radius: 3px; cursor: pointer; border: none; }
  .btn-sm { padding: 4px 8px; font-size: 0.8rem; }
  .btn-success { background-color: #28a745; color: white; }
  .btn-warning { background-color: #ffc107; color: #212529; }
  .btn-danger { background-color: #dc3545; color: white; }
  .btn i { margin-right: 4px;}
  </style>