using Bluube.Auth;

namespace LoginC_;

public partial class Login : Form
{
    private readonly BluubeAuth _app;

    public Login(BluubeAuth app)
    {
        InitializeComponent();
        _app = app;
    }

    private void register_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        loginPanel.Visible = true;
        loginRegister.Visible = false;
    }

    private void login_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        loginRegister.Visible = true;
        loginPanel.Visible = false;
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        btnLogin.Enabled = false;
        try
        {
            var ok = await _app.Login(username.Text.Trim(), password.Text);
            if (!ok)
            {
                MessageBox.Show(_app.LastMessage ?? "Login failed.", "Bluube Auth", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var home = new Home();
            home.FormClosed += (_, _) => Close();
            Hide();
            home.Show();
        }
        finally
        {
            btnLogin.Enabled = true;
        }
    }

    private async void btnRegister_Click(object sender, EventArgs e)
    {
        btnRegister.Enabled = false;
        try
        {
            if (string.IsNullOrWhiteSpace(license_register.Text) ||
                string.IsNullOrWhiteSpace(username_register.Text) ||
                string.IsNullOrWhiteSpace(password_register.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Bluube Auth", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ok = await _app.Register(
                license_register.Text.Trim(),
                username_register.Text.Trim(),
                password_register.Text);

            if (!ok)
            {
                MessageBox.Show(_app.LastMessage ?? "Registration failed.", "Bluube Auth", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var home = new Home();
            home.FormClosed += (_, _) => Close();
            Hide();
            home.Show();
        }
        finally
        {
            btnRegister.Enabled = true;
        }
    }
}
