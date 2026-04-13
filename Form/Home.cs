using System.Globalization;

namespace LoginC_;

public partial class Home : Form
{
    public Home()
    {
        InitializeComponent();
        Load += Home_Load;
    }

    private void Home_Load(object? sender, EventArgs e)
    {
        userInfoList.Items.Clear();
        var ud = Program.App.UserData;
        if (ud == null)
        {
            userInfoList.Items.Add("No user data.");
            return;
        }

        userInfoList.Items.Add("Username: " + (ud["username"]?.ToString() ?? "Unknown"));
        userInfoList.Items.Add("IP address: " + (ud["ip"]?.ToString() ?? "Unknown"));
        userInfoList.Items.Add("Hardware-Id: " + (ud["hwid"]?.ToString() ?? "Unknown"));

        var ca = ud["createdAt"]?.ToString();
        var ea = ud["expiresAt"]?.ToString();
        userInfoList.Items.Add("Created at: " + FormatDate(ca));
        userInfoList.Items.Add("Expires at: " + (string.IsNullOrEmpty(ea) ? "Lifetime" : FormatDate(ea)));
    }

    private static string FormatDate(string? iso)
    {
        if (string.IsNullOrEmpty(iso)) return "Unknown";
        try
        {
            if (DateTime.TryParse(iso, null, DateTimeStyles.RoundtripKind, out var dt))
                return dt.ToLocalTime().ToString("dd/MM/yyyy - HH:mm:ss");
            return iso;
        }
        catch
        {
            return iso;
        }
    }
}
