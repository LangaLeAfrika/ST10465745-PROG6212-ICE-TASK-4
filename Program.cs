using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ClaimDataManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ClaimContext())
            {
                // Create the database and Claims table if they do not exist.
                context.Database.EnsureCreated();

                // Add sample records only when the database contains no claims.
                if (!context.Claims.Any())
                {
                    context.Claims.AddRange(
                        new Claim
                        {
                            LecturerName = "Nomsa Mthembu",
                            ModuleCode = "PROG6212",
                            HoursWorked = 10,
                            HourlyRate = 350,
                            ClaimMonth = "September 2026",
                            Status = "Submitted"
                        },
                        new Claim
                        {
                            LecturerName = "Sibusiso Khumalo",
                            ModuleCode = "PROG6212",
                            HoursWorked = 6,
                            HourlyRate = 300,
                            ClaimMonth = "September 2026",
                            Status = "Draft"
                        }
                    );

                    context.SaveChanges();
                }
            }

            bool running = true;

            while (running)
            {
                Console.Clear();

                Console.WriteLine("======================================");
                Console.WriteLine("       CONTRACT CLAIM DATA MANAGER");
                Console.WriteLine("======================================");
                Console.WriteLine("1. Add Claim");
                Console.WriteLine("2. View Claims");
                Console.WriteLine("3. Update Claim Status");
                Console.WriteLine("4. Delete Claim");
                Console.WriteLine("5. Export Claims to Text File");
                Console.WriteLine("6. Run ADO.NET Record Count");
                Console.WriteLine("7. Exit");
                Console.WriteLine("======================================");

                Console.Write("Select an option: ");
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        AddClaim();
                        break;

                    case "2":
                        ViewClaims();
                        break;

                    case "3":
                        UpdateClaim();
                        break;

                    case "4":
                        DeleteClaim();
                        break;

                    case "5":
                        ExportClaims();
                        break;

                    case "6":
                        RunAdoNetCount();
                        break;

                    case "7":
                        running = false;
                        Console.WriteLine("\nThank you for using Contract Claim Data Manager.");
                        break;

                    default:
                        Console.WriteLine("\nInvalid option. Please choose 1-7.");
                        Pause();
                        break;
                }
            }
        }

        // ==========================================
        // ADD CLAIM
        // ==========================================
        static void AddClaim()
        {
            Console.Clear();
            Console.WriteLine("========== ADD CLAIM ==========\n");

            Console.Write("Lecturer Name: ");
            string lecturerName = Console.ReadLine() ?? "";

            Console.Write("Module Code: ");
            string moduleCode = Console.ReadLine() ?? "";

            decimal hoursWorked = ReadHours();

            decimal hourlyRate = ReadHourlyRate();

            Console.Write("Claim Month: ");
            string claimMonth = Console.ReadLine() ?? "";

            Claim claim = new Claim
            {
                LecturerName = lecturerName,
                ModuleCode = moduleCode,
                HoursWorked = hoursWorked,
                HourlyRate = hourlyRate,
                ClaimMonth = claimMonth,
                Status = "Draft"
            };

            using (var context = new ClaimContext())
            {
                context.Claims.Add(claim);
                context.SaveChanges();
            }

            Console.WriteLine("\nClaim added successfully.");
            Console.WriteLine($"Total Amount: R{claim.TotalAmount:F2}");

            Pause();
        }

        // ==========================================
        // VIEW CLAIMS
        // ==========================================
        static void ViewClaims()
        {
            Console.Clear();
            Console.WriteLine("========== ALL CLAIMS ==========\n");

            using (var context = new ClaimContext())
            {
                var claims = context.Claims
                    .OrderBy(c => c.ClaimId)
                    .ToList();

                if (claims.Count == 0)
                {
                    Console.WriteLine("No claim records found.");
                }
                else
                {
                    foreach (var claim in claims)
                    {
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine($"Claim ID:       {claim.ClaimId}");
                        Console.WriteLine($"Lecturer Name:  {claim.LecturerName}");
                        Console.WriteLine($"Module Code:    {claim.ModuleCode}");
                        Console.WriteLine($"Hours Worked:   {claim.HoursWorked}");
                        Console.WriteLine($"Hourly Rate:    R{claim.HourlyRate:F2}");
                        Console.WriteLine($"Claim Month:    {claim.ClaimMonth}");
                        Console.WriteLine($"Status:         {claim.Status}");
                        Console.WriteLine($"Total Amount:   R{claim.TotalAmount:F2}");
                    }

                    Console.WriteLine("--------------------------------------");
                }
            }

            Pause();
        }

        // ==========================================
        // UPDATE CLAIM
        // ==========================================
        static void UpdateClaim()
        {
            Console.Clear();
            Console.WriteLine("========== UPDATE CLAIM ==========\n");

            int claimId = ReadClaimId();

            using (var context = new ClaimContext())
            {
                Claim? claim = context.Claims.Find(claimId);

                if (claim == null)
                {
                    Console.WriteLine("\nClaim not found.");
                    Pause();
                    return;
                }

                Console.WriteLine($"\nLecturer: {claim.LecturerName}");
                Console.WriteLine($"Current Status: {claim.Status}");

                Console.Write("Enter new status: ");
                string newStatus = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(newStatus))
                {
                    Console.WriteLine("\nStatus cannot be empty.");
                    Pause();
                    return;
                }

                claim.Status = newStatus;
                context.SaveChanges();

                Console.WriteLine("\nClaim status updated successfully.");
            }

            Pause();
        }

        // ==========================================
        // DELETE CLAIM
        // ==========================================
        static void DeleteClaim()
        {
            Console.Clear();
            Console.WriteLine("========== DELETE CLAIM ==========\n");

            int claimId = ReadClaimId();

            using (var context = new ClaimContext())
            {
                Claim? claim = context.Claims.Find(claimId);

                if (claim == null)
                {
                    Console.WriteLine("\nClaim not found.");
                    Pause();
                    return;
                }

                Console.WriteLine($"\nClaim ID: {claim.ClaimId}");
                Console.WriteLine($"Lecturer: {claim.LecturerName}");
                Console.WriteLine($"Total: R{claim.TotalAmount:F2}");

                Console.Write("\nAre you sure you want to delete this claim? (Y/N): ");
                string confirmation = Console.ReadLine() ?? "";

                if (confirmation.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    context.Claims.Remove(claim);
                    context.SaveChanges();

                    Console.WriteLine("\nClaim deleted successfully.");
                }
                else
                {
                    Console.WriteLine("\nDelete cancelled.");
                }
            }

            Pause();
        }

        // ==========================================
        // EXPORT CLAIMS
        // ==========================================
        static void ExportClaims()
        {
            Console.Clear();
            Console.WriteLine("========== EXPORT CLAIMS ==========\n");

            string reportsDirectory = Path.Combine(
                AppContext.BaseDirectory,
                "Reports");

            Directory.CreateDirectory(reportsDirectory);

            string filePath = Path.Combine(
                reportsDirectory,
                "claim_summary.txt");

            using (var context = new ClaimContext())
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                var claims = context.Claims
                    .OrderBy(c => c.ClaimId)
                    .ToList();

                writer.WriteLine("CONTRACT CLAIM SUMMARY");
                writer.WriteLine("======================");
                writer.WriteLine();

                foreach (var claim in claims)
                {
                    writer.WriteLine($"Claim ID: {claim.ClaimId}");
                    writer.WriteLine($"Lecturer Name: {claim.LecturerName}");
                    writer.WriteLine($"Module Code: {claim.ModuleCode}");
                    writer.WriteLine($"Hours Worked: {claim.HoursWorked}");
                    writer.WriteLine($"Hourly Rate: R{claim.HourlyRate:F2}");
                    writer.WriteLine($"Claim Month: {claim.ClaimMonth}");
                    writer.WriteLine($"Status: {claim.Status}");
                    writer.WriteLine($"Total Amount: R{claim.TotalAmount:F2}");
                    writer.WriteLine("--------------------------------------");
                }
            }

            Console.WriteLine($"Report created successfully:");
            Console.WriteLine(filePath);

            Console.WriteLine("\n========== REPORT CONTENT ==========\n");

            string reportContents = File.ReadAllText(filePath);

            Console.WriteLine(reportContents);

            Pause();
        }

        // ==========================================
        // DIRECT ADO.NET QUERY
        // ==========================================
        static void RunAdoNetCount()
        {
            Console.Clear();
            Console.WriteLine("========== ADO.NET RECORD COUNT ==========\n");

            string connectionString = "Data Source=claims.db";

            using (SqliteConnection connection =
                   new SqliteConnection(connectionString))
            {
                connection.Open();

                using (SqliteCommand command =
                       connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT COUNT(*) FROM Claims";

                    object? result = command.ExecuteScalar();

                    long count = Convert.ToInt64(result);

                    Console.WriteLine(
                        $"Number of claim records: {count}");
                }
            }

            Pause();
        }

        // ==========================================
        // VALIDATE HOURS
        // ==========================================
        static decimal ReadHours()
        {
            while (true)
            {
                Console.Write("Hours Worked (1-160): ");
                string input = Console.ReadLine() ?? "";

                if (decimal.TryParse(input, out decimal hours))
                {
                    if (hours >= 1 && hours <= 160)
                    {
                        return hours;
                    }
                }

                Console.WriteLine(
                    "Invalid hours. Enter a value between 1 and 160.");
            }
        }

        // ==========================================
        // VALIDATE HOURLY RATE
        // ==========================================
        static decimal ReadHourlyRate()
        {
            while (true)
            {
                Console.Write("Hourly Rate: ");
                string input = Console.ReadLine() ?? "";

                if (decimal.TryParse(input, out decimal rate))
                {
                    if (rate > 0)
                    {
                        return rate;
                    }
                }

                Console.WriteLine(
                    "Invalid hourly rate. Enter a value greater than zero.");
            }
        }

        // ==========================================
        // READ CLAIM ID
        // ==========================================
        static int ReadClaimId()
        {
            while (true)
            {
                Console.Write("Enter Claim ID: ");
                string input = Console.ReadLine() ?? "";

                if (int.TryParse(input, out int claimId)
                    && claimId > 0)
                {
                    return claimId;
                }

                Console.WriteLine(
                    "Invalid Claim ID. Please enter a positive number.");
            }
        }

        // ==========================================
        // PAUSE
        // ==========================================
        static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
