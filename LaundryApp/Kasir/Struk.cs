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

        public Struk(int id, string pelanggan, string layanan,
                     decimal totalKg, decimal totalHarga,
                     string tambahan, string metode, DateTime dibuat)
        {
            InitializeComponent();

            Id = id;
            Pelanggan = pelanggan;
            Layanan = layanan;
            TotalKg = totalKg;
            TotalHarga = totalHarga;
            Tambahan = tambahan;
            Metode = metode;
            Dibuat = dibuat;

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Struk_Load(object sender, EventArgs e)
        {
            labelStruk.AutoSize = false;
            labelStruk.Width = 400; // WAJIB! Biar text bisa tampil
            labelStruk.Font = new Font("Consolas", 12);
            labelStruk.TextAlign = ContentAlignment.TopLeft;
            labelStruk.Padding = new Padding(10);

            labelStruk.Text = GenerateStrukText();
            labelStruk.Visible = true;

            // Auto hitung tinggi text
            Size textSize = TextRenderer.MeasureText(
                labelStruk.Text,
                labelStruk.Font,
                new Size(labelStruk.Width, int.MaxValue),
                TextFormatFlags.WordBreak
            );

            labelStruk.Height = textSize.Height + 20;

            // Auto adjust ukuran form
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
