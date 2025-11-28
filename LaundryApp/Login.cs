using LaundryApp.Owner;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaundryApp
{
    public partial class Login : Form
    {
        public static string RoleUser;
        public static class UserSession
        {
            public static int Id;
            public static string Username;
            public static string Nama;
            public static string Email;
            public static string Alamat;
            public static string NoHP;
            public static string Password;
            public static string Role;
        }

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username == "" || password == "")
            {
                MessageBox.Show("Username dan password wajib diisi!");
                return;
            }

            string connStr = "server=localhost;user=root;database=laundry_db;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM users WHERE username = @username LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["password"].ToString();
                            string inputHash = ComputeSha256Hash(password);

                            if (string.Equals(storedHash, inputHash, StringComparison.OrdinalIgnoreCase))
                            {
                                // simpan session
                                UserSession.Id = Convert.ToInt32(reader["id"]);
                                UserSession.Username = reader["username"].ToString();
                                UserSession.Nama = reader["nama"].ToString();
                                UserSession.Email = reader["email"].ToString();
                                UserSession.Alamat = reader["alamat"].ToString();
                                UserSession.NoHP = reader["no_hp"].ToString();
                                UserSession.Password = reader["password"].ToString();
                                UserSession.Role = reader["role"].ToString();

                                string role = reader["role"].ToString();

                                this.Hide();

                                // === ROLE HANDLER ===
                                if (role == "admin")
                                {
                                    DashboardAdmin admin = new DashboardAdmin();
                                    admin.FormClosed += (s, args) => Application.Exit();
                                    admin.Show();
                                }
                                else if (role == "karyawan")
                                {
                                    DashboardKasir karyawan = new DashboardKasir();
                                    karyawan.FormClosed += (s, args) => Application.Exit();
                                    karyawan.Show();
                                }
                                else if (role == "owner")   // <<< TAMBAHAN BARU
                                {
                                    DashboardOwner owner = new DashboardOwner();
                                    owner.FormClosed += (s, args) => Application.Exit();
                                    owner.Show();
                                }
                                else
                                {
                                    MessageBox.Show("Role tidak dikenali!");
                                    this.Show();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Username atau password salah!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Username atau password salah!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;

            if (txtPassword.UseSystemPasswordChar)
            {
                btnTogglePassword.Text = "👁️";
            }
            else
            {
                btnTogglePassword.Text = "🚫";
            }
        }
    }
}
