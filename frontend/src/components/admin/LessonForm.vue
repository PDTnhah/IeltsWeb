<template>
  {/* Overlay cho modal */}
  <div class="lesson-form-modal-overlay" @click.self="$emit('close')"> {/* Click bên ngoài để đóng */}
    {/* Nội dung modal */}
    <div class="lesson-form-modal-content">
      <h2>{{ formTitle }}</h2>
      <form @submit.prevent="submitLesson">

        <div class="form-group">
          <label for="lesson-title">Tiêu đề bài học <span class="required">*</span></label>
          <input type="text" id="lesson-title" v-model="formData.title" required :disabled="saving || loadingDetails">
        </div>

        <div class="form-group">
          <label for="lesson-skill">Kỹ năng <span class="required">*</span></label>
          <select id="lesson-skill" v-model="formData.skill_id" required :disabled="saving || skillsLoading || loadingDetails">
            <option :value="null" disabled>-- {{ skillsLoading ? 'Đang tải kỹ năng...' : 'Chọn kỹ năng' }} --</option>
            <option v-for="skill in availableSkills" :key="skill.id" :value="skill.id">
              {{ skill.name }}
            </option>
          </select>
          <p v-if="skillsError" class="error-message small-error">{{ skillsError }}</p>
        </div>

        <div class="form-group">
          <label for="lesson-description">Mô tả ngắn</label>
          <textarea id="lesson-description" v-model="formData.description" rows="3" :disabled="saving || loadingDetails"></textarea>
        </div>

        <div class="form-group">
          <label for="lesson-content">Nội dung chi tiết <span class="required">*</span></label>
          <textarea id="lesson-content-editor" v-model="formData.content" rows="12" required :disabled="saving || loadingDetails" placeholder="..."></textarea>
        </div>

        <div class="form-group">
          <label for="lesson-video-url">URL Video (YouTube, Vimeo...)</label>
          <input type="url" id="lesson-video-url" v-model="formData.video_url" placeholder="https://www.youtube.com/watch?v=..." :disabled="saving || loadingDetails">
        </div>

        <div class="form-group">
          <label for="lesson-is-published">Trạng thái</label>
          <select id="lesson-is-published" v-model="formData.is_published" :disabled="saving || loadingDetails">
            <option :value="false">Bản nháp (Draft)</option>
            <option :value="true">Xuất bản (Published)</option>
          </select>
        </div>
        <!--
        <div class="form-group" v-if="isEditing">
          <label>Quản lý Hình ảnh</label>
          <input type="file" @change="handleFileSelect" multiple accept="image/*" :disabled="uploadingImages">
          <button type="button" @click="uploadSelectedImages" :disabled="!selectedFiles.length || uploadingImages || saving">
            {{ uploadingImages ? 'Đang tải lên...' : 'Upload ảnh đã chọn' }}
          </button>
          <div v-if="uploadError" class="error-message small-error">{{ uploadError }}</div>
          <div class="image-preview-container" v-if="lessonImages.length > 0">
             <p>Ảnh hiện có:</p>
             <div v-for="img in lessonImages" :key="img.id" class="image-preview">
                <img :src="getImageUrl(img.imageUrl)" :alt="img.imageUrl">
                <button type="button" @click="deleteImage(img.id)" class="delete-image-btn">×</button>
             </div>
          </div>
        </div>
        -->

        <p v-if="saveError" class="error-message">{{ saveError }}</p>

        <div class="modal-actions">
          <button type="button" @click="$emit('close')" class="btn btn-secondary" :disabled="saving || loadingDetails">Hủy</button>
          <button type="submit" class="btn btn-primary" :disabled="saving || loadingDetails || skillsLoading">
            <span v-if="saving || loadingDetails" class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            {{ saving ? (isEditing ? 'Đang cập nhật...' : 'Đang tạo...') : (isEditing ? 'Cập nhật' : 'Tạo bài học') }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, watch, onMounted, computed, defineProps, defineEmits } from 'vue';
import axios from 'axios';
// **TODO**: Import và cấu hình Rich Text Editor nếu dùng
// import { QuillEditor } from '@vueup/vue-quill';
// import '@vueup/vue-quill/dist/vue-quill.snow.css';

