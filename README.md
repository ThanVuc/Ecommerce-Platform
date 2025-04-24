# Ecommerce Platform

This is a full-stack Ecommerce Platform project that includes a front-end UI built with Angular and a back-end API built with ASP.NET Core. The project also uses NGINX for reverse proxy and SQL Server for database management. It supports real-time notifications using SignalR and includes features like authentication, authorization, and role management.

---

## **Project Structure**

```
Ecommerce-Platform/
├── EPlatform_API/          # Back-end API built with ASP.NET Core
│   ├── appsettings.json    # Configuration file for the API
│   ├── Dockerfile          # Dockerfile for the API
│   ├── docker-compose.yml  # Docker Compose file for API services
│   ├── Controllers/        # API controllers
│   ├── Data/               # Database context and migrations
│   ├── DTOs/               # Data Transfer Objects
│   ├── ExtensionMethods/   # Extension methods for configuration
│   ├── Helper/             # Helper classes
│   ├── IRepository/        # Repository interfaces
│   ├── IServices/          # Service interfaces
│   ├── LogFolder/          # Log files
│   ├── mailsave/           # Saved email files
│   ├── Mappers/            # DTO mappers
│   ├── Migrations/         # Database migrations
│   ├── Models/             # Data models
│   ├── Repository/         # Repository implementations
│   ├── Services/           # Service implementations
│   ├── Setting/            # Application settings
│   ├── Sqls/               # SQL scripts
│   ├── UnitOfWork/         # Unit of Work pattern
├── EPlatform_UI/           # Front-end UI built with Angular
│   ├── angular.json        # Angular configuration file
│   ├── Dockerfile          # Dockerfile for the UI
│   ├── nginx.conf          # NGINX configuration for the UI
│   ├── package.json        # Node.js dependencies
│   ├── src/                # Angular source code
│   ├── public/             # Public assets
├── nginx/                  # NGINX configuration for reverse proxy
│   ├── nginx.conf          # Main NGINX configuration
│   ├── api.conf            # API-specific NGINX configuration
│   ├── ui.conf             # UI-specific NGINX configuration
├── docker-compose.yml      # Main Docker Compose file for the project
└── README.md               # Project documentation
```

---

## **Features**

### **Back-End API (EPlatform_API)**
- Built with ASP.NET Core 8.0.
- Supports JWT-based authentication and authorization.
- Role-based access control using Identity Framework.
- Real-time notifications using SignalR.
- Database management with SQL Server.
- Redis caching for improved performance and save the security token.
- MongoDB integration for specific use cases.
- Hangfire for background job processing.

### **Front-End UI (EPlatform_UI)**
- Built with Angular 18.2.3.
- Responsive design using SCSS and Bootstrap.
- Real-time updates via SignalR.
- User-friendly interface for customers, shop owners, and admins.
- Features include product management, order management, and user management.

### **NGINX**
- Acts as a reverse proxy for the API and UI.
- Configured to handle WebSocket connections for SignalR.
- Supports HTTPS redirection and Cloudflare Flexible SSL.

---

## **Setup Instructions**

### **Prerequisites**
- **Node.js**: v22.13.1 or higher
- **Angular CLI**: v18.2.10
- **ASP.NET Core**: v8.0.3
- **SQL Server**: 2022 or higher
- **Redis**: Latest version
- **MongoDB**: Latest version
- **Docker**: Latest version
- **Docker Compose**: Latest version

---

### **1. Clone the Repository**
```bash
git clone https://github.com/ThanVuc/Ecommerce-Platform.git
cd ecommerce-platform
```

---

### **2. Back-End API Setup**

#### **Local Development**
1. Navigate to the API folder:
   ```bash
   cd EPlatform_API/EPlatform_API
   ```
2. Add the required configuration files:
   - `appsettings.json`
   - `appsettings.Development.json`
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Run the API:
   ```bash
   dotnet watch run
   ```
5. Note that config the https to use jwt by cookies
#### **Docker Setup**
1. Build and run the API using Docker Compose:
   ```bash
   docker-compose up --build
   ```

---

### **3. Front-End UI Setup**

#### **Local Development**
1. Navigate to the UI folder:
   ```bash
   cd EPlatform_UI
   ```
2. Add the required environment files:
   - `environment.ts`
   - `environment.development.ts`
3. Install dependencies:
   ```bash
   npm install
   ```
4. Run the UI:
   ```bash
   ng serve
   ```

#### **Docker Setup**
1. Build and run the UI using Docker Compose:
   ```bash
   docker-compose up --build
   ```

---

### **4. NGINX Setup**
1. Ensure the `nginx` folder contains the following configuration files:
   - `nginx.conf`
   - `api.conf`
   - `ui.conf`
2. NGINX will be automatically configured and started when using Docker Compose.

---

## **Deployment**

### **Using Docker Compose**
1. Ensure Docker and Docker Compose are installed on your server.
2. Copy the `docker-compose.yml` file and the `nginx` folder to your server.
3. Run the following command:
   ```bash
   docker-compose up --build -d
   ```

---

## **Environment Variables**

### **API**
- `ASPNETCORE_ENVIRONMENT`: Set to `Production` for production environments.
- `ConnectionStrings__Default`: Connection string for the main SQL Server database.
- `ConnectionStrings__VietNamDB`: Connection string for the Vietnamese location database.
- `Redis:Password`: Password for Redis.

### **UI**
- `environment.ts` and `environment.development.ts` should contain the API base URL.

---

## **Technologies Used**
- **Back-End**: ASP.NET Core, SignalR, Identity Framework, Hangfire
- **Front-End**: Angular, Bootstrap, SCSS
- **Database**: SQL Server, MongoDB
- **Caching**: Redis
- **Reverse Proxy**: NGINX
- **Containerization**: Docker, Docker Compose

---

## **Contributing**
1. Fork the repository.
2. Create a new branch:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add feature-name"
   ```
4. Push to the branch:
   ```bash
   git push origin feature-name
   ```
5. Open a pull request.

---

## **License**
This project is licensed under the MIT License. See the `LICENSE` file for details.

---

## **Contact**
For any questions or issues, please contact:
- **Email**: sinhnguyen417@gmail.com
- **GitHub**: https://github.com/ThanVuc