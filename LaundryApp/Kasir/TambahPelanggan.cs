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
    public partial class TambahPelanggan : Form
    {
        public TambahPelanggan()
        {
            InitializeComponent();
        }
        private void TambahPelanggan_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ProfilKasir profilKaryawan = new ProfilKasir();
            profilKaryawan.Show();
            this.Hide();
        }
        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardKasir dashBoardKaryawanForm = new DashboardKasir();
            dashBoardKaryawanForm.Show();
            this.Hide();
        }
        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataPelanggan dataPelangganForm = new DataPelanggan();
            dataPelangganForm.Show();
            this.Hide();
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            DataPesanan dataPesananForm = new DataPesanan();
            dataPesananForm.Show();
            this.Hide();
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            ProfilKasir profilKaryawan = new ProfilKasir();
            profilKaryawan.Show();
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
            // 🔹 Validasi input - pastikan semua field diisi
            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                MessageBox.Show("Nama pelanggan tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNama.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAlamat.Text))
            {
                MessageBox.Show("Alamat tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAlamat.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNoHp.Text))
            {
                MessageBox.Show("Nomor HP tidak boleh kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoHp.Focus();
                return;
            }

            // 🔹 Validasi nomor HP harus angka
            if (!txtNoHp.Text.All(char.IsDigit))
            {
                MessageBox.Show("Nomor HP harus berisi angka saja!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoHp.Focus();
                return;
            }

            try
            {
                int lastInsertedId = 0;
                string namaPelanggan = txtNama.Text.Trim();
                string alamatPelanggan = txtAlamat.Text.Trim();
                string noHpPelanggan = txtNoHp.Text.Trim();

                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();

                    // 🔹 Query INSERT dengan parameterized query (lebih aman dari SQL injection)
                    string query = "INSERT INTO pelanggan (nama, alamat, no_hp) VALUES (@nama, @alamat, @no_hp)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // 🔹 Tambahkan parameter
                    cmd.Parameters.AddWithValue("@nama", namaPelanggan);
                    cmd.Parameters.AddWithValue("@alamat", alamatPelanggan);
                    cmd.Parameters.AddWithValue("@no_hp", noHpPelanggan);

                    // 🔹 Eksekusi query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // 🔹 Ambil ID pelanggan yang baru ditambahkan (LAST_INSERT_ID)
                        lastInsertedId = (int)cmd.LastInsertedId;

                        MessageBox.Show("Data pelanggan berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 🔹 Tanya user apakah mau lanjut ke pesanan baru
                        DialogResult result = MessageBox.Show(
                            "Apakah ingin melanjutkan ke pesanan baru?",
                            "Konfirmasi",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.Yes)
                        {
                            // 🔹 Ambil userId dari session login
                            int userId = Login.UserSession.Id;

                            BuatPesananBaru pesananBaruForm = new BuatPesananBaru(userId, lastInsertedId, namaPelanggan, alamatPelanggan, noHpPelanggan);
                            pesananBaruForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            // 🔹 Clear form dan tetap di halaman Tambah Pelanggan
                            ClearForm();

                            // 🔹 Atau bisa kembali ke halaman Data Pelanggan
                            // DataPelanggan dataPelanggan = new DataPelanggan();
                            // dataPelanggan.Show();
                            // this.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtNama.Clear();
            txtAlamat.Clear();
            txtNoHp.Clear();
            txtNama.Focus();
        }


        private void guna2Button2_Click(object sender, EventArgs e)
        {
            DataPelanggan dataPelangganForm = new DataPelanggan();
            dataPelangganForm.Show();
            this.Hide();
        }
    }
}
