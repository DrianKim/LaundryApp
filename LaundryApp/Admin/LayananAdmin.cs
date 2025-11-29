using LaundryApp.Admin;
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
    public partial class LayananAdmin : Form
    {
        public LayananAdmin()
        {
            InitializeComponent();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LayananAdmin_Load(object sender, EventArgs e)
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

                    // JOIN kategori biar bisa muncul nama + tipe
                    string query = @"
                SELECT 
                    l.id,
                    l.nama_layanan,
                    l.harga,
                    l.deskripsi,
                    l.kategori_id,
                    CONCAT(k.name, ' (', k.tipe, ')') AS kategori_lengkap
                FROM layanan l
                LEFT JOIN kategori k ON k.id = l.kategori_id
                ORDER BY l.id DESC";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dtLayanan = new DataTable();
                    da.Fill(dtLayanan);

                    guna2DataGridView1.DataSource = dtLayanan;

                    // Boleh diedit, nanti dikunci
                    guna2DataGridView1.ReadOnly = false;

                    // Rename header
                    guna2DataGridView1.Columns["id"].HeaderText = "ID";
                    guna2DataGridView1.Columns["nama_layanan"].HeaderText = "Nama Layanan";
                    guna2DataGridView1.Columns["harga"].HeaderText = "Harga (Rp)";
                    guna2DataGridView1.Columns["deskripsi"].HeaderText = "Deskripsi";
                    guna2DataGridView1.Columns["kategori_lengkap"].HeaderText = "Kategori";

                    // Sembunyikan id & kategori_id (internal)
                    guna2DataGridView1.Columns["id"].Visible = false;
                    guna2DataGridView1.Columns["kategori_id"].Visible = false;

                    // Checkbox Aksi
                    if (!guna2DataGridView1.Columns.Contains("Aksi"))
                    {
                        DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                        checkColumn.Name = "Aksi";
                        checkColumn.HeaderText = "Aksi";
                        checkColumn.Width = 60;
                        checkColumn.TrueValue = true;
                        checkColumn.FalseValue = false;
                        guna2DataGridView1.Columns.Add(checkColumn);
                    }

                    // Kunci semua kolom kecuali Aksi
                    foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                    {
                        col.ReadOnly = col.Name != "Aksi";
                    }
                }

                // STYLING
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data layanan: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void guna2ButtonTambah_Click(object sender, EventArgs e)
        {
            using (var tambahForm = new TambahLayanan())
            {
                if (tambahForm.ShowDialog() == DialogResult.OK)
                {
                    LoadDataLayanan(); 
                }
            }
        }

        private void guna2ButtonEdit_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["ID"].Value);

                using (var editForm = new EditLayanan(id))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadDataLayanan(); 
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data layanan dulu bro 😎", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2ButtonHapus_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["id"].Value);
                string namaLayanan = guna2DataGridView1.CurrentRow.Cells["nama_layanan"].Value.ToString();

                var confirm = MessageBox.Show(
                    $"Yakin mau hapus layanan '{namaLayanan}'?",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        using (var conn = new DatabaseHelper().GetConnection())
                        {
                            conn.Open();
                            string query = "DELETE FROM layanan WHERE id = @id";
                            using (var cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@id", id);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show($"Layanan '{namaLayanan}' berhasil dihapus ✅",
                            "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadDataLayanan(); // refresh tabel
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal hapus layanan: " + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih layanan dulu sebelum hapus bro 😅",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            KategoriAdmin kategoriAdmin = new KategoriAdmin();
            kategoriAdmin.Show();
            this.Hide();
        }
    }
}