// --- Props và Emits ---
const props = defineProps({
  lessonIdToEdit: {
    type: Number, // Hoặc String
    default: null
  }
});
const emit = defineEmits(['close', 'saved']);

// --- Trạng thái nội bộ ---
const isEditing = computed(() => props.lessonIdToEdit !== null);
const formTitle = computed(() => isEditing.value ? 'Chỉnh sửa bài học' : 'Tạo bài học mới');

const saving = ref(false); // Trạng thái đang lưu (submit)
const loadingDetails = ref(false); // Trạng thái đang tải chi tiết khi sửa
const saveError = ref('');

// --- Dữ liệu Form ---
const formData = reactive({
  id: null,
  title: '',
  description: '',
  content: '', // HTML từ RTE
  skill_id: null,
  is_published: false,
  video_url: '',
  // Thêm các trường khác nếu LessionDtos backend yêu cầu
});

// --- API và Keys ---
const API_BASE_URL = 'http://localhost:5206/api/v1';
const AUTH_TOKEN_KEY = 'adminUserToken'; // Token admin
// const API_IMAGE_BASE_URL = 'http://localhost:5206'; // Nếu cần base URL cho ảnh

// --- Dữ liệu Kỹ năng ---
const availableSkills = ref([]);
const skillsLoading = ref(false);
const skillsError = ref('');

// --- Fetch Skills ---
const fetchAvailableSkills = async () => {
  skillsLoading.value = true; skillsError.value = '';
  const token = localStorage.getItem(AUTH_TOKEN_KEY);
  try {
    const response = await axios.get(`${API_BASE_URL}/skills`, { // Gọi API lấy skills
      headers: token ? { Authorization: `Bearer ${token}` } : {} // Gửi token nếu có
    });
    availableSkills.value = response.data; // Lưu danh sách skills
  } catch (err) {
    console.error("Error fetching skills:", err.response?.data || err.message);
    skillsError.value = "Lỗi tải danh sách kỹ năng.";
  } finally {
    skillsLoading.value = false;
  }
};

// --- Fetch Lesson Details (Khi Edit) ---
const fetchLessonDetails = async (lessonId) => {
  if (!lessonId) return;
  loadingDetails.value = true; // Bắt đầu tải chi tiết
  saveError.value = ''; // Xóa lỗi cũ
  const token = localStorage.getItem(AUTH_TOKEN_KEY);
  try {
    const response = await axios.get(`${API_BASE_URL}/lessions/${lessonId}`, { // API chi tiết lesson
      headers: { Authorization: `Bearer ${token}` }
    });
    const lessonData = response.data;
    // Điền dữ liệu vào form
    formData.id = lessonData.id;
    formData.title = lessonData.title || '';
    formData.description = lessonData.description || '';
    formData.content = lessonData.content || '';
    formData.skill_id = lessonData.skill_id || null;
    formData.is_published = lessonData.is_published || false;
    formData.video_url = lessonData.video_url || '';
    // lessonImages.value = lessonData.LessionImages || []; // Nếu API trả về ảnh
  } catch (err) {
    console.error("Error fetching lesson details:", err.response?.data || err.message);
    saveError.value = "Không thể tải chi tiết bài học. " + (err.response?.data?.message || err.message);
    emit('close'); // Đóng modal nếu không tải được chi tiết
  } finally {
    loadingDetails.value = false; // Kết thúc tải chi tiết
  }
};

// --- Reset Form ---
const resetForm = () => {
    formData.id = null;
    formData.title = '';
    formData.description = '';
    formData.content = ''; // Reset RTE content nếu cần
    formData.skill_id = null;
    formData.is_published = false;
    formData.video_url = '';
    saveError.value = '';
    // uploadedImages.value = [];
    // selectedFiles.value = [];
};

// --- Watcher: Theo dõi thay đổi ID để fetch hoặc reset ---
watch(() => props.lessonIdToEdit, (newId) => {
  if (newId) {
    fetchLessonDetails(newId);
  } else {
    resetForm();
  }
}, { immediate: true }); // Chạy ngay khi component mount

