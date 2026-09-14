
# Quantum Count

> Full-Stack Inventory & Operations Management System built with ASP.NET Core, Blazor, Entity Framework Core, and SQL Server.

[![Live Demo](https://img.shields.io/badge/Live-Demo-00C7B7?style=for-the-badge)](https://quantumcount.runasp.net)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Interactive%20Server-512BD4?style=for-the-badge&logo=blazor&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-ORM-512BD4?style=for-the-badge)](https://learn.microsoft.com/ef/core/)

---

## 🚀 Live Demo

**[Open Quantum Count](https://quantumcount.runasp.net)**

Quantum Count is publicly deployed and fully functional.

---

## 📌 Overview

**Quantum Count** is a full-stack inventory and operations management system designed to centralize the management of materials, equipment, projects, procurement, staff, reporting, and operational activities in a single application.

The system is built with **ASP.NET Core .NET 10** and **Blazor Interactive Server**, using **Entity Framework Core** for data access and **Microsoft SQL Server** as the database.

The application focuses on real operational workflows rather than simple CRUD screens.

For example:

```text
Purchase Order
      ↓
Receive Items
      ↓
Inventory Increases
      ↓
Inventory Transaction Recorded
````

and:

```text
Project
 ├── Materials
 ├── Equipment
 └── Tasks
```

This allows related operations to remain synchronized throughout the system.

---

## ✨ Key Features

### 📊 Dashboard

* Inventory overview
* Equipment statistics
* Project progress
* Operational metrics
* Quick access to major modules

### 📦 Materials & Inventory

* Create, update, and manage materials
* Inventory categories
* Stock quantity tracking
* Minimum and maximum stock levels
* Low-stock detection
* Unit pricing
* Inventory transactions
* Stock In / Stock Out operations
* Inventory history

### 🛠️ Equipment & Assets

* Equipment registration
* Equipment categories
* Equipment codes and serial numbers
* Brand and model information
* Purchase information
* Equipment status tracking
* Equipment history
* Assignment and return workflows

Supported equipment states include:

* Available
* In Use
* Maintenance
* Damaged
* Retired

### 📁 Project Management

Projects provide an operational layer connecting inventory and equipment with project work.

Each project can contain:

```text
Project
 ├── Materials
 ├── Equipment
 └── Tasks
```

Features include:

* Project creation and editing
* Project materials
* Material issuing
* Material returns
* Equipment assignment
* Equipment returns
* Project tasks
* Task progress tracking
* Task status management
* Automatic project progress calculation

Project progress is calculated from task progress rather than manually entered.

### 🧾 Procurement

Complete procurement workflow:

```text
Supplier
   ↓
Purchase Order
   ↓
Purchase Order Items
   ↓
Receive Items
   ↓
Inventory Updated
   ↓
Inventory Transaction
```

Features include:

* Supplier management
* Purchase orders
* Purchase order items
* Purchase order status tracking
* Partial receiving
* Full receiving
* Automatic inventory updates
* Inventory transaction recording

Supported purchase order states include:

* Draft
* Submitted
* Approved
* Ordered
* Partially Received
* Received
* Cancelled

### 📈 Reports & Analytics

* Inventory reports
* Operational metrics
* Filtering
* Report generation
* Excel export
* PDF export

### 👥 Staff & Roles

* User management
* Role-based structure
* ASP.NET Core Identity integration
* Authentication and authorization

### ⚙️ Application Settings

Configurable application settings including:

* Organization name
* Currency
* Time zone
* Date format
* Low-stock alerts
* Negative stock behavior
* Default unit
* Purchase order notifications
* Equipment maintenance notifications
* Theme settings
* Sidebar preferences

### 🤖 AI Assistant

Quantum Count includes an integrated AI assistant designed to provide an intelligent interaction layer within the application.

The assistant is intended to complement the application's operational workflows rather than replace its core business logic.

---

## 🔐 Authentication & Authorization

Quantum Count uses **ASP.NET Core Identity** for authentication.

The application supports:

* User registration
* User login
* Logout
* Authentication cookies
* Protected application pages
* Authorization
* Identity persistence through SQL Server

Protected pages require authentication.

---

## 🏗️ Architecture

Quantum Count follows a lightweight full-stack ASP.NET Core architecture.

```text
┌───────────────────────────────────────────────┐
│                 Blazor UI                     │
│          Interactive Server Rendering         │
├───────────────────────────────────────────────┤
│              Application Layer                │
│       Services / Business Workflows           │
├───────────────────────────────────────────────┤
│              ASP.NET Core API                 │
│              Controllers                      │
├───────────────────────────────────────────────┤
│              Entity Framework Core             │
├───────────────────────────────────────────────┤
│               SQL Server                      │
└───────────────────────────────────────────────┘
```

### Application Structure

```text
Quantum Count
│
├── Components/
│   ├── Layout/
│   ├── Pages/
│   └── Shared/
│
├── Controllers/
│
├── Data/
│
├── Models/
│
├── Services/
│
├── wwwroot/
│   ├── css/
│   └── js/
│
└── Program.cs
```

---

## 🧩 Technology Stack

| Technology                    | Purpose                                |
| ----------------------------- | -------------------------------------- |
| **C#**                        | Primary programming language           |
| **.NET 10**                   | Application framework                  |
| **ASP.NET Core**              | Backend and application infrastructure |
| **Blazor Interactive Server** | User interface                         |
| **MudBlazor**                 | UI component library                   |
| **Entity Framework Core**     | ORM / data access                      |
| **Microsoft SQL Server**      | Relational database                    |
| **ASP.NET Core Identity**     | Authentication & authorization         |
| **ASP.NET Core Web API**      | REST API endpoints                     |
| **OpenAPI / Swagger**         | API documentation and testing          |
| **Git / GitHub**              | Version control                        |
| **Visual Studio**             | Development environment                |
| **MonsterASP.NET**            | Deployment / hosting                   |

---

## 🗄️ Database

Quantum Count uses **Microsoft SQL Server** with Entity Framework Core.

The database contains entities for areas including:

* Inventory categories
* Materials
* Inventory transactions
* Equipment
* Equipment history
* Projects
* Project materials
* Project equipment
* Project tasks
* Suppliers
* Purchase orders
* Purchase order items
* Application settings
* ASP.NET Identity

Entity relationships are managed through Entity Framework Core.

Examples include:

```text
InventoryCategory
       │
       ├── Materials
       └── Equipment
```

```text
Project
   ├── ProjectMaterial
   ├── ProjectEquipment
   └── ProjectTask
```

```text
Supplier
    │
    └── PurchaseOrder
            │
            └── PurchaseOrderItem
```

---

## 🔄 Core Business Workflows

### Inventory Receiving

When a purchase order item is received:

```text
Validate Receipt
      ↓
Increase Material Quantity
      ↓
Increase QuantityReceived
      ↓
Recalculate Purchase Order Status
      ↓
Create Inventory Transaction
      ↓
Save Changes
```

### Project Material Issue

```text
Select Project
      ↓
Select Material
      ↓
Issue Quantity
      ↓
Decrease Inventory
      ↓
Record ProjectMaterial Issue
      ↓
Create Inventory Transaction
```

### Project Material Return

```text
Return Material
      ↓
Increase Inventory
      ↓
Update ProjectMaterial
      ↓
Create Inventory Transaction
```

### Equipment Assignment

```text
Available Equipment
      ↓
Assign to Project
      ↓
Equipment → In Use
      ↓
Create ProjectEquipment Assignment
      ↓
Record EquipmentHistory
```

### Equipment Return

```text
Return Equipment
      ↓
Equipment → Available
      ↓
Close Assignment
      ↓
Record EquipmentHistory
```

---

## 🔌 API

Quantum Count also exposes ASP.NET Core Web API endpoints for application resources.

API areas include:

* Inventory Categories
* Materials
* Equipment
* Inventory Transactions
* Equipment History
* Projects
* Suppliers
* Purchase Orders
* Application Settings

The API layer separates HTTP endpoints from application business logic.

---

## 🛡️ Data Integrity

The application uses Entity Framework Core relationships and database constraints to maintain consistency.

Examples include:

* Unique material codes
* Unique equipment codes
* Unique project codes
* Unique supplier codes
* Unique purchase order numbers
* Equipment serial number uniqueness
* Foreign key relationships
* Restricted deletes where historical data must be preserved
* Decimal precision for quantities and financial values
* Atomic procurement receiving operations

---

## 🎨 UI & Design

Quantum Count uses a dark, technical interface designed around:

* Obsidian/dark backgrounds
* Cyan and green accents
* Glass-style cards
* Rounded UI components
* Subtle borders and shadows
* Responsive layouts
* Compact navigation
* Clear data presentation
* Operational dashboards

The interface is designed to feel like an actual business application rather than a collection of disconnected CRUD pages.

---

## 🚀 Deployment

Quantum Count is deployed publicly using **MonsterASP.NET**.

```text
GitHub
   ↓
Visual Studio Publish
   ↓
MonsterASP.NET
   ↓
.NET 10 Application
   ↓
SQL Server
```

### Live Application

**[https://quantumcount.runasp.net](https://quantumcount.runasp.net)**

---

## 🧪 Testing

The application has been tested across its primary operational workflows, including:

* Authentication
* Materials
* Inventory transactions
* Equipment
* Equipment history
* Project management
* Project materials
* Project equipment
* Project tasks
* Procurement
* Suppliers
* Purchase orders
* Receiving
* Reports
* Settings
* API endpoints

---

## 💻 Running Locally

### Prerequisites

Make sure the following are installed:

* .NET 10 SDK
* Visual Studio 2026
* SQL Server
* SQL Server Management Studio or another SQL client
* Git

### Clone the repository

```bash
git clone https://github.com/Usman444793/Quantum_Count.git
```

```bash
cd Quantum_Count
```

### Configure the database

Update the connection string in your local configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_SQL_SERVER_CONNECTION_STRING"
  }
}
```

> Never commit production database credentials or API keys to source control.

### Restore dependencies

```bash
dotnet restore
```

### Apply migrations

```bash
dotnet ef database update
```

### Run the application

```bash
dotnet run
```

Or open the solution in Visual Studio and run the project using the configured development profile.

---

## 📂 Main Application Modules

```text
Dashboard
│
├── Materials & Inventory
│
├── Equipment & Assets
│
├── Projects
│   ├── Materials
│   ├── Equipment
│   └── Tasks
│
├── Procurement
│   ├── Suppliers
│   ├── Purchase Orders
│   └── Receiving
│
├── Reports & Analytics
│
├── Staff & Roles
│
├── Settings
│
└── AI Assistant
```

---

## 🎯 Project Goals

Quantum Count was developed to demonstrate practical full-stack software engineering using the Microsoft ecosystem.

The project focuses on:

* Full-stack .NET development
* Blazor application development
* Entity Framework Core
* Relational database design
* REST API development
* Authentication and authorization
* Business workflow implementation
* Data integrity
* Enterprise-style application structure
* Deployment of a real web application

---

## 🔮 Future Improvements

Potential future improvements include:

* Expanded audit logging
* More granular role and permission management
* Automated testing
* CI/CD pipelines
* Application monitoring
* Advanced analytics
* Additional AI-assisted workflows

These are intentionally separated from the current core application so that the existing operational modules remain focused and maintainable.

---

## 👨‍💻 Author

**Usman Amjad**

Software Engineering Student | .NET Developer | AI Learner

### Connect

* GitHub: [Usman444793](https://github.com/Usman444793)
* LinkedIn: [Usman Amjad](https://www.linkedin.com/in/usman-amjad-46aa66329/)
* Live Project: [Quantum Count](https://quantumcount.runasp.net)

---

## ⭐ Project

If you find Quantum Count interesting, consider giving the repository a star.

**Built with C#, .NET 10, Blazor, Entity Framework Core, and SQL Server.**

```
