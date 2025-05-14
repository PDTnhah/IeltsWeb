<template>
    <div class="user-management">
      <h1>Quản lý Người dùng</h1>
  
      <!--
      <div class="filters">
        <input type="text" placeholder="Tìm theo tên hoặc email..." v-model="searchTerm">
        <select v-model="filterRole">
          <option value="">Tất cả vai trò</option>
          <option value="User">User</option>
          <option value="Admin">Admin</option>
        </select>
      </div>
      -->
  
      <div v-if="loading" class="loading-state">Đang tải danh sách người dùng...</div>
      <div v-else-if="error" class="error-state">Lỗi: {{ error }}</div>
      <table v-else class="users-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Họ và Tên</th>
            <th>Email</th>
            <th>Vai trò</th>
            <th>Ngày tham gia</th>
            <th>Trạng thái</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {/* **TODO**: Lọc danh sách users dựa trên searchTerm và filterRole nếu có */}
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.id }}</td>
            <td>{{ user.name }}</td>
            <td>{{ user.email }}</td>
            <td>
              <span :class="['role-badge', user.role?.toLowerCase()]">{{ user.role || 'N/A' }}</span>
            </td>
            <td>{{ formatDate(user.createdAt) }}</td>
            <td>
              {/* **TODO**: Hiển thị trạng thái (Active/Banned) - Cần thêm trường này từ API */}
              <span class="status-active">Hoạt động</span>
            </td>
            <td>
              <button @click="viewUserProgress(user.id)" class="btn btn-sm btn-info" title="Xem tiến độ">
                <i class="fas fa-chart-bar"></i>
              </button>
              <button @click="editUserRole(user)" class="btn btn-sm btn-warning" title="Sửa vai trò">
                <i class="fas fa-user-shield"></i>
              </button>
               <button @click="toggleUserStatus(user.id, /* currentState */)" class="btn btn-sm btn-secondary" title="Khóa/Mở khóa">
                <i class="fas fa-user-lock"></i> 
              </button>
              <!--
              <button @click="confirmDeleteUser(user.id)" class="btn btn-sm btn-danger" title="Xóa người dùng">
                <i class="fas fa-user-times"></i>
              </button>
               -->
            </td>
          </tr>
          <tr v-if="users.length === 0">
            <td colspan="7" style="text-align: center;">Không tìm thấy người dùng nào.</td>
          </tr>
        </tbody>
      </table>
  
  
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  
  const loading = ref(true);
  const error = ref('');
  const users = ref([]);
  // const searchTerm = ref(''); // Cho chức năng lọc
  // const filterRole = ref(''); // Cho chức năng lọc
  
  const API_BASE_URL = 'http://localhost:5206/api/v1';
  const AUTH_TOKEN_KEY = 'adminUserToken'; // Key token admin
  
  // Hàm lấy danh sách người dùng
  const fetchUsers = async () => {
    loading.value = true; error.value = '';
    const token = localStorage.getItem(AUTH_TOKEN_KEY);
    if (!token) {
        error.value = "Không tìm thấy token xác thực admin.";
        loading.value = false;
        return;
    }
  
    try {
      // **Backend Cần Endpoint Này**: GET /api/v1/admin/users
      // API này nên trả về thông tin user bao gồm id, name, email, role.name, createdAt
      const response = await axios.get(`${API_BASE_URL}/admin/users`, {
          headers: { Authorization: `Bearer ${token}` }
      });
      // Giả sử API trả về mảng user objects, mỗi object có { id, name, email, role: 'Admin'/'User', createdAt }
      users.value = response.data.map(u => ({...u, role: u.role || 'User'})); // Đảm bảo có role
    } catch (err) {
      console.error("Error fetching users:", err.response || err);
      error.value = err.response?.data?.message || "Không thể tải danh sách người dùng.";
      if (err.response?.status === 401 || err.response?.status === 403) {
          error.value += " Vui lòng đăng nhập lại.";
      }
    } finally { loading.value = false; }
  };
  
  // Hàm format ngày (ví dụ)
  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
        const options = { year: 'numeric', month: 'short', day: 'numeric' };
        return new Date(dateString).toLocaleDateString('vi-VN', options);
    } catch (e) {
        return dateString; // Trả về chuỗi gốc nếu không parse được
    }
  };
  
  // --- Placeholder Functions for Actions ---
  const viewUserProgress = (userId) => {
    console.log(`Xem tiến độ cho user ID: ${userId}`);
    alert(`Chức năng xem tiến độ cho user ID ${userId} chưa được triển khai.`);
    // **TODO**: Điều hướng đến trang chi tiết tiến độ hoặc mở modal
    // Yêu cầu API backend phức tạp hơn để lấy dữ liệu tiến độ
  };
  
  const editUserRole = (user) => {
    console.log('Sửa vai trò cho user:', user);
    alert(`Chức năng sửa vai trò cho user ${user.name} chưa được triển khai.`);
    // **TODO**: Mở Modal/Form để cho phép chọn Role mới (User/Admin) và gọi API PUT /admin/users/{userId}/role
  };
  
  const toggleUserStatus = (userId, currentState) => {
    console.log(`Thay đổi trạng thái cho user ID: ${userId}`);
    const action = currentState === 'Active' ? 'Khóa' : 'Mở khóa';
    alert(`Chức năng ${action} user ID ${userId} chưa được triển khai.`);
    // **TODO**: Gọi API PUT /admin/users/{userId}/status để cập nhật trạng thái (Active/Banned)
    // Cần cập nhật trạng thái hiển thị sau khi thành công
  };
  
