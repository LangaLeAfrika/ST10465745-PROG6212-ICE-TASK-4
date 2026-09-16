# Contract Claim Data Manager

## PROG6212 – ICE Task 4

### Project Description

The **Contract Claim Data Manager** is a C# .NET 8 console application developed to manage lecturer contract claim records.

The application uses **SQLite** as its database and **Entity Framework Core (EF Core)** for data access. It allows users to add, view, update and delete claim records. It also exports claim information to a text file and performs a direct ADO.NET query to count the number of claim records.

The application does not require a graphical interface, authentication or an approval workflow.

---

## Technologies Used

* C#
* .NET 8
* Visual Studio
* Entity Framework Core
* SQLite
* Microsoft.Data.Sqlite
* ADO.NET

### NuGet Packages

The project uses the following required NuGet packages:

* `Microsoft.EntityFrameworkCore.Sqlite`
* `Microsoft.Data.Sqlite`

---

## Claim Data

Each claim record contains the following information:

* Claim ID
* Lecturer Name
* Module Code
* Hours Worked
* Hourly Rate
* Claim Month
* Status
* Total Amount

The `TotalAmount` property is calculated automatically using:

**Hours Worked × Hourly Rate**

For example:

**10 hours × R350 = R3500**

The `TotalAmount` property is read-only and is not entered directly by the user.

---

# Data Access Concepts

## 1. ORM

ORM stands for **Object-Relational Mapping**.

An ORM allows an application to work with database data using programming objects instead of writing SQL for every database operation.

In this project, **Entity Framework Core** is used as the ORM. It allows the C# `Claim` objects to be stored and retrieved from the SQLite database.

---

## 2. Entity

An **entity** is a C# object that represents a record in a database.

In this project, the `Claim` class is the entity.

Each `Claim` object represents one lecturer claim record.

For example:

```text
ClaimId: 1
LecturerName: Nomsa Mthembu
ModuleCode: PROG6212
HoursWorked: 10
HourlyRate: 350
ClaimMonth: September 2026
Status: Submitted
```

---

## 3. DbContext

`DbContext` is an Entity Framework Core class that manages the connection between the application and the database.

The project contains a class called:

`ClaimContext`

It inherits from:

`DbContext`

The `ClaimContext` is responsible for configuring the SQLite database and providing access to the claim data.

The database connection is configured as:

```text
Data Source=claims.db
```

---

## 4. DbSet

A `DbSet` represents a collection of entities that can be queried and stored in the database.

This project contains:

```csharp
public DbSet<Claim> Claims { get; set; }
```

The `Claims` DbSet represents the claims table in the SQLite database.

It is used for operations such as:

* Adding claims
* Viewing claims
* Updating claims
* Deleting claims
* Querying claim records

---

## 5. Provider

A **database provider** allows Entity Framework Core to communicate with a specific type of database.

This project uses the **SQLite provider**:

`Microsoft.EntityFrameworkCore.Sqlite`

The SQLite provider allows Entity Framework Core to create and work with the `claims.db` SQLite database.

---

# Code First vs Database First

## Code First

**Code First** is an approach where the database structure is created from the application's C# classes and Entity Framework Core configuration.

In this project, the `Claim` class defines the claim data structure, while `ClaimContext` defines the database context.

The application uses:

```csharp
context.Database.EnsureCreated();
```

to ensure that the database and required table are created.

Therefore, this project follows a **Code First-style approach**.

---

## Database First

**Database First** is an approach where the database already exists and the application classes and models are generated or designed based on the existing database structure.

The database structure is therefore the starting point rather than the C# model.

This is different from the approach used in this project.

---

# Application Features

The application provides the following menu options:

### 1. Add Claim

Allows the user to enter a lecturer's:

* Name
* Module code
* Hours worked
* Hourly rate
* Claim month

New claims are given a default status of **Draft**.

The application validates that:

* Hours are between 1 and 160.
* Hourly rate is greater than zero.

### 2. View Claims

Displays all stored claim records, including the calculated total amount.

### 3. Update Claim Status

Allows the user to find a claim using its Claim ID and change its status.

### 4. Delete Claim

Allows the user to find a claim using its Claim ID, confirm the deletion and remove the record.

### 5. Export Claims

Creates a `Reports` directory if it does not already exist.

The application exports all claims to:

```text
Reports/claim_summary.txt
```

The file is created using `StreamWriter`.

The completed report is then read back and displayed in the console.

### 6. ADO.NET Record Count

The application uses:

* `SqliteConnection`
* `SqliteCommand`
* `ExecuteScalar()`

to execute a direct SQL query:

```sql
SELECT COUNT(*) FROM Claims
```

The result displays the number of claim records stored in the database.

### 7. Exit

Closes the application.

---

# Database

The application uses a SQLite database called:

```text
claims.db
```

The database is created automatically when the application starts.

Two sample claim records are inserted only when the database does not already contain any claim records. This prevents the sample records from being duplicated each time the application is opened.

---

# Project Structure

The project contains the following important files:

```text
ClaimDataManager
│
├── Claim.cs
├── ClaimContext.cs
├── Program.cs
├── README.md
├── claims.db
│
└── Reports
    └── claim_summary.txt
```

---

# Testing

The application was tested to confirm that:

1. The project builds and runs.
2. The two sample records are stored only once.
3. New claims can be added.
4. Existing claims can be viewed.
5. Claim statuses can be updated.
6. Claims can be deleted after confirmation.
7. Claim data remains stored after restarting the application.
8. Hours outside the range of 1–160 are rejected.
9. Hourly rates of zero or less are rejected.
10. The claim report is successfully exported.
11. The exported report can be read back into the console.
12. The ADO.NET record count matches the records stored in the Claims table.

---

# Conclusion

The Contract Claim Data Manager demonstrates how a C# .NET 8 console application can use Entity Framework Core and SQLite to persist and manage data.

The application provides basic claim management functionality, file processing and a direct ADO.NET database query. It also demonstrates important data-access concepts including ORM, entities, `DbContext`, `DbSet`, database providers, Code First and Database First.

