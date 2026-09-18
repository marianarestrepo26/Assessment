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
│   └── appsettings.json    # Configuration (placeholders — use user-secrets for real values)
│
└── 📂 front/               # Vue 3 Frontend + Vite
    ├── 📂 src/
    │   ├── 📂 views/       # Application Pages (Login, Courses)
    │   ├── localApi.js     # Demo data layer (localStorage-backed)
    │   ├── api.js          # Axios client for the real backend (unused in demo mode)
    │   └── main.js         # Bootstrap & Theme Configuration
    └── package.json        # Dependencies
```

## 🚀 Quick Start (Demo Mode)

This repo runs as a **frontend-only demo**: no backend, no database, no setup.
All data (courses, lessons, login) is stored in the browser via `localStorage`,
seeded automatically the first time you open the app.

```bash
cd front
npm install
npm run dev
```
*Application available at: `http://localhost:3000`*

To reset the demo data back to the seed state, clear the browser's site data
(or run `localStorage.clear()` in the DevTools console) and reload.

## 🔑 Access Credentials

| User | Password |
| :--- | :--- |
| **test@test.com** | **Test@123** |

## 🖥️ Real Backend (optional)

A full ASP.NET Core 9 + MySQL API lives in `back/` and can be wired back up
later (see `front/src/api.js` for the Axios client that was used before the
demo-mode switch). It is not required to run the demo.

```bash
cd back
dotnet run
```
*API listening on the port in `back/Properties/launchSettings.json`.*

Before running it against a real database:
1. Set your own connection string and JWT key — via `dotnet user-secrets` or
   environment variables, **not** committed in `appsettings.json`.
2. Run `dotnet ef database update` (or let the app auto-migrate on startup).

## ✨ Key Features

- **🎨 Premium Dark UI**: Built with Bootswatch "Darkly" theme for a modern look.
- **🔐 Secure Auth**: Robust JWT implementation.
- **📱 Responsive**: Works on desktop and mobile.

## 🧪 Testing

The solution includes 5 critical business rule tests to ensure stability:
- Publishing validation.
- Unique lesson order logic.
- Soft delete mechanics.

---
*Developed by Mariana Restrepo*
