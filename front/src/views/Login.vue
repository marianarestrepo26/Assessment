<template>
<div class="row justify-content-center mt-5 fade-in">
    <div class="col-md-5 col-lg-4">
        <div class="card shadow-lg border-0 rounded-3">
            <div class="card-header bg-primary text-white text-center py-3">
                <h4 class="mb-0">Bienvenido</h4>
            </div>
            <div class="card-body p-4">
                <form @submit.prevent="handleLogin">
                    <div class="mb-4">
                        <label class="form-label text-muted small text-uppercase fw-bold">Email</label>
                        <input v-model="email" type="email" class="form-control form-control-lg bg-dark text-light border-secondary" placeholder="nombre@ejemplo.com" required />
                    </div>
                    <div class="mb-4">
                        <label class="form-label text-muted small text-uppercase fw-bold">Contraseña</label>
                        <input v-model="password" type="password" class="form-control form-control-lg bg-dark text-light border-secondary" placeholder="******" required />
                    </div>
                    <button type="submit" class="btn btn-primary w-100 btn-lg shadow-sm transit">Iniciar Sesión</button>
                    <div v-if="error" class="alert alert-danger mt-3 text-center small">{{ error }}</div>
                </form>
            </div>
        </div>
    </div>
</div>
</template>

<style scoped>
.fade-in { animation: fadeIn 0.8s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }
.transit { transition: all 0.3s; }
.transit:hover { transform: translateY(-2px); }
</style>

<script setup>
import { ref } from 'vue'
import { localApi } from '../localApi'

const email = ref('')
const password = ref('')
const error = ref('')

const handleLogin = async () => {
    try {
        localApi.login(email.value, password.value)
        window.location.href = '/courses'
    } catch (e) {
        error.value = 'Invalid Credentials'
    }
}
</script>
