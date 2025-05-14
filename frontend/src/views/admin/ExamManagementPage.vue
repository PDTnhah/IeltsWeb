<template>
    <div class="exam-management">
      <h1>Quản lý Bài thi</h1>
      <div class="actions">
        <button @click="openAddExamModal" class="btn btn-success">
          <i class="fas fa-plus"></i> Thêm bài thi mới
        </button>
      </div>
  
      <div v-if="loading" class="loading-state">Đang tải danh sách bài thi...</div>
      <div v-else-if="error" class="error-state">Lỗi: {{ error }}</div>
      <table v-else class="exams-table">
        <thead>
          <tr>
            <th>Tiêu đề</th>
            <th>Số phần/câu</th> 
            <th>Độ khó</th>
            <th>Trạng thái</th>
            <th>Ngày tạo</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="exam in exams" :key="exam.id">
            <td>{{ exam.title }}</td>
            <td>{{ exam.numberOfParts || 'N/A' }} phần</td> {/* Giả sử có trường này */}
            <td>{{ exam.difficultyLevel || 'N/A' }}</td>
            <td>
               <span :class="['status-badge', exam.isPublished ? 'published' : 'draft']">
                   {{ exam.isPublished ? 'Đã xuất bản' : 'Bản nháp' }}
               </span>
            </td>
            <td>{{ formatDate(exam.createdAt) }}</td>
            <td>
              <button @click="viewExamDetails(exam.id)" class="btn btn-sm btn-info" title="Xem chi tiết/Câu hỏi">
                  <i class="fas fa-eye"></i>
              </button>
              <button @click="openEditExamModal(exam)" class="btn btn-sm btn-warning" title="Sửa thông tin">
                <i class="fas fa-edit"></i>
              </button>
              <button @click="confirmDeleteExam(exam.id)" class="btn btn-sm btn-danger" title="Xóa bài thi">
                <i class="fas fa-trash"></i>
              </button>
               <button @click="togglePublishStatus(exam.id, exam.isPublished)" class="btn btn-sm btn-secondary" :title="exam.isPublished ? 'Hủy xuất bản' : 'Xuất bản'">
                  <i :class="['fas', exam.isPublished ? 'fa-eye-slash' : 'fa-eye']"></i>
              </button>
            </td>
          </tr>
          <tr v-if="exams.length === 0">
            <td colspan="6" style="text-align: center;">Chưa có bài thi nào.</td>
          </tr>
        </tbody>
      </table>
  
      <ExamFormModal
        v-if="showExamModal"
        :examData="selectedExam"
        @close="closeExamModal"
        @saved="handleExamSaved"
      />
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  // import ExamFormModal from '@/components/admin/ExamForm.vue'; // **TODO**: Tạo component này
  
  const loading = ref(true);
  const error = ref('');
  const exams = ref([]);
  const showExamModal = ref(false);
  const selectedExam = ref(null); // null: add new, object: edit
  
  const API_BASE_URL = 'http://localhost:5206/api/v1';
  const AUTH_TOKEN_KEY = 'adminUserToken';
  
  // Hàm lấy danh sách bài thi
  const fetchExams = async () => {
    loading.value = true; error.value = '';
    const token = localStorage.getItem(AUTH_TOKEN_KEY);
     if (!token) { error.value = "Token admin không hợp lệ."; loading.value = false; return; }
  
    try {
      // **Backend Cần Endpoint Này**: GET /api/v1/admin/exams
      // API nên trả về: id, title, difficultyLevel, isPublished, createdAt, numberOfParts (nếu có)
      const response = await axios.get(`${API_BASE_URL}/admin/exams`, {
          headers: { Authorization: `Bearer ${token}` }
      });
      exams.value = response.data; // Giả sử trả về mảng exam objects
    } catch (err) {
      console.error("Error fetching exams:", err.response || err);
      error.value = err.response?.data?.message || "Không thể tải danh sách bài thi.";
       if (err.response?.status === 401 || err.response?.status === 403) { error.value += " Vui lòng đăng nhập lại."; }
    } finally { loading.value = false; }
  };
  
  // Hàm format ngày
  const formatDate = (dateString) => {
     if (!dateString) return 'N/A';
      try {
          const options = { year: 'numeric', month: 'short', day: 'numeric' };
          return new Date(dateString).toLocaleDateString('vi-VN', options);
      } catch(e) { return dateString; }
  };
  
  // --- Modal Handling ---
  const openAddExamModal = () => {
    selectedExam.value = null;
    // showExamModal.value = true; // **TODO**: Bỏ comment khi có ExamFormModal
    alert('Chức năng thêm bài thi mới chưa được triển khai (cần ExamFormModal).');
  };
  
  const openEditExamModal = (exam) => {
    selectedExam.value = { ...exam };
    // showExamModal.value = true; // **TODO**: Bỏ comment khi có ExamFormModal
    alert(`Chức năng sửa bài thi "${exam.title}" chưa được triển khai (cần ExamFormModal).`);
  };
  
  const closeExamModal = () => {
    showExamModal.value = false;
    selectedExam.value = null;
  };
  
  const handleExamSaved = () => {
    closeExamModal();
    fetchExams(); // Tải lại danh sách
  };
  
  // --- Placeholder Actions ---
  const viewExamDetails = (examId) => {
      console.log(`Xem chi tiết bài thi ID: ${examId}`);
      alert(`Chức năng xem chi tiết/câu hỏi bài thi ID ${examId} chưa được triển khai.`);
      // **TODO**: Điều hướng đến trang quản lý câu hỏi của bài thi này
  };
  
  const confirmDeleteExam = async (examId) => {
    if (!confirm('Bạn có chắc chắn muốn xóa bài thi này và tất cả câu hỏi liên quan?')) return;
  
    const token = localStorage.getItem(AUTH_TOKEN_KEY);
    if (!token) { alert("Token không hợp lệ"); return; }
  
    try {
      console.log(`Xóa bài thi ID: ${examId}`);
      alert(`Chức năng xóa bài thi ID ${examId} chưa được triển khai.`);
      // **TODO**: Gọi API DELETE /admin/exams/{examId}
      // await axios.delete(`${API_BASE_URL}/admin/exams/${examId}`, { headers: { Authorization: `Bearer ${token}` } });
      // alert('Xóa bài thi thành công!');
      // fetchExams(); // Tải lại danh sách
    } catch (err) {
      console.error("Error deleting exam:", err);
      alert('Xóa bài thi thất bại.');
    }
  };
  
  const togglePublishStatus = (examId, isPublished) => {
      const action = isPublished ? 'Hủy xuất bản' : 'Xuất bản';
      console.log(`${action} bài thi ID: ${examId}`);
      alert(`Chức năng ${action} bài thi ID ${examId} chưa được triển khai.`);
       // **TODO**: Gọi API PUT /admin/exams/{examId}/publish hoặc tương tự
       // Cập nhật trạng thái trong danh sách sau khi thành công
  };
  
  
  onMounted(fetchExams);
  </script>
  
  <style scoped>
  .exam-management h1 { margin-bottom: 20px; color: #2c3e50;}
  .actions { margin-bottom: 20px; }
  .exams-table { /* Style tương tự lessons-table hoặc users-table */
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
    background-color: #fff;
    box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  }
  .exams-table th, .exams-table td {
    border: 1px solid #e0e0e0;
    padding: 10px 12px;
    text-align: left;
    vertical-align: middle;
  }
  .exams-table th {
    background-color: #f8f9fa;
    font-weight: 600;
    color: #333;
     font-size: 0.9rem;
  }
  .exams-table tbody tr:nth-child(even) { background-color: #fdfdfd; }
  .exams-table tbody tr:hover { background-color: #f1f1f1; }
  
  /* Style cho nút và значки */
  .btn { padding: 5px 10px; font-size: 0.85rem; margin-right: 5px; border-radius: 3px; cursor: pointer; border: none; }
  .btn-sm { padding: 3px 6px; font-size: 0.75rem; }
  .btn-success { background-color: #28a745; color: white; }
  .btn-info { background-color: #17a2b8; color: white; }
  .btn-warning { background-color: #ffc107; color: #212529; }
  .btn-danger { background-color: #dc3545; color: white; }
  .btn-secondary { background-color: #6c757d; color: white; }
  .btn i { /* Không cần margin */ }
  
  /* Status Badge */
  .status-badge {
    padding: 4px 9px;
    border-radius: 12px;
    font-size: 0.75rem;
    color: white;
    font-weight: 500;
    text-transform: uppercase;
  }
  .status-badge.published { background-color: #28a745; /* Xanh lá */ }
  .status-badge.draft { background-color: #6c757d; /* Xám */ }
  
  .loading-state, .error-state { padding: 20px; text-align: center; color: #555; }
  .error-state { color: #e74c3c; }
  </style>