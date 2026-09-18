// Demo data layer: persists everything in the browser's localStorage.
// Mimics the shape of the real backend responses so the views don't
// need to know whether they're talking to a server or to the browser.

const DB_KEY = 'course-platform-demo-db'
const TOKEN_KEY = 'token'

function uid() {
    return crypto.randomUUID()
}

function now() {
    return new Date().toISOString()
}

function seedDb() {
    const courseVue = uid()
    const courseCsharp = uid()
    const t0 = now()

    return {
        user: { email: 'test@test.com', password: 'Test@123', fullName: 'Test User' },
        courses: [
            { id: courseVue, title: 'Introducción a Vue 3', description: 'Fundamentos de componentes, reactividad y router.', status: 'Published', createdAt: t0, updatedAt: t0 },
            { id: courseCsharp, title: 'C# para Principiantes', description: 'Sintaxis básica, POO y buenas prácticas.', status: 'Draft', createdAt: t0, updatedAt: t0 }
        ],
        lessons: [
            { id: uid(), courseId: courseVue, title: 'Componentes y props', content: 'Cómo dividir la UI en piezas reutilizables.', order: 1, createdAt: t0, updatedAt: t0 },
            { id: uid(), courseId: courseVue, title: 'Reactividad con ref y reactive', content: 'Estado reactivo en Vue 3.', order: 2, createdAt: t0, updatedAt: t0 },
            { id: uid(), courseId: courseVue, title: 'Vue Router', content: 'Navegación entre vistas.', order: 3, createdAt: t0, updatedAt: t0 }
        ]
    }
}

function loadDb() {
    const raw = localStorage.getItem(DB_KEY)
    if (raw) {
        try { return JSON.parse(raw) } catch { /* fall through to reseed */ }
    }
    const db = seedDb()
    saveDb(db)
    return db
}

function saveDb(db) {
    localStorage.setItem(DB_KEY, JSON.stringify(db))
}

function courseDto(db, course) {
    const lessonCount = db.lessons.filter(l => l.courseId === course.id).length
    return {
        id: course.id,
        title: course.title,
        description: course.description,
        status: course.status,
        createdAt: course.createdAt,
        updatedAt: course.updatedAt,
        lessonCount
    }
}

function requireCourse(db, id) {
    const course = db.courses.find(c => c.id === id)
    if (!course) throw new Error('Curso no encontrado.')
    return course
}

function requireLesson(db, id) {
    const lesson = db.lessons.find(l => l.id === id)
    if (!lesson) throw new Error('Lección no encontrada.')
    return lesson
}

export function resetDemoData() {
    saveDb(seedDb())
}

export const localApi = {
    // --- auth ---
    login(email, password) {
        const db = loadDb()
        if (email !== db.user.email || password !== db.user.password) {
            throw new Error('Credenciales inválidas')
        }
        const token = `demo-token.${btoa(email)}.${Date.now()}`
        localStorage.setItem(TOKEN_KEY, token)
        return { token, email: db.user.email, fullName: db.user.fullName }
    },

    // --- courses ---
    getCourses(status) {
        const db = loadDb()
        let courses = db.courses
        if (status) courses = courses.filter(c => c.status === status)
        return courses
            .slice()
            .sort((a, b) => new Date(b.updatedAt) - new Date(a.updatedAt))
            .map(c => courseDto(db, c))
    },

    getCourse(id) {
        const db = loadDb()
        const course = db.courses.find(c => c.id === id)
        return course ? courseDto(db, course) : null
    },

    createCourse({ title, description }) {
        const db = loadDb()
        const course = { id: uid(), title, description: description || '', status: 'Draft', createdAt: now(), updatedAt: now() }
        db.courses.push(course)
        saveDb(db)
        return courseDto(db, course)
    },

    updateCourse(id, { title, description }) {
        const db = loadDb()
        const course = requireCourse(db, id)
        course.title = title
        course.description = description
        course.updatedAt = now()
        saveDb(db)
        return courseDto(db, course)
    },

    deleteCourse(id) {
        const db = loadDb()
        db.courses = db.courses.filter(c => c.id !== id)
        db.lessons = db.lessons.filter(l => l.courseId !== id)
        saveDb(db)
    },

    publishCourse(id) {
        const db = loadDb()
        const course = requireCourse(db, id)
        const hasLessons = db.lessons.some(l => l.courseId === id)
        if (!hasLessons) throw new Error('Cannot publish a course with no lessons.')
        course.status = 'Published'
        course.updatedAt = now()
        saveDb(db)
    },

    unpublishCourse(id) {
        const db = loadDb()
        const course = requireCourse(db, id)
        course.status = 'Draft'
        course.updatedAt = now()
        saveDb(db)
    },

    // --- lessons ---
    getLessonsByCourse(courseId) {
        const db = loadDb()
        return db.lessons
            .filter(l => l.courseId === courseId)
            .sort((a, b) => a.order - b.order)
    },

    createLesson({ courseId, title, content, order }) {
        const db = loadDb()
        const conflict = db.lessons.some(l => l.courseId === courseId && l.order === order)
        if (conflict) throw new Error('Duplicate lesson order.')

        const lesson = { id: uid(), courseId, title, content: content || '', order, createdAt: now(), updatedAt: now() }
        db.lessons.push(lesson)
        saveDb(db)
        return lesson
    },

    updateLesson(id, { title, content, order }) {
        const db = loadDb()
        const lesson = requireLesson(db, id)
        const siblings = db.lessons.filter(l => l.courseId === lesson.courseId && l.id !== lesson.id)

        const oldOrder = lesson.order
        const newOrder = Math.min(Math.max(order, 1), siblings.length + 1)

        if (newOrder > oldOrder) {
            siblings.filter(l => l.order > oldOrder && l.order <= newOrder).forEach(l => { l.order -= 1 })
        } else if (newOrder < oldOrder) {
            siblings.filter(l => l.order >= newOrder && l.order < oldOrder).forEach(l => { l.order += 1 })
        }

        lesson.title = title
        lesson.content = content
        lesson.order = newOrder
        lesson.updatedAt = now()
        saveDb(db)
        return lesson
    },

    deleteLesson(id) {
        const db = loadDb()
        db.lessons = db.lessons.filter(l => l.id !== id)
        saveDb(db)
    }
}
