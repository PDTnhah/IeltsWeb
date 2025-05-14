import { createRouter, createWebHistory } from 'vue-router';
import HomePage from '../views/HomePage.vue';
import AuthPage from '../components/AuthPage.vue'; // Hoặc '../views/AuthPage.vue'

// Import các view mới
import ListeningPage from '../views/skill/ListenningPage.vue';
import ReadingPage from '../views/skill/ReadingPage.vue';
import WritingPage from '../views/skill/WritingPage.vue';
import SpeakingPage from '../views/skill/SpeakingPage.vue';
import LessonView from '../views/LessonView.vue';
import ExamListPage from '../views/ExamListPage.vue';
import ExamView from '../views/ExamView.vue';
import DashboardPage from '../views/DashboardPage.vue';

// --- Admin Components ---
import AdminLayout from '../views/admin/AdminLayout.vue'; // Layout chung
import AdminLoginPage from '../views/admin/AdminLoginPage.vue'; // Trang login admin
import AdminDashboard from '../views/admin/AdminDashboard.vue'; // Trang tổng quan admin
import LessonManagementPage from '../views/admin/LessonManagementPage.vue'; // Trang quản lý bài học
import UserManagementPage from '../views/admin/UserManagementPage.vue'; // Trang quản lý người dùng (Placeholder)
import ExamManagementPage from '../views/admin/ExamManagementPage.vue'; // Trang quản lý bài thi (Placeholder)

// Placeholder cho các trang chưa có component thật
const createPlaceholderComponent = (name) => ({
    name: `${name}Page`,
    template: `<div style="padding: 50px; text-align: center;"><h2>${name} Page</h2><p>Nội dung trang ${name} sẽ ở đây.</p></div>`
});

const routes = [
    { path: '/', name: 'Home', component: HomePage },
    { path: '/login', name: 'Auth', component: AuthPage, meta: { requiresGuest: true } }, // Chỉ cho khách
    { path: '/register', name: 'Register', component: AuthPage, meta: { requiresGuest: true } }, // Chỉ cho khách

    // Routes Kỹ năng
    { path: '/listening', name: 'Listening', component: ListeningPage, meta: { requiresAuth: true } }, // Cần đăng nhập
    { path: '/reading', name: 'Reading', component: ReadingPage, meta: { requiresAuth: true } }, // Cần đăng nhập
    { path: '/writing', name: 'Writing', component: WritingPage, meta: { requiresAuth: true } }, // Cần đăng nhập
    { path: '/speaking', name: 'Speaking', component: SpeakingPage, meta: { requiresAuth: true } }, // Cần đăng nhập

    // Route Chi tiết bài học (Dynamic)
    { path: '/lessons/:skill/:lessonSlug', name: 'Lesson', component: LessonView, props: true, meta: { requiresAuth: true } }, // props: true để truyền params làm props

    // Routes Thi thử
    { path: '/exams', name: 'ExamList', component: ExamListPage, meta: { requiresAuth: true } }, // Cần đăng nhập
    { path: '/exams/:examSlug', name: 'Exam', component: ExamView, props: true, meta: { requiresAuth: true } }, // Cần đăng nhập

    // Route Dashboard
    { path: '/dashboard', name: 'Dashboard', component: DashboardPage, meta: { requiresAuth: true } }, // Cần đăng nhập

    // == Admin Routes ==
    {
        path: '/admin/login',
        name: 'AdminLogin',
        component: AdminLoginPage,
        meta: { requiresGuest: true } // Chỉ cho phép truy cập khi chưa đăng nhập (admin hoặc user)
    },
    {
        path: '/admin',
        component: AdminLayout, // Sử dụng layout riêng cho admin
        meta: { requiresAuth: true, requiresAdmin: true }, // Yêu cầu đăng nhập VÀ là Admin
        children: [
            { path: '', redirect: { name: 'AdminDashboard' } }, // Mặc định vào dashboard
            { path: 'dashboard', name: 'AdminDashboard', component: AdminDashboard },
            { path: 'lessons', name: 'AdminLessonManagement', component: LessonManagementPage },
            { path: 'users', name: 'AdminUserManagement', component: UserManagementPage }, // Component cần tạo
            { path: 'exams', name: 'AdminExamManagement', component: ExamManagementPage }, // Component cần tạo
            // Thêm các route admin khác ở đây
        ]
    },

    // Routes khác (ví dụ)
    { path: '/profile', name: 'Profile', component: createPlaceholderComponent('User Profile'), meta: { requiresAuth: true } },
    { path: '/history', name: 'History', component: createPlaceholderComponent('Learning History'), meta: { requiresAuth: true } },
    { path: '/terms', name: 'Terms', component: createPlaceholderComponent('Terms') },
    { path: '/privacy', name: 'Privacy', component: createPlaceholderComponent('Privacy') },

    // Catch-all 404
    { path: '/:catchAll(.*)', name: 'NotFound', component: createPlaceholderComponent('404 Not Found') }
];

