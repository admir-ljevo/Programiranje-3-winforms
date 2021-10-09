using cSharpIntroWinForms.P10;
using cSharpIntroWinForms.P9;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cSharpIntroWinForms.P8
{
    public partial class KorisniciPolozeniPredmeti : Form
    {
        private Korisnik korisnik;

        KonekcijaNaBazu konekcijaNaBazu = DLWMS.DB;

        int[] ocjene = { 6, 7, 8, 9, 10 };

        public KorisniciPolozeniPredmeti()
        {
            InitializeComponent();
            dgvPolozeniPredmeti.AutoGenerateColumns = false;
        }

        public KorisniciPolozeniPredmeti(Korisnik korisnik) : this()
        {
            this.korisnik = korisnik;
           
        }

        private void UcitajOcjene()
        {
            cmbOcjene.DataSource = ocjene;
        }



        private void UcitajPredmete(List<Predmeti> rezultat=null)
        {
            try
            {
                if (rezultat != null)
                {
                    cmbPredmeti.DataSource = rezultat;
                }
                else
                {
                    cmbPredmeti.DataSource = konekcijaNaBazu.Predmeti.ToList();
                }
            }
            catch (Exception err)
            {

                MessageBox.Show($"{err.Message} {err.InnerException}");
            }

            cmbPredmeti.DisplayMember = "Naziv";
            cmbPredmeti.ValueMember = "Id";
        }

        private void KorisniciPolozeniPredmeti_Load(object sender, EventArgs e)
        {
            UcitajPredmete();
            UcitajOcjene();
            UcitajPolozenePredmete();
        }

        private void UcitajPolozenePredmete()
        {
            dgvPolozeniPredmeti.DataSource = null;
            dgvPolozeniPredmeti.DataSource = korisnik.Uspjeh;
        }

        private void btnDodajPolozeni_Click(object sender, EventArgs e)
        {
            try
            {   
                if(!PostojiDupli(cmbPredmeti.SelectedItem as Predmeti))
                {

                   
                KorisniciPredmeti polozeni = new KorisniciPredmeti();
                polozeni.Predmet = cmbPredmeti.SelectedItem as Predmeti;
                polozeni.Ocjena = int.Parse(cmbOcjene.SelectedValue.ToString());
                polozeni.Datum = dtpDatumPolaganja.Value.ToString("dd.MM.yyyy");
                //polozeni.Korisnik = korisnik;
                korisnik.Uspjeh.Add(polozeni);
                konekcijaNaBazu.SaveChanges();

                MessageBox.Show($"Predmet {cmbPredmeti.SelectedItem as Predmeti} uspješno dodan.", "Obavijest" ,MessageBoxButtons.OK ,MessageBoxIcon.Information );
                UcitajPolozenePredmete();

                }
                else 
                MessageBox.Show($"Predmet {cmbPredmeti.SelectedItem as Predmeti} već postoji.", "Obavijest", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            catch (Exception err)
            {

                MessageBox.Show($"{err.Message} {err.InnerException}");
            }
        }

        private void cbUcitajNepolozene_CheckedChanged(object sender, EventArgs e)
        {
            List<Predmeti> sviPredmeti = konekcijaNaBazu.Predmeti.ToList();

            foreach (var predmet in korisnik.Uspjeh)
            {
                sviPredmeti.Remove(predmet.Predmet);
            }
            UcitajPredmete(sviPredmeti);
        }
        bool PostojiDupli(Predmeti predmeti)
        {
            for (int i = 0; i < korisnik.Uspjeh.Count(); i++)
            {
                if (korisnik.Uspjeh[i].Predmet.Naziv == predmeti.Naziv)
                    return true;
            }
            return false;
        }

        private void btnPrintajUvjerenje_Click(object sender, EventArgs e)
        {
            Form forma = new Izvjestaji(korisnik);
            forma.ShowDialog();
        }

        private void btnASYNC_Click(object sender, EventArgs e)
        {

            // Textbox, i ostale elemente moramo izvan async, inace nece thread moci prepoznati
            Predmeti odabraniPredmet = cmbPredmeti.SelectedItem as Predmeti;

            var DodavanjePredmetaTask = Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < 500; i++)
                    {

                        KorisniciPredmeti kp = new KorisniciPredmeti();

                        kp.Predmet = odabraniPredmet;
                        kp.Ocjena = 6;
                        kp.Datum = DateTime.Now.ToString("dd.MM.yyyy");

                        // Uvezivanje sa korisnikom
                        korisnik.Uspjeh.Add(kp);

                        // Spasi u bazu
                        konekcijaNaBazu.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    MboxHelper.PrikaziGresku(ex);
                }
            });

            var cekanje = DodavanjePredmetaTask.GetAwaiter();//AWAIT
            cekanje.OnCompleted(() => {
                MessageBox.Show("Uspješno je dodano 500 predmeta");
                // Refresh tabele
                UcitajPolozenePredmete();
            });
        }

    }
   
}
