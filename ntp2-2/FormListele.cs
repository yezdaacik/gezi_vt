
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ntp2_2
{
    public partial class FormListele : Form
    {
        string baglanti = "Server=localhost;Database=gezi;Uid=root;Pwd='';";
        string yeniAd;
        public FormListele()
        {
            InitializeComponent();
        }

        private void FormListele_Load(object sender, EventArgs e)
        {
            string klasorYolu = @"poster";
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
            }
            DgwDoldur();
           
        }

        void DgwDoldur()
        {
            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                string sorgu = "SELECT * FROM yerler;";

                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                dgvListe.DataSource = dt;

            }
        }

        private void dgvListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvListe.SelectedRows.Count > 0)
            {
                txtAd.Text = dgvListe.SelectedRows[0].Cells["ad"].Value.ToString();
                txtKonum.Text = dgvListe.SelectedRows[0].Cells["konum"].Value.ToString();
                txtAciklama.Text = dgvListe.SelectedRows[0].Cells["aciklama"].Value.ToString();             
                chkUcretli.Checked = Convert.ToBoolean(dgvListe.SelectedRows[0].Cells["ucretli"].Value);

                string dosyaYolu = Path.Combine(Environment.CurrentDirectory, "poster", dgvListe.SelectedRows[0].Cells["resim"].Value.ToString());

                pbResim.ImageLocation = null;

                if (File.Exists(dosyaYolu))
                {
                    pbResim.ImageLocation = dosyaYolu;
                    pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
                }


            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            string sorgu = " SELECT * FROM yerler WHERE ad LIKE @aranan;";


            using (MySqlConnection baglan = new MySqlConnection(baglanti))
            {
                baglan.Open();
                MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                cmd.Parameters.AddWithValue("@aranan", "%" + txtAra.Text + "%");
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvListe.DataSource = dt;
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            DataGridViewRow dr = dgvListe.SelectedRows[0];

            int satirId = Convert.ToInt32(dr.Cells[0].Value);

            DialogResult cevap = MessageBox.Show("geziyi silmek istediğinizden emin misiniz?",
                                                 "geziyi sil", MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Error);

            if (cevap == DialogResult.Yes)
            {

                using (MySqlConnection baglan = new MySqlConnection(baglanti))
                {
                    int gezi_no = Convert.ToInt32(dgvListe.SelectedRows[0].Cells["id"].Value);
                    baglan.Open();
                    string sorgu = "DELETE FROM yerler WHERE id=@id;";
                    MySqlCommand cmd = new MySqlCommand(sorgu, baglan);
                    cmd.Parameters.AddWithValue("@id", gezi_no);
                    cmd.ExecuteNonQuery();

                    DgwDoldur();
                }
            }
        }

        private void pbResim_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog(this);

            if (result != DialogResult.OK) return;

            string kaynakDosya = openFileDialog.FileName;
            yeniAd = Guid.NewGuid().ToString() + Path.GetExtension(kaynakDosya);
            string hedefDosya = Path.Combine(Environment.CurrentDirectory, "poster", yeniAd);

            File.Copy(kaynakDosya, hedefDosya);

            pbResim.Image = null;

            if (File.Exists(hedefDosya))
            {
                pbResim.Image = Image.FromFile(hedefDosya);
                pbResim.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
