using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LaskutusSovellus
{
    /// <summary>
    /// Interaction logic for LaskutusView.xaml
    /// </summary>
    public partial class LaskutusView : Window
    {
        public int Selected { get; set; }

        InvoiceRepository repoObj;
        InvoiceHolder holderObj;

        public LaskutusView(int selected)
        {
            Selected = selected;
            

            InitializeComponent();

            repoObj = new();
            holderObj = new();

            holderObj.Invoices = repoObj.GetInvoices();
            CheckSelected(holderObj.Invoices.Count);

            DataContext = holderObj.Invoices[Selected];
        }

        private void CheckSelected(int max)
        {
            // jos ei ole valittu mitään asetetaan 0 arvoski ettei ohjelma kaadu
            // parannuksena luo uuden laskun, tarvii vielä rakentamista 
            if(Selected == -1)
            {
                Selected = 0;
            }
            if(Selected == max)
            {
                holderObj.Invoices.Add(new Invoice
                {

                });
                // TODO pitäisi lisätä uusi rivi tietokantaan
            }
        }
    }
}
