using LaundryApp.Admin;
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
    public partial class EditProfilAdmin : Form
    {
        public EditProfilAdmin()
        {
            InitializeComponent();
        }

        private string connectionString = "server=localhost;database=laundry_db;uid=root;pwd=;";
        private string oldUsername;

        public EditProfilAdmin(string username, string nama, string alamat, string noHp, string password)
        {
            InitializeComponent();
            guna2TextBoxUsername.Text = username;
            guna2TextBoxNama.Text = nama;
            guna2TextBoxAlamat.Text = alamat;
            guna2TextBoxNoHp.Text = noHp;
            guna2TextBoxPassword.PlaceholderText = "Kosongkan jika tidak ingin mengubah password";
            oldUsername = username;
        }

        private void EditProfilAdmin_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // 🔹 Cek apakah password diubah atau enggak
                    string query;
                    MySqlCommand cmd;

                    if (string.IsNullOrWhiteSpace(guna2TextBoxPassword.Text))
                    {
                        // Kalo password kosong → gak diubah
                        query = @"UPDATE users 
                                  SET username = @username, 
                                      nama = @nama, 
                                      alamat = @alamat, 
                                      no_hp = @no_hp
                                  WHERE username = @oldUsername";

                        cmd = new MySqlCommand(query, conn);
                    }
                    else
                    {
                        // Kalo password diisi → hash & update
                        string hashedPassword = ComputeSha256Hash(guna2TextBoxPassword.Text);

                        query = @"UPDATE users 
                                  SET username = @username, 
                                      nama = @nama, 
                                      alamat = @alamat, 
                                      no_hp = @no_hp, 
                                      password = @password
                                  WHERE username = @oldUsername";

                        cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                    }

                    // Parameter umum
                    cmd.Parameters.AddWithValue("@username", guna2TextBoxUsername.Text);
                    cmd.Parameters.AddWithValue("@nama", guna2TextBoxNama.Text);
                    cmd.Parameters.AddWithValue("@alamat", guna2TextBoxAlamat.Text);
                    cmd.Parameters.AddWithValue("@no_hp", guna2TextBoxNoHp.Text);
                    cmd.Parameters.AddWithValue("@oldUsername", oldUsername);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Profil berhasil diperbarui 🔥", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan perubahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ComputeSha256Hash(string rawData)
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

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardAdmin dashboard = new DashboardAdmin();
            dashboard.Show();
            this.Hide();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataKasir dataKasir = new DataKasir();
            dataKasir.Show();
            this.Hide();
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {
            LayananAdmin layananAdmin = new LayananAdmin();
            layananAdmin.Show();
            this.Hide();
        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            ProfilAdmin profilAdmin = new ProfilAdmin();
            profilAdmin.Show();
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
            KategoriAdmin kategoriAdmin = new KategoriAdmin();
            kategoriAdmin.Show();
            this.Hide();
        }
    }
}
