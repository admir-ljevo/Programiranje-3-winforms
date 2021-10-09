using cSharpIntroWinForms.P10;
using cSharpIntroWinForms.P8;
using cSharpIntroWinForms.P9;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cSharpIntroWinForms
{
    public partial class KorisniciAdmin : Form
    {

        KonekcijaNaBazu konekcijaNaBazu = DLWMS.DB;
        string[] godineStudija = { "SVI", "1", "2", "3" };
        string[] aktivnosti = { "SVE", "Aktivan", "Neaktivan" };
        public KorisniciAdmin()
        {
            InitializeComponent();
            dgvKorisnici.AutoGenerateColumns = false;
        }

        private void KorisniciAdmin_Load(object sender, EventArgs e)
        {
            LoadData();
            comboBox1.DataSource = godineStudija.ToList();
            comboBox2.DataSource= aktivnosti.ToList();

        }

        private void LoadData(List<Korisnik> korisnici = null)
        {
            try
            {
                List<Korisnik> rezultati = korisnici ?? konekcijaNaBazu.Korisnici.ToList();

                dgvKorisnici.DataSource = null;
                dgvKorisnici.DataSource = rezultati;

            }
            catch (Exception ex)
            {
                MboxHelper.PrikaziGresku(ex);
            }
        }

        private void txtPretraga_TextChanged(object sender, EventArgs e)
        {
            List<Korisnik> rezultat = new List<Korisnik>();
            string filter = txtPretraga.Text.ToLower();
            foreach (var korisnik in konekcijaNaBazu.Korisnici)
            {
                if (korisnik.Ime.ToLower().Contains(filter) || korisnik.Prezime.ToLower().Contains(filter))
                    rezultat.Add(korisnik as Korisnik);
            }
            dgvKorisnici.DataSource = null;
            dgvKorisnici.DataSource = rezultat;
        }

        private void dgvKorisnici_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Korisnik korisnik = dgvKorisnici.SelectedRows[0].DataBoundItem as Korisnik;
            Form forma = null;
            if (korisnik != null)
            {
                if (e.ColumnIndex == 5)
                {
                    forma = new KorisniciPolozeniPredmeti(korisnik);
                }
                
            }
            forma.ShowDialog();
            LoadData();


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
