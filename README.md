# ExpenseTrackingApplication

A web-based application to track and manage personal expenses, built with **ASP.NET 7**, **MVC**, and **Microsoft SQL Server**.

## Features

- User Authentication & Authorization
- Add, Update, Delete Expenses
- Categorize Expenses
- View Expense History by Date Range
- Dashboard with Expense Summary and Charts
- Export Expense Data (CSV/Excel)
- Responsive UI Design
- Search and Pagination for Expense Records

## Tech Stack

- **Backend**: ASP.NET 7 (MVC)
- **Frontend**: Razor Pages, HTML5, CSS3, JavaScript
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core / Dapper (optional)

## Prerequisites

- **.NET 7 SDK**: Ensure you have .NET 7 SDK installed. [Download here](https://dotnet.microsoft.com/download).
- **SQL Server**: Microsoft SQL Server 2019 or later.
- **Visual Studio 2022** (or any IDE supporting .NET 7).
- **SQL Server Management Studio (SSMS)** (optional for managing the database).

## Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/Jaroslawx/ExpenseTrackingApplication.git
    ```

2. Navigate to the project directory:

    ```bash
    cd ExpenseTrackingApplication
    ```

3. Restore the dependencies:

    ```bash
    dotnet restore
    ```

4. Update the database connection string in `appsettings.json`:

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=your_server_name;Database=ExpenseDB;User Id=your_username;Password=your_password;"
    }
    ```

5. Apply the migrations and create the database:

    ```bash
    dotnet ef database update
    ```

6. Run the application:

    ```bash
    dotnet run
    ```

7. Open the browser and navigate to:

    ```
    http://localhost:5000
    ```

## Usage

1. **Sign Up / Log In**: Users can register and log into the system to start tracking their expenses.
2. **Add Expenses**: Navigate to the 'Add Expense' page to input details like amount, category, and description.
3. **View and Manage Expenses**: View all your expenses, search and filter them, or delete/edit any record.
4. **Dashboard**: A summary of your expenses, including a breakdown by category, and graphical representation.
5. **Export**: You can export your expenses in CSV or Excel format for offline use.

## Database Schema

The application uses the following tables:

- `Users`: Stores user login information.
- `Expenses`: Contains all the expense records, including date, amount, category, description, and user reference.
- `Categories`: Defines different categories of expenses (e.g., Food, Travel, etc.).

## Contribution

Contributions are welcome! Please follow these steps:

1. Fork the project.
2. Create a new branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -m "Add some feature"`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For any inquiries or feedback, feel free to reach out via email: `Jaroslawx@gmail.com`
