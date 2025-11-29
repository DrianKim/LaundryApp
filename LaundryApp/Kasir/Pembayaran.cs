using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaundryApp
{
    public partial class Pembayaran : Form
    {
        public decimal TotalAmount { get; private set; }
        public decimal PaidAmount { get; private set; }

        public string MetodeBayar { get; private set; }

        public Pembayaran(decimal total)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;
            this.TopMost = true;

            // 🔥 Set default otomatis ke Tunai
            MetodeBayar = "Tunai";

            TotalAmount = total;
            guna2TextBoxTotalBayar.Text = $"Rp {TotalAmount:N0}";
            guna2TextBoxTotalBayar.ReadOnly = true;
            guna2TextBoxTotalBayar.FillColor = Color.FromArgb(240, 240, 240);
            guna2TextBoxTotalBayar.BorderColor = Color.Transparent;

            guna2TextBoxTotalDibayar.Text = TotalAmount.ToString(CultureInfo.InvariantCulture);
            guna2TextBoxTotalDibayar.Focus();
        }


        private void Pembayaran_Load(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ButtonTambah_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(guna2TextBoxTotalDibayar.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal paid))
            {
                MessageBox.Show("Jumlah bayar harus berupa angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBoxTotalDibayar.Focus();
                return;
            }

            if (paid < TotalAmount)
            {
                MessageBox.Show("Jumlah bayar kurang dari total. Minta jumlah yang cukup.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBoxTotalDibayar.Focus();
                return;
            }

            PaidAmount = paid;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
