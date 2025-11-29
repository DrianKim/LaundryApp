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
    public partial class DataPelanggan : Form
    {
        DataTable dtPelanggan;
        public DataPelanggan()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, nama, alamat, no_hp FROM pelanggan";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    dtPelanggan = new DataTable();
                    da.Fill(dtPelanggan);
                    guna2DataGridView1.DataSource = dtPelanggan;

                    // 🔥 Sembunyiin kolom ID
                    guna2DataGridView1.Columns["id"].Visible = false;

                    // 🔹 Styling
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    guna2DataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 11);
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 88, 255);
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    guna2DataGridView1.EnableHeadersVisualStyles = false;
                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 255);
                    guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 200, 255);
                    guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
                    guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    guna2DataGridView1.RowHeadersVisible = false;

                    guna2DataGridView1.RowTemplate.Height = 30;
                    guna2DataGridView1.ColumnHeadersHeight = 35;

                    guna2DataGridView1.Columns["nama"].HeaderText = "Nama Pelanggan";
                    guna2DataGridView1.Columns["alamat"].HeaderText = "Alamat";
                    guna2DataGridView1.Columns["no_hp"].HeaderText = "No HP";

                    // 🔹 Tambah kolom checkbox kalau belum ada
                    if (!guna2DataGridView1.Columns.Contains("Aksi"))
                    {
                        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                        checkColumn.Name = "Aksi";
                        checkColumn.HeaderText = "Aksi";
                        checkColumn.Width = 60;
                        checkColumn.FalseValue = false;
                        checkColumn.TrueValue = true;
                        guna2DataGridView1.Columns.Add(checkColumn);
                    }

                    // 🔓 Buka akses full dulu
                    guna2DataGridView1.ReadOnly = false;

                    // 🔐 Kunci semua kecuali Aksi
                    foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                    {
                        col.ReadOnly = col.Name != "Aksi";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load data: " + ex.Message);
            }
        }


        private void DataPelanggan_Load(object sender, EventArgs e)
        {
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Left, screen.Top);
            this.Size = new Size(screen.Width, screen.Height);

            // AUTO SCALE
            this.AutoScaleMode = AutoScaleMode.Dpi; // bisa juga Font
            this.PerformAutoScale();

            LoadData();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            DashboardKasir dashboard = new DashboardKasir();
            dashboard.Show();
            this.Hide();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            DataPelanggan dataPelanggan = new DataPelanggan();
            dataPelanggan.Show();
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

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2ButtonTambah_Click(object sender, EventArgs e)
        {
            TambahPelanggan tambahPelanggan = new TambahPelanggan();
            tambahPelanggan.Show();
            this.Hide();
        }

        private void guna2ButtonEdit_Click(object sender, EventArgs e)
        {
            try
            {
                bool found = false;

                // 🔹 Loop semua row buat cari yang di-check
                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    // 🔹 Cek apakah checkbox di row ini di-check
                    bool isChecked = Convert.ToBoolean(row.Cells["Aksi"].Value);

                    if (isChecked)
                    {
                        // 🔹 Ambil data dari row yang di-check
                        int id = Convert.ToInt32(row.Cells["id"].Value);
                        string nama = row.Cells["nama"].Value.ToString();
                        string alamat = row.Cells["alamat"].Value.ToString();
                        string noHp = row.Cells["no_hp"].Value.ToString();

                        // 🔹 Pindah ke form Edit dengan passing data
                        EditPelanggan editForm = new EditPelanggan(id, nama, alamat, noHp);
                        editForm.Show();
                        this.Hide();

                        found = true;
                        break; // Stop loop setelah nemu yang di-check
                    }
                }

                // 🔹 Kalau gak ada yang di-check, kasih warning
                if (!found)
                {
                    MessageBox.Show("Pilih data pelanggan yang ingin diedit dengan mencentang checkbox pada kolom Aksi!",
                                  "Peringatan",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2ButtonHapus_Click(object sender, EventArgs e)
        {
            try
            {
                bool found = false;
                int id = 0;
                string nama = "";

                // 🔹 Loop semua row buat cari yang di-check
                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    // 🔹 Cek apakah checkbox di row ini di-check
                    bool isChecked = Convert.ToBoolean(row.Cells["Aksi"].Value);

                    if (isChecked)
                    {
                        // 🔹 Ambil ID dan nama dari row yang di-check
                        id = Convert.ToInt32(row.Cells["id"].Value);
                        nama = row.Cells["nama"].Value.ToString();
                        found = true;
                        break; // Stop loop setelah nemu yang di-check
                    }
                }

                // 🔹 Kalau gak ada yang di-check, kasih warning
                if (!found)
                {
                    MessageBox.Show("Pilih data pelanggan yang ingin dihapus dengan mencentang checkbox pada kolom Aksi!",
                                  "Peringatan",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }

                // 🔹 Konfirmasi sebelum hapus
                DialogResult result = MessageBox.Show(
                    $"Apakah Anda yakin ingin menghapus data pelanggan:\n\n{nama}?",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    DeletePelanggan(id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🔹 Method untuk hapus data pelanggan
        private void DeletePelanggan(int id)
        {
            try
            {
                using (var conn = new DatabaseHelper().GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM pelanggan WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data pelanggan berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Reload data setelah hapus
                    }
                    else
                    {
                        MessageBox.Show("Data gagal dihapus!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat menghapus data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2ButtonPesanan_Click(object sender, EventArgs e)
        {
            try
            {
                bool found = false;
                int id = 0;
                string nama = "";
                string alamat = "";
                string no_hp = "";

                // 🔹 Loop semua row buat cari yang dicentang
                foreach (DataGridViewRow row in guna2DataGridView1.Rows)
                {
                    bool isChecked = row.Cells["Aksi"].Value != null && Convert.ToBoolean(row.Cells["Aksi"].Value);

                    if (isChecked)
                    {
                        id = Convert.ToInt32(row.Cells["id"].Value);
                        nama = row.Cells["nama"].Value.ToString();
                        alamat = row.Cells["alamat"].Value.ToString();
                        no_hp = row.Cells["no_hp"].Value.ToString();
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("Pilih pelanggan dulu dengan mencentang checkbox di kolom Aksi!",
                                    "Peringatan",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Buat pesanan baru untuk pelanggan:\n\n{nama}?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // 🔥 TAMBAHKAN userId DI SINI - ambil dari session login
                    int userId = Login.UserSession.Id; // atau cara lain untuk dapatkan user ID

                    // Panggil dengan parameter yang benar - SEMUA 5 PARAMETER
                    BuatPesananBaru f = new BuatPesananBaru(userId, id, nama, alamat, no_hp);
                    f.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (dtPelanggan == null) return;

            DataView dv = dtPelanggan.DefaultView;
            dv.RowFilter = $"nama LIKE '%{guna2TextBox1.Text}%'";
        }
    }
}
