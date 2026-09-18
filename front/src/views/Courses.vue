<template>
<div class="fade-in">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold text-light"><i class="bi bi-journal-bookmark me-2"></i>Cursos</h2>
        <button class="btn btn-success shadow-sm" @click="openCreateModal"><i class="bi bi-plus-circle me-2"></i>Nuevo Curso</button>
    </div>
    
    <div class="card bg-dark border-secondary shadow mb-4">
        <div class="card-body p-2 d-flex align-items-center">
            <i class="bi bi-filter text-muted ms-2 me-2"></i>
            <select v-model="filterStatus" class="form-select bg-dark text-light border-secondary shadow-none w-auto d-inline-block" @change="loadCourses">
                <option value="">Todos los Estados</option>
                <option value="Draft">Borrador</option>
                <option value="Published">Publicado</option>
            </select>
        </div>
    </div>

    <div class="table-responsive shadow-sm rounded">
        <table class="table table-hover table-dark align-middle mb-0">
            <thead class="bg-secondary text-uppercase text-muted small">
                <tr>
                    <th class="py-3 ps-4">Título</th>
                    <th>Lecciones</th>
                    <th>Estado</th>
                    <th>Actualizado</th>
                    <th class="text-end pe-4">Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="c in courses" :key="c.id" class="transit-row">
                    <td class="ps-4 fw-bold text-info"><i class="bi bi-bookmark-fill me-2 text-primary"></i>{{ c.title }}</td>
                    <td><span class="badge bg-secondary rounded-pill">{{ c.lessonCount }}</span></td>
                    <td>
                        <span :class="{'badge bg-success': c.status == 'Published', 'badge bg-warning text-dark': c.status == 'Draft'}">
                            <i :class="c.status === 'Draft' ? 'bi bi-pencil-square' : 'bi bi-check-circle-fill'"></i>
                            {{ c.status === 'Draft' ? 'Borrador' : 'Publicado' }}
                        </span>
                    </td>
                    <td class="text-muted small">{{ new Date(c.updatedAt).toLocaleDateString() }}</td>
                    <td class="text-end pe-4">
                        <button class="btn btn-sm btn-outline-light me-2" @click="$router.push(`/courses/${c.id}`)" title="Editar">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button v-if="c.status === 'Draft'" class="btn btn-sm btn-outline-info me-2" @click="publish(c.id)" title="Publicar">
                            <i class="bi bi-upload"></i>
                        </button>
                        <button v-else class="btn btn-sm btn-outline-warning me-2" @click="unpublish(c.id)" title="Despublicar">
                            <i class="bi bi-download"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger" @click="deleteCourse(c.id)" title="Eliminar">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    
    <div v-if="courses.length === 0" class="text-center py-5 text-muted">
        <i class="bi bi-inbox fs-1 d-block mb-3"></i>
        <h4>No se encontraron cursos</h4>
        <p>Crea uno nuevo para comenzar.</p>
    </div>

    <!-- Create Course Modal -->
    <div v-if="showCreateModal" class="modal-backdrop fade show"></div>
    <div v-if="showCreateModal" class="modal d-block fade show" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content bg-dark border-secondary shadow-lg">
                <div class="modal-header border-secondary">
                    <h5 class="modal-title text-light"><i class="bi bi-plus-lg me-2"></i>Crear Nuevo Curso</h5>
                    <button type="button" class="btn-close btn-close-white" @click="showCreateModal = false"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label text-muted small text-uppercase fw-bold">Título del Curso</label>
                        <input v-model="newCourseTitle" class="form-control bg-dark text-light border-secondary" placeholder="Ej. Introducción a Vue 3" />
                    </div>
                </div>
                <div class="modal-footer border-secondary">
                    <button type="button" class="btn btn-secondary" @click="showCreateModal = false">Cancelar</button>
                    <button class="btn btn-primary" @click="createCourse" :disabled="!newCourseTitle">Crear Curso</button>
                </div>
            </div>
        </div>
    </div>

</div>
</template>

<style scoped>
.fade-in { animation: fadeIn 0.6s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
.transit-row { transition: background-color 0.2s; }
.transit-row:hover { background-color: rgba(255,255,255,0.05); }
</style>

<script setup>
import { ref, onMounted } from 'vue'
import { localApi } from '../localApi'

const courses = ref([])
const filterStatus = ref('')
const showCreateModal = ref(false)
const newCourseTitle = ref('')

const loadCourses = async () => {
    courses.value = localApi.getCourses(filterStatus.value)
}

const openCreateModal = () => {
    newCourseTitle.value = ''
    showCreateModal.value = true
}

const createCourse = async () => {
    if (!newCourseTitle.value) return
    localApi.createCourse({ title: newCourseTitle.value, description: '' })
    showCreateModal.value = false
    loadCourses()
}

const deleteCourse = async (id) => {
    if(!confirm("¿Estás seguro de eliminar este curso?")) return;
    localApi.deleteCourse(id)
    loadCourses()
}

const publish = async (id) => {
    try {
        localApi.publishCourse(id)
        loadCourses()
    } catch(e) {
        alert(e.message || "Error al publicar")
    }
}

const unpublish = async (id) => {
    localApi.unpublishCourse(id)
    loadCourses()
}

onMounted(loadCourses)
</script>
