<!-- <template>
  <div id="app">
    <AuthPage />
  </div>
</template>

<script setup>
import AuthPage from './components/AuthPage.vue';

</script>

<style>
body {
  margin: 0; /* Loại bỏ margin mặc định của body */
  font-family: Avenir, Helvetica, Arial, sans-serif; /* Font chữ mặc định */
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50; /* Màu chữ mặc định */
}

#app {
  min-height: 100vh; /* Đảm bảo #app chiếm ít nhất toàn bộ chiều cao màn hình */
  display: flex; /* Sử dụng flex để AuthPage có thể tự căn giữa (nếu cần) */
  flex-direction: column; /* Sắp xếp các phần tử con theo chiều dọc */
}

/* Bạn có thể thêm các style global khác ở đây */
</style> -->

<template>
  <div id="app">
    <TheHeader /> 
    <main>
      <router-view /> 
    </main>
    <TheFooter />
  </div>
</template>

<script setup>
// Import Header và Footer
import TheHeader from './components/TheHeader.vue';
import TheFooter from './components/TheFooter.vue';
</script>

<style>
/* Global Styles (giữ nguyên hoặc tùy chỉnh) */
body {
  margin: 0;
  font-family: 'Arial', sans-serif; /* Có thể chọn font đẹp hơn */
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #333;
  background-color: #fdfdfd; /* Màu nền chung nhẹ nhàng */
}

#app {
  display: flex;
  flex-direction: column;
  min-height: 100vh; /* Đảm bảo app chiếm toàn bộ chiều cao */
}

main {
  flex-grow: 1; /* Làm cho phần main chiếm hết không gian còn lại giữa header và footer */
  /* Không cần padding ở đây nếu các view/component con tự quản lý padding */
}

/* Có thể thêm các Global style khác ở đây */
a {
   /* Reset link style cơ bản nếu cần */
}
</style>