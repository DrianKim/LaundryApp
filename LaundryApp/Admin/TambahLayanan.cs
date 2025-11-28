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
    public partial class TambahLayanan : Form
    {
        public TambahLayanan()
        {
            InitializeComponent();
        }

        private void TambahLayanan_Load(object sender, EventArgs e)
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
            string nama = guna2TextBoxNama.Text.Trim();
            string hargaText = guna2TextBoxHarga.Text.Trim();
            string deskripsi = guna2TextBoxDeskripsi.Text.Trim();

            if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(hargaText))
            {
                MessageBox.Show("Nama dan harga wajib diisi bro 😅", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(hargaText, out decimal harga))
            {
                MessageBox.Show("Harga harus angka bro 🤑", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO layanan (nama_layanan, harga_per_kg, deskripsi) VALUES (@nama, @harga, @deskripsi)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama", nama);
                        cmd.Parameters.AddWithValue("@harga", harga);
                        cmd.Parameters.AddWithValue("@deskripsi", deskripsi);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Layanan baru berhasil ditambah 🔥", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal nambah layanan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
    }
}
