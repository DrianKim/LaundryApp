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

namespace LaundryApp.Admin
{
    public partial class EditKategori : Form
    {
        private int kategoriId;

        public EditKategori(int id)
        {
            InitializeComponent();
            kategoriId = id;
        }

        private void EditKategori_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            LoadDataKategori();
        }

        private void LoadDataKategori()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT name, tipe FROM kategori WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", kategoriId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                guna2TextBoxNamaKategori.Text = reader["name"].ToString();
                                guna2TextBoxTipe.Text = reader["tipe"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Kategori tidak ditemukan 😢", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data kategori: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2TextBoxNamaKategori_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBoxTipe_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string nama = guna2TextBoxNamaKategori.Text.Trim();
            string tipe = guna2TextBoxTipe.Text.Trim();

            if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(tipe))
            {
                MessageBox.Show("Nama kategori dan tipe wajib diisi bro 😅", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE kategori 
                                     SET name = @name,
                                         tipe = @tipe,
                                         updated_at = NOW()
                                     WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", nama);
                        cmd.Parameters.AddWithValue("@tipe", tipe);
                        cmd.Parameters.AddWithValue("@id", kategoriId);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Kategori berhasil diupdate 🔥", "Sukses",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Update kategori gagal 😢", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error update kategori: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            KategoriAdmin kategoriAdmin = new KategoriAdmin();
            kategoriAdmin.Show();
            this.Hide();
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

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            ProfilAdmin profilAdmin = new ProfilAdmin();
            profilAdmin.Show();
            this.Hide();
        }
    }
}
