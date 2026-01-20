# Contact Form SoftwareHouse

ASP.NET Core MVC web application with a contact form for Software House, utilizing Entity Framework Core and SQL Server.

## Table of Contents
- [About the Project](#about-the-project)
- [Technologies](#technologies)
- [Requirements](#requirements)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Author](#author)

## About the Project

The project presents a software house website with a functional contact form that saves customer inquiries to a SQL Server database. 

The application was designed with the following in mind:
- Clean architecture
- Security (CSRF protection, data validation)
- Responsiveness (Tailwind CSS)

## Technologies

### Backend: 
- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core 8.0.16** - ORM
- **SQL Server** - Database
- **C# 12** - Programming language

### Frontend:
- **Razor Pages** - Rendering views
- **Tailwind CSS** - Styling
- **HTML5 & CSS3**
- **jQuery Validation** - Client-side validation

## Requirements

- **.NET 8.0 SDK** or newer
- **SQL Server** (LocalDB, Express, or full version)
- **Visual Studio 2022**
- **Git**

## Installation

### 1. Clone the repository:
```bash
git clone https://github.com/MacPal2002/ContactFormSoftwareHouse.git
cd ContactFormSoftwareHouse
```

### 2. Navigate to the project directory:
```bash
cd Projekt001
```

### 3. Restore NuGet dependencies:
```bash
dotnet restore
```

## Configuration

### 1. Configure connection string

Edit the `appsettings.json` file and set the connection string to your SQL Server database:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ContactFormDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 2. Create the database

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> **Note:** If `dotnet ef` is not installed, run:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

## Running the Application

### Run in development mode:
```bash
dotnet run
```

The application will be available at: 
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### Run in Visual Studio: 
1. Open `Projekt001.sln`
2. Press `F5` or click "Run"

## Project Structure

```
ContactFormSoftwareHouse/
├── Projekt001/                          # Main application project
│   ├── Controllers/                     # MVC controllers
│   │   ├── ContactController.cs         # Contact form handler
│   │   └── HomeController.cs            # Home page controller
│   ├── Entities/                        # Database entities
│   │   └── ContactEntity.cs             # Data model for messages
│   ├── Models/                          # ViewModels
│   │   ├── ContactModel.cs              # Contact form model
│   │   └── ErrorViewModel.cs            # Error page model
│   ├── Views/                           # Razor views
│   │   ├── Contact/
│   │   │   ├── Index.cshtml             # Contact form
│   │   │   └── Confirmation.cshtml      # Submission confirmation
│   │   ├── Home/
│   │   │   └── Index.cshtml             # Home page
│   │   └── Shared/
│   │       ├── _Layout.cshtml           # Main layout
│   │       └── Error.cshtml             # Error page
│   ├── wwwroot/                         # Static files (CSS, JS, images)
│   ├── AppDbContext.cs                  # Entity Framework context
│   ├── ContactFormRepository.cs         # Repository implementation
│   ├── IContactFormRepository.cs        # Repository interface
│   ├── Program.cs                       # Application entry point
│   ├── appsettings.json                 # Application configuration
│   └── Projekt001.csproj                # Project file
├── Testowanie001/                       # Unit tests project
└── Projekt001.sln                       # Visual Studio solution
```

## Security

The project implements the following security mechanisms: 
- **Anti-CSRF Tokens** - Protection against CSRF attacks
- **Model Validation** - Input data validation
- **XSS Protection** - Razor automatically encodes output
- **HTTPS Redirection** - Enforcing encrypted connection
- **Error Handling** - Safe exception handling
- **Logging** - Detailed operation logging

## License

This project was created for educational/demonstration purposes. 
