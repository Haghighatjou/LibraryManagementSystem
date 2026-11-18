# Library Management System

A Library Management System built with ASP.NET Core Web API and Entity Framework Core.

This project provides RESTful APIs for managing books, library members, borrowing and returning books, and user authentication.

## Technologies

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* xUnit
* Moq
* SQLite (for testing)

## Features

### Book Management

* Create, retrieve, update, and delete books.
* Track book availability.
* Prevent deletion of books with loan history.

### Member Management

* Register and manage library members.
* Enable and disable members.
* Validate member information.

### Loan Management

* Borrow and return books.
* Prevent borrowing unavailable books.
* Prevent borrowing by inactive members.
* Generate loan reports.

### Authentication and Authorization

* User registration and login.
* Password hashing.
* JWT-based authentication.
* Role-based authorization (Admin and User).
* Admin user management.
* Protection against removing or demoting the last administrator.

## Project Structure

* `LibraryManagementAPI`: ASP.NET Core Web API and controllers.
* `LibraryManagement.core`: Models, data access, services, and business logic.
* `LibraryManagement.Tests`: Unit tests.
* `Library Management System`: Earlier console-based version of the project.

## Database

The application uses SQL Server and Entity Framework Core.

Configure the database connection string in the application settings before running the API.

Configure the JWT secret key securely using User Secrets or environment variables. Do not commit real secrets to source control.

## Running the Project

1. Clone the repository.
2. Open `LibraryManagementAPI.slnx` in Visual Studio.
3. Configure the database connection string.
4. Configure the JWT secret key.
5. Ensure the database is created and migrations are applied.
6. Run the `LibraryManagementAPI` project.

## Testing

The test project uses xUnit, SQLite in-memory databases, and Moq.

Run tests using Visual Studio Test Explorer or the `dotnet test` command.

## Project Status

This project is developed as a practical backend development project.
