using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaundryApp
{
    public partial class ProfilAdmin : Form
    {
        public ProfilAdmin()
        {
            InitializeComponent();
        }

        private void ProfilAdmin_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT username, nama, alamat, no_hp FROM users WHERE role='admin' LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        guna2TextBoxUsername.Text = reader.GetString("username");
                        guna2TextBoxNama.Text = reader.GetString("nama");
                        guna2TextBoxAlamat.Text = reader.GetString("alamat");
                        guna2TextBoxNoHp.Text = reader.GetString("no_hp");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load profil: " + ex.Message);
            }

            guna2TextBoxUsername.ReadOnly = true;
            guna2TextBoxNama.ReadOnly = true;
            guna2TextBoxAlamat.ReadOnly = true;
            guna2TextBoxNoHp.ReadOnly = true;
            guna2TextBoxPassword.ReadOnly = true;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                // kirim data lama ke form Edit
                EditProfilAdmin editForm = new EditProfilAdmin(
                    guna2TextBoxUsername.Text,
                    guna2TextBoxNama.Text,
                    guna2TextBoxAlamat.Text,
                    guna2TextBoxNoHp.Text,
                    guna2TextBoxPassword.Text // tambahin password juga bro
                );

                this.Hide(); // sembunyikan form utama sementara
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    ProfilAdmin_Load(null, null); // reload data kalau udah disimpan
                }
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal membuka form edit profil: " + ex.Message);
            }
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
    }
}
