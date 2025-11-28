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

namespace LaundryApp.Owner
{
    public partial class TambahUser : Form
    {
        public TambahUser()
        {
            InitializeComponent();
        }

        private void TambahUser_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Validasi input
            if (string.IsNullOrWhiteSpace(guna2TextBoxNama.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxUsername.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxAlamat.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxNoHp.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxPassword.Text))
            {
                MessageBox.Show("Semua field wajib diisi!",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO users 
                                    (nama, username, email, alamat, no_hp, role, password, created_at) 
                                    VALUES 
                                    (@nama, @username, @email, @alamat, @no_hp, @role, @password, NOW())";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    string hashedPassword = ComputeSha256Hash(guna2TextBoxPassword.Text);

                    cmd.Parameters.AddWithValue("@nama", guna2TextBoxNama.Text);
                    cmd.Parameters.AddWithValue("@username", guna2TextBoxUsername.Text);
                    cmd.Parameters.AddWithValue("@email", guna2TextBoxEmail.Text);
                    cmd.Parameters.AddWithValue("@alamat", guna2TextBoxAlamat.Text);
                    cmd.Parameters.AddWithValue("@no_hp", guna2TextBoxNoHp.Text);

                    cmd.Parameters.AddWithValue("@role", "admin");

                    cmd.Parameters.AddWithValue("@password", hashedPassword);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User admin berhasil ditambahkan!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat menyimpan: " + ex.Message);
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();

                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardOwner dashboard = new DashboardOwner();
            dashboard.Show();
            this.Hide();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataUser dataPelanggan = new DataUser();
            dataPelanggan.Show();
            this.Hide();
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox6_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show
            (
                "Apakah Anda Yakin Ingin logout?",
                "Konfirmasi Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                // Hapus session user
                Login.UserSession.Id = 0;
                Login.UserSession.Username = null;
                Login.UserSession.Nama = null;
                Login.UserSession.Email = null;
                Login.UserSession.Alamat = null;
                Login.UserSession.NoHP = null;
                Login.UserSession.Password = null;
                Login.UserSession.Role = null;

                // Buka form login lagi
                Login loginForm = new Login();
                loginForm.Show();

                // Tutup form sekarang (dashboard)
                this.Hide();
            }
        }
    }
}
