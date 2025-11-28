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
    public partial class EditUser : Form
    {
        private int userId;

        public EditUser(int id)
        {
            InitializeComponent();
            userId = id;
            LoadUserData();
        }

        private void LoadUserData()
        {
            using (var conn = new DatabaseHelper().GetConnection())
            {
                conn.Open();
                string query = "SELECT nama, username, email, alamat, no_hp FROM users WHERE id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", userId);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    guna2TextBoxNama.Text = reader.GetString("nama");
                    guna2TextBoxUsername.Text = reader.GetString("username");
                    guna2TextBoxEmail.Text = reader.GetString("email");
                    guna2TextBoxAlamat.Text = reader.GetString("alamat");
                    guna2TextBoxNoHp.Text = reader.GetString("no_hp");
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(guna2TextBoxNama.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxUsername.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxAlamat.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxEmail.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBoxNoHp.Text))
            {
                MessageBox.Show("Semua field wajib diisi!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query;

                    if (!string.IsNullOrWhiteSpace(guna2TextBoxPassword.Text))
                    {
                        string hashed = ComputeSha256Hash(guna2TextBoxPassword.Text);

                        query = "UPDATE users SET nama=@nama, username=@username, email=@email, alamat=@alamat, no_hp=@no_hp, password=@password WHERE id=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nama", guna2TextBoxNama.Text);
                        cmd.Parameters.AddWithValue("@username", guna2TextBoxUsername.Text);
                        cmd.Parameters.AddWithValue("@email", guna2TextBoxEmail.Text);
                        cmd.Parameters.AddWithValue("@alamat", guna2TextBoxAlamat.Text);
                        cmd.Parameters.AddWithValue("@no_hp", guna2TextBoxNoHp.Text);
                        cmd.Parameters.AddWithValue("@password", hashed);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        query = "UPDATE users SET nama=@nama, username=@username, email=@email, alamat=@alamat, no_hp=@no_hp WHERE id=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@nama", guna2TextBoxNama.Text);
                        cmd.Parameters.AddWithValue("@username", guna2TextBoxUsername.Text);
                        cmd.Parameters.AddWithValue("@email", guna2TextBoxEmail.Text);
                        cmd.Parameters.AddWithValue("@alamat", guna2TextBoxAlamat.Text);
                        cmd.Parameters.AddWithValue("@no_hp", guna2TextBoxNoHp.Text);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("User berhasil diperbarui!");
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error update: " + ex.Message);
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private void EditUser_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();
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

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            LaporanPemasukan laporanPemasukan = new LaporanPemasukan();
            laporanPemasukan.Show();
            this.Hide();
        }
    }
}
