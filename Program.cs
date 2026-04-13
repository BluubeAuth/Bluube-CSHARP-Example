using System.Globalization;
using Bluube.Auth;
using Newtonsoft.Json.Linq;

namespace Bluube.Example;

class Program
{
    // APP_ID, OWNER_ID, VERSION
    public static readonly BluubeAuth App = new("APP_ID", "OWNER_ID", "1.0");

    static async Task Main(string[] args)
    {
        _clear();
        _header();

        if (!await App.Initialize())
        {
            System.Console.WriteLine("\n" + (App.LastMessage ?? "Initialization failed."));
            System.Console.Write("\nPress ENTER to exit...");
            System.Console.ReadLine();
            return;
        }

        while (true)
        {
            _clear();
            _header();
            System.Console.WriteLine("[1] Login (username/password)");
            System.Console.WriteLine("[2] Register (license key + username + password)");
            System.Console.WriteLine("[3] Exit");
            System.Console.Write("Select: ");

            var option = System.Console.ReadLine();
            if (string.IsNullOrEmpty(option)) continue;

            if (option == "1")
            {
                System.Console.Write("Username: ");
                var user = System.Console.ReadLine() ?? "";
                System.Console.Write("Password: ");
                var pass = System.Console.ReadLine() ?? "";

                var ok = await App.Login(user, pass);
                
                _clear();
                _header();
                System.Console.WriteLine(ok ? "Authenticated" : (App.LastMessage ?? "Login failed."));
                if (ok) _print_user_data();
            }
            else if (option == "2")
            {
                System.Console.Write("License key: ");
                var key = System.Console.ReadLine() ?? "";
                System.Console.Write("Username: ");
                var user = System.Console.ReadLine() ?? "";
                System.Console.Write("Password: ");
                var pass = System.Console.ReadLine() ?? "";

                var ok = await App.Register(key, user, pass);
                
                _clear();
                _header();
                System.Console.WriteLine(ok ? "Registered and authenticated" : (App.LastMessage ?? "Registration failed."));
                if (ok) _print_user_data();
            }
            else if (option == "3")
            {
                break;
            }
            else
            {
                System.Console.WriteLine("Invalid option.");
            }

            System.Console.Write("\nPress ENTER to continue...");
            System.Console.ReadLine();
        }
    }

    static void _clear() => System.Console.Clear();

    static void _header()
    {
        System.Console.WriteLine("BluubeAuth - C# Example");
        System.Console.WriteLine(new string('-', 32));
    }

    static void _print_user_data()
    {
        if (App.UserData == null) return;

        System.Console.WriteLine("\nUser data: ");
        System.Console.WriteLine("Username: " + (App.UserData["username"]?.ToString() ?? "Unknown"));
        System.Console.WriteLine("IP address: " + (App.UserData["ip"]?.ToString() ?? "Unknown"));
        System.Console.WriteLine("Hardware-Id: " + (App.UserData["hwid"]?.ToString() ?? "Unknown"));

        var ca = App.UserData["createdAt"]?.ToString();
        var ea = App.UserData["expiresAt"]?.ToString();

        System.Console.WriteLine("Created at: " + _format_date(ca));
        System.Console.WriteLine("Expires at: " + (string.IsNullOrEmpty(ea) ? "Lifetime" : _format_date(ea)));
        System.Console.WriteLine(new string('-', 32));
    }

    static string _format_date(string? iso_str)
    {
        if (string.IsNullOrEmpty(iso_str)) return "Unknown";
        try
        {
            if (DateTime.TryParse(iso_str, null, DateTimeStyles.RoundtripKind, out var dt))
            {
                return dt.ToLocalTime().ToString("dd/MM/yyyy - HH:mm:ss");
            }
            return iso_str;
        }
        catch { return iso_str; }
    }
}
