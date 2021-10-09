using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
namespace cSharpIntroWinForms.P10
{
    public partial class Izvjestaji : Form
    {
        private Korisnik korisnik;

        public Izvjestaji()
        {
            InitializeComponent();
        }

        public Izvjestaji(Korisnik korisnik): this()
        {
            this.korisnik = korisnik;
        }

        private void Izvjestaji_Load(object sender, EventArgs e)
        {
            if (korisnik != null)
            {

                ReportParameterCollection rpc = new ReportParameterCollection();
                rpc.Add(new ReportParameter("ImePrezime", $"{korisnik.Ime} {korisnik.Prezime}"));
                dtsDLWMS.tblPolozeniDataTable tbl = new dtsDLWMS.tblPolozeniDataTable();
                int i = 1;

                List<object> list = new List<object>();

                foreach (var polozeni in korisnik.Uspjeh)
                {
                    list.Add(new
                    {
                        Rb = i++,
                        Naziv = polozeni.Predmet.Naziv,
                        Ocjena = polozeni.Ocjena,
                        Datum = polozeni.Datum
                    });
                }

                ReportDataSource rds = new ReportDataSource();

                rds.Name = "dtsDLWMS";
                rds.Value = list;

                reportViewer1.LocalReport.SetParameters(rpc);
                reportViewer1.LocalReport.DataSources.Add(rds);
            }

            this.reportViewer1.RefreshReport();
        }
    }
}
