import { createRouter, createWebHistory } from 'vue-router'
import Login from './views/Login.vue'
import Courses from './views/Courses.vue'
import CourseEdit from './views/CourseEdit.vue'

const routes = [
    { path: '/login', component: Login },
    { path: '/', redirect: '/courses' },
    { path: '/courses', component: Courses, meta: { requiresAuth: true } },
    { path: '/courses/:id', component: CourseEdit, meta: { requiresAuth: true } }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

router.beforeEach((to, from, next) => {
    const token = localStorage.getItem('token');
    if (to.meta.requiresAuth && !token) {
        next('/login');
    } else {
        next();
    }
})

export default router