const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes,
    scrollBehavior(to, from, savedPosition) {
        if (savedPosition) return savedPosition;
        return { top: 0 };
    },
});

// --- Navigation Guards (Bước 10) ---
router.beforeEach((to, from, next) => {
    const userToken = localStorage.getItem('userToken');     // Token của User thường
    const adminToken = localStorage.getItem('adminUserToken'); // Token của Admin (mô phỏng)
    const adminRole = localStorage.getItem('adminRole');     // Role của Admin (phải là 'Admin')

    // Xác định trạng thái đăng nhập
    const isLoggedInAdmin = !!adminToken && adminRole === 'Admin';
    // User đăng nhập nếu có userToken VÀ không phải là admin đang đăng nhập
    const isLoggedInUser = !!userToken && !isLoggedInAdmin;

    // Lấy metadata của route đích
    const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
    const requiresGuest = to.matched.some(record => record.meta.requiresGuest);
    const requiresAdmin = to.matched.some(record => record.meta.requiresAdmin);
    const requiresUser = to.matched.some(record => record.meta.requiresUser); // **Quan trọng**

    console.log(`[Guard Final V3] Path: ${to.path}, UserLoggedIn: ${isLoggedInUser}, AdminLoggedIn: ${isLoggedInAdmin}`);
    console.log(`[Guard Final V3] Meta: Auth=${requiresAuth}, Guest=${requiresGuest}, Admin=${requiresAdmin}, User=${requiresUser}`);

    // 1. Xử lý các route chỉ dành cho Khách (Guest)
    if (requiresGuest) {
        if (isLoggedInAdmin) {
            console.log('[Guard V3] Guest route, but Admin is logged in. Redirecting to AdminDashboard.');
            next({ name: 'AdminDashboard' }); // Nếu Admin đã login, vào trang login/register -> về Admin Dashboard
            return; // Quan trọng: return để dừng xử lý guard
        }
        if (isLoggedInUser) {
            console.log('[Guard V3] Guest route, but User is logged in. Redirecting to User Dashboard.');
            next({ name: 'Dashboard' }); // Nếu User đã login, vào trang login/register -> về User Dashboard
            return; // Quan trọng: return
        }
        // Nếu chưa ai login, cho phép vào trang Guest
        console.log('[Guard V3] Guest access GRANTED.');
        next();
        return;
    }

    // 2. Xử lý các route yêu cầu quyền Admin
    if (requiresAdmin) {
        if (isLoggedInAdmin) {
            console.log('[Guard V3] Admin access GRANTED.');
            next(); // Là Admin, cho phép
        } else {
            console.log('[Guard V3] Admin access DENIED. Redirecting to AdminLogin.');
            // Dù là user thường hay guest, nếu không phải admin thì về trang login admin
            next({ name: 'AdminLogin', query: { redirect: to.fullPath } });
        }
        return;
    }

    // 3. Xử lý các route yêu cầu quyền User
    if (requiresUser) {
        if (isLoggedInUser) {
            console.log('[Guard V3] User access GRANTED.');
            next(); // Là User, cho phép
        } else {
            console.log('[Guard V3] User access DENIED. Redirecting to UserLogin (Auth).');
            // Dù là admin hay guest, nếu không phải user thường thì về trang login user
            next({ name: 'Auth', query: { redirect: to.fullPath } });
        }
        return;
    }

    // 4. Xử lý các route yêu cầu đăng nhập chung (ít dùng nếu đã có requiresUser/Admin)
    // Đây là trường hợp "cứ đăng nhập là được", bất kể vai trò
    if (requiresAuth) {
        if (isLoggedInUser || isLoggedInAdmin) {
            console.log('[Guard V3] General Auth access GRANTED.');
            next();
        } else {
            console.log('[Guard V3] General Auth required. Redirecting to User Login (Auth).');
            next({ name: 'Auth', query: { redirect: to.fullPath } });
        }
        return;
    }

    // 5. Các route công khai (không có meta nào ở trên)
    console.log('[Guard V3] Public route access GRANTED.');
    next(); // Luôn cho phép
});
// --- Hết Navigation Guards ---


export default router;