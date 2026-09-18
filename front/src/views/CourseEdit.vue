<template>
<div v-if="course" class="fade-in">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h3 class="text-light fw-bold"><i class="bi bi-pencil-square me-2"></i>Editar Curso</h3>
        <button class="btn btn-outline-light" @click="$router.push('/courses')"><i class="bi bi-arrow-left me-1"></i>Volver</button>
    </div>

    <div class="card bg-dark border-secondary shadow-lg mb-5">
        <div class="card-header bg-secondary border-secondary text-white d-flex align-items-center">
            <i class="bi bi-info-circle me-2"></i><h5 class="mb-0">Información Básica</h5>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <label class="form-label text-muted small text-uppercase fw-bold">Título</label>
                <input v-model="course.title" class="form-control bg-dark text-light border-secondary" />
            </div>
             <div class="mb-4">
                <label class="form-label text-muted small text-uppercase fw-bold">Descripción</label>
                <textarea v-model="course.description" class="form-control bg-dark text-light border-secondary" rows="3"></textarea>
            </div>
             <div class="text-end">
                 <button class="btn btn-primary px-4" @click="updateCourse"><i class="bi bi-save me-2"></i>Guardar Cambios</button>
             </div>
        </div>
    </div>

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h4 class="text-light"><i class="bi bi-list-task me-2"></i>Lecciones</h4>
        <button class="btn btn-sm btn-success shadow-sm" @click="addLesson"><i class="bi bi-plus-lg me-1"></i>Agregar Lección</button>
    </div>

    <div class="list-group shadow-sm">
        <div v-for="l in lessons" :key="l.id" class="list-group-item list-group-item-action bg-dark text-light border-secondary d-flex justify-content-between align-items-center p-3 animate-item">
            <div class="d-flex align-items-center">
                <span class="badge bg-secondary me-3 rounded-circle shadow-sm d-flex align-items-center justify-content-center" style="width: 35px; height: 35px; font-size: 1rem;">{{ l.order }}</span>
                <div>
                   <span class="fw-bold d-block">{{ l.title }}</span>
                   <small class="text-muted text-truncate d-block" style="max-width: 300px;">{{ l.content }}</small>
                </div>
            </div>
            <div>
                <button class="btn btn-sm btn-outline-secondary me-1" @click="moveUp(l)" :disabled="l.order <= 1" title="Subir"><i class="bi bi-arrow-up"></i></button>
                <button class="btn btn-sm btn-outline-secondary me-3" @click="moveDown(l)" :disabled="l.order >= lessons.length" title="Bajar"><i class="bi bi-arrow-down"></i></button>
                
                <button class="btn btn-sm btn-outline-info me-2" @click="editLesson(l)" title="Editar"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-outline-danger" @click="deleteLesson(l.id)" title="Eliminar"><i class="bi bi-trash"></i></button>
            </div>
        </div>
    </div>
    
    <div v-if="lessons.length === 0" class="text-center mt-5 p-5 border border-secondary border-dashed rounded text-muted">
        <i class="bi bi-journal-plus fs-1 mb-3 d-block"></i>
        <h5>No hay lecciones todavía</h5>
        <p>¡Agrega la primera lección para comenzar!</p>
    </div>

    <!-- Modal Styling -->
    <div v-if="showLessonModal" class="modal-backdrop fade show"></div>
    <div v-if="showLessonModal" class="modal d-block fade show" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content bg-dark border-secondary shadow-lg">
                <div class="modal-header border-secondary">
                    <h5 class="modal-title text-light"><i class="bi bi-file-earmark-text me-2"></i>{{ editingLesson?.id ? 'Editar' : 'Nueva' }} Lección</h5>
                    <button type="button" class="btn-close btn-close-white" @click="closeModal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label text-muted small text-uppercase fw-bold">Título</label>
                        <input v-model="lessonForm.title" class="form-control bg-dark text-light border-secondary" placeholder="Ej. Variables en C#" />
                    </div>
                    <div class="mb-3">
                        <label class="form-label text-muted small text-uppercase fw-bold">Orden</label>
                        <div class="input-group">
                            <span class="input-group-text bg-secondary border-secondary text-white"><i class="bi bi-sort-numeric-down"></i></span>
                            <input v-model.number="lessonForm.order" type="number" min="1" class="form-control bg-dark text-light border-secondary" @input="validateOrder" />
                        </div>
                        <small class="text-muted d-block mt-1">Si cambias el orden, las demás lecciones se ajustarán automáticamente.</small>
                    </div>
                    <div class="mb-3">
                        <label class="form-label text-muted small text-uppercase fw-bold">Contenido</label>
                        <textarea v-model="lessonForm.content" class="form-control bg-dark text-light border-secondary" rows="5" placeholder="Escribe el contenido de la lección..."></textarea>
                    </div>
                </div>
                <div class="modal-footer border-secondary">
                    <button type="button" class="btn btn-secondary" @click="closeModal">Cancelar</button>
                    <button class="btn btn-primary" @click="saveLesson"><i class="bi bi-check2-circle me-1"></i>Guardar</button>
                </div>
            </div>
        </div>
    </div>

