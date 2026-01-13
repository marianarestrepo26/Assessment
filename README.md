# Online Course Platform

A modern web application for managing online courses, featuring a sleek dark mode UI and robust backend architecture.

## 📂 Project Structure

```
/
├── 📂 back/                # ASP.NET Core 9.0 API
│   ├── 📂 Controllers/     # API Endpoints
│   ├── 📂 Data/            # Database Context & Migrations
│   ├── 📂 DTOs/            # Data Transfer Objects
│   ├── 📂 Models/          # Core Domain Entities
│   ├── 📂 Services/        # Business Logic (Reordering, Auth)
│   └── appsettings.json    # Configuration (Remote MySQL)
│
└── 📂 front/               # Vue 3 Frontend + Vite
    ├── 📂 src/
    │   ├── 📂 views/       # Application Pages (Login, Courses)
    │   ├── api.js          # Axios Setup with JWT Interceptor
    │   └── main.js         # Bootstrap & Theme Configuration
    └── package.json        # Dependencies
```

## 🚀 Quick Start

Follow these simple steps to get the platform running.

### 1️⃣ Start the Backend
```bash
cd back
dotnet run
```
*API listening on: `http://localhost:5000`*

### 2️⃣ Start the Frontend
```bash
cd front
npm run dev
```
*Application available at: `http://localhost:3000`*

## 🔑 Access Credentials
An admin user is automatically created on startup.

| User | Password |
| :--- | :--- |
| **test@test.com** | **Test@123** |

## ✨ Key Features

- **🎨 Premium Dark UI**: Built with Bootswatch "Darkly" theme for a modern look.
- **🔐 Secure Auth**: Robust JWT implementation.
- **📱 Responsive**: Works on desktop and mobile.

## 🛠️ Configuration & Database

The project uses **MySQL** (or compatible) by default. 

### 1. Configure Connection String
Edit `back/appsettings.json` and update `DefaultConnection` with your database credentials:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CoursePlatform;User=root;Password=yourpassword;"
}
```

### 2. Run Migrations
The application automatically applies migrations on startup. Manually:
```bash
cd back
dotnet ef database update
```

## 🧪 Testing

The solution includes 5 critical business rule tests to ensure stability:
- Publishing validation.
- Unique lesson order logic.
- Soft delete mechanics.

---
*Developed by Mariana Restrepo*
