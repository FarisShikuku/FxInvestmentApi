---

💼 Investment Portfolios API

Secure and scalable financial portfolio management backend built with .NET Core Web API.


---

🚀 Overview

Investment Portfolios API is a RESTful backend system designed to manage and track multiple investment portfolios including:

Forex

Crypto

Stocks

Digital Assets


The API provides structured portfolio management, capital tracking, returns monitoring, and withdrawal processing with secure, token-based authentication.


---

🛠️ Built With

Framework: ASP.NET Core (.NET Core Web API)

Language: C#

Database: SQL Server

ORM: Entity Framework Core

Authentication: JWT (JSON Web Token)

Architecture: RESTful API

IDE: Visual Studio 2022



---

🏗️ Architecture

Client Application → REST API → Secure Database

No direct database exposure

Controlled CRUD operations via API endpoints

Token-based authentication for all protected routes

Scalable and maintainable backend structure



---

🔐 Security

The system uses JWT Authentication:

Users authenticate via login endpoint

A secure token is generated

Every protected request must include:


Authorization: Bearer {token}

Unauthorized access is blocked.


---

📂 Project Structure

InvestmentPortfoliosAPI/
│── Controllers/
│── Models/
│── DTOs/
│── Services/
│── Data/
│── Migrations/
│── Program.cs
│── appsettings.json


---

📊 Core Features

✔ Portfolio Creation & Management
✔ Capital Allocation Tracking
✔ Returns Recording (Weekly/Monthly)
✔ Withdrawal Management
✔ Performance Summary Reporting
✔ Role-Based Access Control (Admin/User)
✔ Secure API Endpoints


---

📌 Sample Endpoints

Authentication

POST /api/auth/register
POST /api/auth/login

Portfolios

GET    /api/portfolios
GET    /api/portfolios/{id}
POST   /api/portfolios
PUT    /api/portfolios/{id}
DELETE /api/portfolios/{id}

Returns

POST /api/returns
GET  /api/returns/{portfolioId}

Withdrawals

POST /api/withdrawals
GET  /api/withdrawals/{portfolioId}


---

⚙️ Configuration

Update appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=InvestmentDB;Trusted_Connection=True;"
}

Run migrations:

dotnet ef migrations add InitialCreate
dotnet ef database update


---

▶️ Running the Project

1️⃣ Clone repository

git clone https://github.com/FarisShikuku/InvestmentPortfoliosAPI.git

2️⃣ Navigate to project

cd InvestmentPortfoliosAPI

3️⃣ Run

dotnet run

Swagger available at:

https://localhost:5001/swagger


---

📈 Future Improvements

Advanced Portfolio Analytics

Graph-based Performance Visualization

Docker Deployment

CI/CD Integration

Multi-Currency Support



---

👨‍💻 Author

Faris Shikuku
Backend Developer | Sphere Developers


---

📜 License

MIT License


---