//   const confirmDeleteUser = (userId) => {
//       if (confirm(`Bạn có chắc chắn muốn XÓA vĩnh viễn người dùng ID: ${userId}? Hành động này không thể hoàn tác!`)) {
//           console.log(`Xóa user ID: ${userId}`);
//           alert(`Chức năng xóa user ID ${userId} chưa được triển khai.`);
//           // **TODO**: Gọi API DELETE /admin/users/{userId}
//           // Cập nhật danh sách sau khi xóa
//       }
//   };
  
  onMounted(fetchUsers);
  </script>
  
  <style scoped>
  .user-management h1 { margin-bottom: 20px; color: #2c3e50; }
  .filters { margin-bottom: 20px; display: flex; gap: 15px; }
  .filters input, .filters select { padding: 8px 12px; border: 1px solid #ccc; border-radius: 4px; }
  
  .users-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
    background-color: #fff;
    box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  }
  .users-table th, .users-table td {
    border: 1px solid #e0e0e0;
    padding: 10px 12px; /* Giảm padding một chút */
    text-align: left;
    vertical-align: middle; /* Căn giữa theo chiều dọc */
  }
  .users-table th {
    background-color: #f8f9fa;
    font-weight: 600;
    color: #333;
    font-size: 0.9rem; /* Giảm font size header */
  }
  .users-table tbody tr:nth-child(even) { background-color: #fdfdfd; }
  .users-table tbody tr:hover { background-color: #f1f1f1; }
  
  /* Style cho nút và значки */
  .btn { padding: 5px 10px; font-size: 0.85rem; margin-right: 5px; border-radius: 3px; cursor: pointer; border: none; }
  .btn-sm { padding: 3px 6px; font-size: 0.75rem; } /* Nhỏ hơn nữa */
  .btn-info { background-color: #17a2b8; color: white; }
  .btn-warning { background-color: #ffc107; color: #212529; }
  .btn-danger { background-color: #dc3545; color: white; }
  .btn-secondary { background-color: #6c757d; color: white; }
  .btn i { /* Không cần margin nếu chỉ có icon */ }
  
  .role-badge {
    padding: 3px 8px;
    border-radius: 10px;
    font-size: 0.8rem;
    color: white;
    text-transform: uppercase;
    font-weight: 600;
  }
  .role-badge.admin { background-color: #e74c3c; /* Màu đỏ cho Admin */ }
  .role-badge.user { background-color: #3498db; /* Màu xanh cho User */ }
  
  .status-active { color: #28a745; font-weight: 500; }
  .status-banned { color: #dc3545; font-weight: 500; } /* Ví dụ */
  
  .loading-state, .error-state { padding: 20px; text-align: center; color: #555; }
  .error-state { color: #e74c3c; }
  </style>