using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaundryApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // bikin path rounded buat btnLogin
            GraphicsPath path = new GraphicsPath();
            int radius = 35;

            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btnLogin.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btnLogin.Width - radius, btnLogin.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btnLogin.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();

            btnLogin.Region = new Region(path); 

            pictureBox2.Parent = pictureBox1;
            pictureBox2.BackColor = Color.Transparent;

            //btnTogglePassword.Parent = txtPassword;
            //btnTogglePassword.BackColor = Color.Transparent;
            txtPassword.UseSystemPasswordChar = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ayo_mulai_Click(object sender, EventArgs e)
        {

        }

        private void username_Click(object sender, EventArgs e)
        {

        }

        private void password_Click(object sender, EventArgs e)
        {

        }

        private void login_Click(object sender, EventArgs e)
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
                    string query = "SELECT * FROM users WHERE username=@username AND password=@password LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string role = reader["role"].ToString();

                        if (role == "pemilik")
                        {
                            DashboardAdmin admin = new DashboardAdmin();
                            this.Hide();
                            admin.ShowDialog();
                            this.Show();
                        }
                        else if (role == "karyawan")
                        {
                            DashboardKasir karyawan = new DashboardKasir();
                            this.Hide();
                            karyawan.ShowDialog();
                            this.Show();
                        }
                        else
                        {
                            MessageBox.Show("Role tidak dikenali!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username atau password salah!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

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

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}