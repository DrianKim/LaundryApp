using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace LaundryApp
{
    public partial class Struk : Form
    {
        int Id;
        string Pelanggan, Layanan, Tambahan, Metode;
        decimal TotalKg, TotalHarga;
        DateTime Dibuat;

        public Struk(long id)
        {
            InitializeComponent();
            LoadDataById(id);
        }

        private void LoadDataById(long id)
        {
            using (var conn = new DatabaseHelper().GetConnection())
            {
                conn.Open();

                string sql = @"SELECT p.id, pel.nama AS pelanggan, l.nama_layanan AS layanan,
                       p.total_pesanan, p.total_harga, p.tambahan,
                       p.metode_bayar, p.dibuat_pada
                       FROM pesanan p
                       JOIN pelanggan pel ON p.pelanggan_id = pel.id
                       JOIN layanan l ON p.layanan_id = l.id
                       WHERE p.id = @id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        Id = dr.GetInt32("id");
                        Pelanggan = dr.GetString("pelanggan");
                        Layanan = dr.GetString("layanan");
                        TotalKg = dr.GetDecimal("total_pesanan");
                        TotalHarga = dr.GetDecimal("total_harga");
                        Tambahan = dr.GetString("tambahan");
                        Metode = dr.GetString("metode_bayar");
                        Dibuat = dr.GetDateTime("dibuat_pada");
                    }
                }
            }
        }

        private void Struk_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;

            labelStruk.AutoSize = false;
            labelStruk.Width = 400; 
            labelStruk.Font = new Font("Consolas", 12);
            labelStruk.TextAlign = ContentAlignment.TopLeft;
            labelStruk.Padding = new Padding(10);

            labelStruk.Text = GenerateStrukText();
            labelStruk.Visible = true;

            Size textSize = TextRenderer.MeasureText(
                labelStruk.Text,
                labelStruk.Font,
                new Size(labelStruk.Width, int.MaxValue),
                TextFormatFlags.WordBreak
            );

            labelStruk.Height = textSize.Height + 20;

            this.Width = labelStruk.Left + labelStruk.Width + 40;
            this.Height = labelStruk.Top + labelStruk.Height + 80;
        }

        private string GenerateStrukText()
        {
            string header = "===== STRUK LAUNDRY =====";
            string footer = "===========================";

            // Biar kelihatan center (manual center)
            string Center(string text)
            {
                int totalWidth = 30; // lebar struk
                int pad = (totalWidth - text.Length) / 2;
                if (pad < 0) pad = 0;
                return new string(' ', pad) + text;
            }

            // kiri rata, kanan wrap tanpa nabrak
            string Row(string label, string value)
            {
                return $"{label.PadRight(15)} : {value}";
            }

            return
                Center(header) + "\n\n" +
                Row("ID Pesanan", Id.ToString()) + "\n" +
                Row("Pelanggan", Pelanggan) + "\n" +
                Row("Layanan", Layanan) + "\n" +
                Row("Total KG", TotalKg.ToString()) + "\n" +
                Row("Tambahan", Tambahan) + "\n" +
                Row("Metode", Metode) + "\n" +
                Row("Total Harga", $"Rp {TotalHarga:N0}") + "\n" +
                Row("Tanggal", Dibuat.ToString("dd MMM yyyy - HH:mm")) + "\n\n" +
                "Terima kasih telah menggunakan layanan kami 😄\n" +
                Center(footer) + "\n";
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = printDoc;
            preview.Width = 600;
            preview.Height = 800;

            preview.ShowDialog();
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            string text = GenerateStrukText();
            Font font = new Font("Consolas", 12);
            e.Graphics.DrawString(text, font, Brushes.Black, new PointF(20, 20));
        }

        private void labelStruk_Click(object sender, EventArgs e)
        {

        }

        private void labelStruk_Click_1(object sender, EventArgs e)
        {

        }
    }
}