// --- Submit Form (Create/Update) ---
const submitLesson = async () => {
  saving.value = true;
  saveError.value = '';
  const token = localStorage.getItem(AUTH_TOKEN_KEY);
  if (!token) { saveError.value = "Token không hợp lệ."; saving.value = false; return;}

  // Tạo payload dựa trên formData, đảm bảo khớp LessionDtos backend
  const payload = {
      title: formData.title,
      description: formData.description,
      content: formData.content, // Nội dung HTML từ RTE
      skill_id: formData.skill_id,
      is_published: formData.is_published,
      video_url: formData.video_url,
      // Thêm các trường khác từ LessionDtos nếu cần
  };

  try {
    let response;
    if (isEditing.value) {
      // --- UPDATE ---
      console.log(`Submitting PUT to ${API_BASE_URL}/lessions/${formData.id} with payload:`, payload);
      response = await axios.put(`${API_BASE_URL}/lessions/${formData.id}`, payload, {
        headers: { Authorization: `Bearer ${token}` }
      });
      alert('Cập nhật bài học thành công!');
    } else {
      // --- CREATE ---
      console.log(`Submitting POST to ${API_BASE_URL}/lessions with payload:`, payload);
      response = await axios.post(`${API_BASE_URL}/lessions`, payload, {
        headers: { Authorization: `Bearer ${token}` }
      });
      formData.id = response.data.id; // Lưu lại ID nếu muốn upload ảnh ngay
      alert('Tạo bài học thành công!');
    }

    console.log("API response:", response.data);
    emit('saved'); // Thông báo thành công

  } catch (err) {
    console.error("Error saving lesson:", err.response?.data || err.message);
    let errorMessage = "Lưu bài học thất bại. ";
    // Xử lý lỗi chi tiết hơn từ response nếu có
    if (err.response?.data) {
        if (Array.isArray(err.response.data)) errorMessage += err.response.data.join(' ');
        else if (typeof err.response.data === 'string') errorMessage += err.response.data;
        else if (err.response.data.message) errorMessage += err.response.data.message;
        else if (err.response.data.title && typeof err.response.data.errors === 'object') {
            errorMessage += err.response.data.title + " ";
            const errors = err.response.data.errors;
            for (const key in errors) errorMessage += `${key}: ${errors[key].join(', ')} `;
        }
    } else errorMessage += err.message;
    saveError.value = errorMessage.trim();
  } finally {
    saving.value = false;
  }
};

// --- Lifecycle Hook ---
onMounted(() => {
  fetchAvailableSkills(); // Tải danh sách kỹ năng khi component sẵn sàng
});

// --- Logic Upload Ảnh (Ví dụ cơ bản, cần hoàn thiện) ---
// const selectedFiles = ref([]);
// const uploadingImages = ref(false);
// const uploadError = ref('');
// const lessonImages = ref([]); // Ảnh hiện có của bài học đang sửa

// const handleFileSelect = (event) => {
//   selectedFiles.value = Array.from(event.target.files);
//   uploadError.value = ''; // Reset lỗi khi chọn file mới
// };

// const uploadSelectedImages = async () => {
//   if (selectedFiles.value.length === 0 || !formData.id) return;
//   uploadingImages.value = true;
//   uploadError.value = '';
//   const token = localStorage.getItem(AUTH_TOKEN_KEY);
//   const imageFormData = new FormData();
//   selectedFiles.value.forEach(file => {
//     imageFormData.append('files', file);
//   });

//   try {
//     const response = await axios.post(`${API_BASE_URL}/lessions/uploads/${formData.id}`, imageFormData, {
//       headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'multipart/form-data' }
//     });
//     console.log("Images uploaded:", response.data);
//     lessonImages.value.push(...response.data); // Thêm ảnh mới vào danh sách hiển thị
//     selectedFiles.value = []; // Xóa file đã chọn trong input (cần reset input)
//     // event.target.value = null; // Reset input file
//     alert('Upload ảnh thành công!');
//   } catch (err) {
//     console.error("Error uploading images:", err.response?.data || err.message);
//     uploadError.value = "Lỗi upload ảnh: " + (err.response?.data?.message || err.message);
//   } finally {
//     uploadingImages.value = false;
//   }
// };

