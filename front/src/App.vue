<template>
  <div class="container-fluid p-0">
    <nav v-if="isAuthenticated" class="navbar navbar-expand-lg navbar-dark bg-primary mb-4 px-3">
      <a class="navbar-brand" href="#">Online Courses</a>
      <button class="btn btn-outline-light ms-auto" @click="logout">Logout</button>
    </nav>
    <div class="container">
      <router-view />
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const isAuthenticated = computed(() => !!localStorage.getItem('token'))

const logout = () => {
  localStorage.removeItem('token')
  router.push('/login')
  setTimeout(() => window.location.reload(), 100)
}
</script>
