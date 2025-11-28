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
    public partial class EditPelanggan : Form
    {
        // 🔹 Variable buat nyimpen data pelanggan yang mau di-edit
        private int pelangganId;
        private string pelangganNama;
        private string pelangganAlamat;
        private string pelangganNoHp;

        // 🔹 Constructor default (dipake sama designer)
        public EditPelanggan()
        {
            InitializeComponent();
        }

        // 🔹 Constructor dengan parameter (dipanggil dari DataPelanggan)
        public EditPelanggan(int id, string nama, string alamat, string noHp) : this()
        {
            this.pelangganId = id;
            this.pelangganNama = nama;
            this.pelangganAlamat = alamat;
            this.pelangganNoHp = noHp;
        }

        private void EditPelanggan_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.PerformAutoScale();

            // 🔹 Populate TextBox dengan data yang dikirim dari DataPelanggan
            PopulateData();
        }

        // 🔹 Method untuk populate TextBox dengan data pelanggan
        private void PopulateData()
        {
            txtNama.Text = pelangganNama;
            txtAlamat.Text = pelangganAlamat;
            txtNoHp.Text = pelangganNoHp;
        }

        // 🔹 Button SIMPAN - Update data pelanggan
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
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();

                    // 🔹 Query UPDATE dengan parameterized query (aman dari SQL injection)
                    string query = "UPDATE pelanggan SET nama = @nama, alamat = @alamat, no_hp = @no_hp WHERE id = @id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // 🔹 Tambahkan parameter
                    cmd.Parameters.AddWithValue("@nama", txtNama.Text.Trim());
                    cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text.Trim());
                    cmd.Parameters.AddWithValue("@no_hp", txtNoHp.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", pelangganId);

                    // 🔹 Eksekusi query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data pelanggan berhasil diupdate!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 🔹 Kembali ke halaman Data Pelanggan
                        DataPelanggan dataPelanggan = new DataPelanggan();
                        dataPelanggan.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Data gagal diupdate!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat mengupdate data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🔹 Button BATAL - Tutup form tanpa save
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            DataPelanggan dataPelanggan = new DataPelanggan();
            dataPelanggan.Show();
            this.Close();
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
            DataPesanan dataPesananForm = new DataPesanan();
            dataPesananForm.Show();
            this.Hide();
        }
    }
}