// const deleteImage = async (imageId) => {
//    if (!confirm('Bạn có chắc muốn xóa ảnh này?')) return;
//    const token = localStorage.getItem(AUTH_TOKEN_KEY);
//    // **Backend cần API DELETE /api/v1/lession-images/{imageId}**
//    try {
//       await axios.delete(`${API_BASE_URL}/lession-images/${imageId}`, { headers: { Authorization: `Bearer ${token}` }});
//       lessonImages.value = lessonImages.value.filter(img => img.id !== imageId);
//       alert('Xóa ảnh thành công.');
//    } catch (err) {
//        console.error("Error deleting image:", err.response?.data || err.message);
//        alert("Lỗi xóa ảnh.");
//    }
// };

// const getImageUrl = (imageName) => {
//   // Trả về URL đầy đủ để hiển thị ảnh, ví dụ:
//   // return `${API_IMAGE_BASE_URL}/Uploads/${imageName}`;
//   // Hoặc nếu bạn cấu hình static files trong ASP.NET Core để serve thư mục Uploads:
//   return `/Uploads/${imageName}`;
// };

</script>

<style scoped>
/* Style cho Modal và Form (giữ nguyên hoặc tùy chỉnh) */
.lesson-form-modal-overlay {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background-color: rgba(0,0,0,0.6); display: flex;
  justify-content: center; align-items: center; z-index: 1050;
  padding: 20px; box-sizing: border-box;
}
.lesson-form-modal-content {
  background-color: white; padding: 25px 30px; border-radius: 8px;
  width: 100%; max-width: 800px; max-height: 90vh; overflow-y: auto;
  box-shadow: 0 5px 15px rgba(0,0,0,0.3);
}
.lesson-form-modal-content h2 { text-align: center; margin-top: 0; margin-bottom: 25px; color: #333; font-size: 1.5rem; }
.form-group { margin-bottom: 18px; }
label { display: block; margin-bottom: 6px; font-weight: 600; color: #444; font-size: 0.9rem; }
input[type="text"], input[type="url"], select, textarea {
  width: 100%; padding: 10px 12px; border: 1px solid #ced4da;
  border-radius: 4px; box-sizing: border-box; font-size: 0.95rem;
  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}
input:focus, select:focus, textarea:focus { border-color: #80bdff; outline: 0; box-shadow: 0 0 0 0.2rem rgba(0,123,255,.25); }
textarea { min-height: 120px; resize: vertical; }
#lesson-content-editor { min-height: 250px; /* Tăng chiều cao cho RTE */ }
.modal-actions { margin-top: 30px; padding-top: 20px; border-top: 1px solid #e9ecef; display: flex; justify-content: flex-end; gap: 10px; }
.btn { padding: 10px 18px; font-size: 0.95rem; border-radius: 4px; cursor: pointer; border: none; font-weight: 500; display: inline-flex; align-items: center; justify-content: center; }
.btn-secondary { background-color: #6c757d; color: white; }
.btn-secondary:hover { background-color: #5a6268; }
.btn-primary { background-color: #007bff; color: white; }
.btn-primary:hover { background-color: #0069d9; }
.btn:disabled { opacity: 0.65; cursor: not-allowed; }
.error-message { color: #dc3545; font-size: 0.875rem; margin-top: 10px; }
.small-error { font-size: 0.8em; margin-top: 4px;}
.required { color: #dc3545; margin-left: 2px; }
.spinner-border-sm { width: 1rem; height: 1rem; border-width: .2em; margin-right: .5em;} /* Style cho spinner loading */
/* Image preview styles */
.image-preview-container { display: flex; flex-wrap: wrap; gap: 10px; margin-top: 10px; border-top: 1px solid #eee; padding-top: 10px; }
.image-preview { position: relative; }
.image-preview img { max-width: 100px; max-height: 100px; border: 1px solid #ddd; border-radius: 4px; display: block; }
.delete-image-btn { position: absolute; top: -5px; right: -5px; background: red; color: white; border: none; border-radius: 50%; width: 20px; height: 20px; font-size: 12px; line-height: 18px; text-align: center; cursor: pointer; padding: 0;}
</style>