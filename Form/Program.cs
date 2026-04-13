using Bluube.Auth;

namespace LoginC_;

internal static class Program
{
    // APP_ID, OWNER_ID, VERSION
    public static readonly BluubeAuth App = new("APP_ID", "OWNER_ID", "1.0");

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (!App.Initialize().GetAwaiter().GetResult())
        {
            MessageBox.Show(App.LastMessage ?? "Initialization failed.", "Bluube Auth", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Application.Run(new Login(App));
    }
}