</div>
</template>

<style scoped>
.fade-in { animation: fadeIn 0.5s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
.border-dashed { border-style: dashed !important; }
.animate-item { transition: all 0.3s ease; }
.animate-item:hover { transform: translateX(5px); }
</style>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { localApi } from '../localApi'

const route = useRoute()
const course = ref(null)
const lessons = ref([])
const showLessonModal = ref(false)
const editingLesson = ref(null)
const lessonForm = ref({ title: '', content: '', order: 1 })

const load = async () => {
    const id = route.params.id
    course.value = localApi.getCourse(id)
    lessons.value = localApi.getLessonsByCourse(id).sort((a, b) => a.order - b.order)
}

const updateCourse = async () => {
    localApi.updateCourse(course.value.id, {
        title: course.value.title,
        description: course.value.description
    })
    // Simple visual feedback could be improved with a toast
    const btn = document.activeElement;
    const originalText = btn.innerHTML;
    btn.innerHTML = '<i class="bi bi-check-lg"></i> Guardado';
    btn.classList.add('btn-success');
    btn.classList.remove('btn-primary');
    setTimeout(() => {
        btn.innerHTML = originalText;
        btn.classList.remove('btn-success');
        btn.classList.add('btn-primary');
    }, 2000);
}

const addLesson = () => {
    editingLesson.value = null
    lessonForm.value = { title: '', content: '', order: lessons.value.length + 1 }
    showLessonModal.value = true
}

const editLesson = (l) => {
    editingLesson.value = l
    lessonForm.value = { ...l }
    showLessonModal.value = true
}

const deleteLesson = async (id) => {
    if(!confirm("¿Eliminar lección?")) return
    localApi.deleteLesson(id)
    load()
}

const saveLesson = async () => {
    try {
        if (editingLesson.value) {
            localApi.updateLesson(editingLesson.value.id, { ...lessonForm.value })
        } else {
            localApi.createLesson({ ...lessonForm.value, courseId: course.value.id })
        }
        closeModal()
        load()
    } catch(e) {
        alert(e.message || "Error al guardar lección")
    }
}

const moveUp = async (l) => {
    if(l.order <= 1) return;
    localApi.updateLesson(l.id, { ...l, order: l.order - 1 })
    load()
}

const moveDown = async (l) => {
    if(l.order >= lessons.value.length) return;
    localApi.updateLesson(l.id, { ...l, order: l.order + 1 })
    load()
}

const validateOrder = () => {
    if(lessonForm.value.order < 1 || !lessonForm.value.order) {
        lessonForm.value.order = 1;
    }
}

const closeModal = () => {
    showLessonModal.value = false
}

onMounted(load)
</script>
