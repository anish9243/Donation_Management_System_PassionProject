# Donation Management System Passion Project

## Overview
The Donation Management System Passion Project aims to facilitate the management of donations, campaigns, and donors for charitable organizations. This project utilizes ASP.NET for backend services, Entity Framework for ORM, and provides Web API endpoints for CRUD operations.

## Running this Project
### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.7.2

### Setup Steps
1. **Clone the Repository:**
- git clone [https://github.com/anish9243/Donation_Management_System_PassionProject.git]
- cd Donation_Management_System_PassionProject)

2. **Open the Project:**
- Open the solution file (e.g., `Donation_Management_System_PassionProject.sln`) in Visual Studio.

3. **Set Target Framework:**
- Project > Donation_Management_System_PassionProject Properties > Change target framework to 4.7.1 -> Change back to 4.7.2

4. **Ensure App_Data Folder Exists:**
- Right-click on the solution > View in File Explorer.
- Create the `App_Data` folder if it does not exist.

5. **Update Database:**
- Tools > NuGet Package Manager > Package Manager Console.
- Run the command: `Update-Database`

6. **Verify Database Creation:**
- View > SQL Server Object Explorer > MSSQLLocalDb > ..

### Common Issues and Resolutions
- **(update-database) Could not attach .mdf database:**
- SOLUTION: Ensure the `App_Data` folder is created in the project.

- **(update-database) Error. 'Type' cannot be null:**
- SOLUTION: Update Entity Framework to the latest version using NuGet Package Manager.

- **(update-database) System Exception: Exception has been thrown by the target of an invocation:**
- SOLUTION: Clone the project repository to the local drive (not cloud-based storage).

- **(running server) Could not find part to the path ../bin/roslyn/csc.exe:**
- SOLUTION: Change target framework to 4.7.1 and back to 4.7.2.

- **(running server) Project Failed to build. System.Web.Http does not have reference to serialize...:**
- SOLUTION: Add a reference to `System.Web.Extensions` in Solution Explorer > References.

### API Commands
- **Get a List of Donors:**
  curl https://localhost:{port}/api/donordata/listdonors

- **Get a Single Donor:**
  curl https://localhost:{port}/api/donordata/finddonor/{id}

- **Add a New Donor (data in donor.json):**
  curl -H "Content-Type
/json" -d @donor.json https://localhost:{port}/api/donordata/adddonor

- **Delete a Donor:**
curl -d "" https://localhost:{port}/api/donordata/deletedonor/{id}

- **Update a Donor (existing donor info including id in donor.json):**
curl -H "Content-Type
/json" -d @donor.json https://localhost:{port}/api/donordata/updatedonor/{id}

### Running the Views
- Use SQL Server Object Explorer to manage donors and campaigns.
- Navigate to specific endpoints (e.g., `/Donor/New`) to interact with the application's frontend.

## Additional Notes
- This project demonstrates skills in ASP.NET, Entity Framework, and Web API development.
- For troubleshooting or further assistance, contact Anish at [Email](ravipatel9243@gmail.com).
