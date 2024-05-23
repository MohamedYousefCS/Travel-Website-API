# Travel Website System

## Overview
Travel Website is a comprehensive travel booking and management system designed to streamline the process of managing travel packages, services, bookings, payments, and reviews. It supports multiple user roles including Super Admin, Package Admin, Tourist Guide, Customer Service, and general Users.

## Features

### User Management
#### User Registration and Profile Management
- Users can register and manage their profiles.
- Supports multiple user roles: Super Admin, Package Admin, Tourist Guide, Customer Service, and User.

### Package Management
#### Package Operations
- Create, update, and delete travel packages.
- Detailed package information including name, description, quality, price, currency, start date, status, and booking status.
- Associate multiple services with each package.

### Service Management
#### Service Operations
- Create, update, and delete services.
- Detailed service information including name, description, type, quality, price, currency, status, and service provider.
- Link services to multiple packages.

### Booking Management
#### Booking Operations
- Users can book travel packages.
- Store booking details including booking date.
- Each booking links to one package and one payment.

### Payment Processing
#### Payment Operations
- Manage payments for bookings.
- Payment details include payment method, status, currency, total price, date, and receipt URL.
- Secure payment processing.

### Review System
#### Review Operations
- Users can write reviews for packages and services.
- Review details include body and rating.
- Manage associations of reviews to users, packages, and services.

### Relationship Management
#### Relationships
- Users can book multiple packages, but each booking is for a single package.
- Users can write multiple reviews.
- Packages can include multiple services and have multiple reviews.
- Services can have multiple reviews.
- Each booking is associated with a single payment.

## Non-Functional Requirements

### Security
- Secure authentication and authorization.
- Role-based access control.

### Performance
- Efficient handling of concurrent users.
- Optimized database queries and indexing.

### Usability
- Intuitive and accessible user interface.

### Scalability
- Scalable system architecture to accommodate growth in users, packages, and bookings.

### Reliability
- High availability and reliability with regular backups and disaster recovery plans.

### Maintainability
- Clear documentation and well-structured code for ease of maintenance and updates.


## Get Started

To get started with the Attendance Management System, follow these steps:

1. Clone the repository to your local machine.
    ```bash
        git clone https://github.com/MohamedYousefCS/Travel-Website-API.git
    ```
2. Open the project in Visual Studio.
3. Install Entity Framework Core and Entity Framework Core tools if not already installed. You can install them via NuGet Package Manager or by using the .NET CLI:
    ```bash
        dotnet add package Microsoft.EntityFrameworkCore
        dotnet add package Microsoft.EntityFrameworkCore.Tools
    ```
4. Build the project to resolve any dependencies.
5. Run the application to start using the Travel Website System.

## Team Members:

[MohamedYousefCS](https://github.com/MohamedYousefCS)

[MohamedHelmy12](https://github.com/MohamedHelmy12)

[sherifebrahim](https://https://github.com/sherifebrahim)

[amira-mohamed-mahmoud](https://github.com/amira-mohamed-mahmoud)

-----------------------------------------------


This README provides an overview of the Travel Website system, its features, and the database schema. 
