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
    public partial class EditLayanan : Form
    {
        private int layananId;
        public EditLayanan(int id)
        {
            InitializeComponent();
            layananId = id;
        }


        private void EditLayanan_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            LoadDataLayanan();
        }

        private void LoadDataLayanan()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT nama_layanan, harga_per_kg, deskripsi FROM layanan WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", layananId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                guna2TextBoxNama.Text = reader["nama_layanan"].ToString();
                                guna2TextBoxHarga.Text = reader["harga_per_kg"].ToString();
                                guna2TextBoxDeskripsi.Text = reader["deskripsi"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Data layanan ga ditemukan bro 😢", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data layanan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                // ambil nilai input dari textbox
                string nama = guna2TextBoxNama.Text.Trim();
                string hargaText = guna2TextBoxHarga.Text.Trim();
                string deskripsi = guna2TextBoxDeskripsi.Text.Trim();

                // validasi dulu biar aman
                if (string.IsNullOrEmpty(nama) || string.IsNullOrEmpty(hargaText))
                {
                    MessageBox.Show("Nama dan harga wajib diisi bro!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(hargaText, out decimal harga) || harga < 0)
                {
                    MessageBox.Show("Harga harus berupa angka dan ga boleh minus bro!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // update ke database
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE layanan 
                             SET nama_layanan = @nama, 
                                 harga_per_kg = @harga, 
                                 deskripsi = @deskripsi 
                             WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama", nama);
                        cmd.Parameters.AddWithValue("@harga", harga);
                        cmd.Parameters.AddWithValue("@deskripsi", deskripsi);
                        cmd.Parameters.AddWithValue("@id", layananId);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Data layanan berhasil diupdate ✅", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK; // biar balik ke form sebelumnya dan reload data
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Gagal update data layanan 😢", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat update layanan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
