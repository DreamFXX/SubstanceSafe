var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
        {
    app.MapOpenApi();
        }

app.UseHttpsRedirection();

            var totalAmount = usages.Sum(u => u.Amount);
            var days = (DateTime.Now - usages.Min(u => u.UsageDate)).TotalDays;
            return days > 0 ? totalAmount / (decimal)days : 0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tracker = new SubstanceTracker();
            string command;

            while (true)
            {
                Console.WriteLine("\nSubstance Usage Tracker Menu:");
                Console.WriteLine("1. Log new usage");
                Console.WriteLine("2. View usage history");
                Console.WriteLine("3. View statistics");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");
                command = Console.ReadLine();

                switch (command)
                {
                    case "1":
                        LogNewUsage(tracker);
                        break;
                    case "2":
                        ViewUsageHistory(tracker);
                        break;
                    case "3":
                        ViewStatistics(tracker);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void LogNewUsage(SubstanceTracker tracker)
        {
            Console.Write("Enter substance name: ");
            string substance = Console.ReadLine();
            Console.Write("Enter amount: ");
            int amount = int.Parse(Console.ReadLine());
            Console.Write("Enter unit (e.g., mg, ml): ");
            string unit = Console.ReadLine();
            Console.Write("Enter notes (optional): ");
            string notes = Console.ReadLine();

            tracker.LogUsage(substance, amount, unit, notes);
            Console.WriteLine("Usage logged successfully!");
        }

        static void ViewUsageHistory(SubstanceTracker tracker)
        {
            Console.WriteLine("\nUsage History:");
            var history = tracker.GetUsageHistory();
            
            if (!history.Any())
            {
                Console.WriteLine("No usage history found.");
                return;
            }

            foreach (var usage in history)
            {
                Console.WriteLine($"{usage.UsageDate:yyyy-MM-dd HH:mm} - {usage.Substance}: {usage.Amount} {usage.Unit}");
                if (!string.IsNullOrEmpty(usage.Notes))
                {
                    Console.WriteLine($"  Notes: {usage.Notes}");
                }
            }
        }

        static void ViewStatistics(SubstanceTracker tracker)
        {
            Console.WriteLine("\nStatistics:");
            var avgUsage = tracker.GetAverageUsagePerDay();
            Console.WriteLine($"Average daily usage: {avgUsage:F2}");
        }
    }
}
