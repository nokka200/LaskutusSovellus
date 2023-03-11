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

            holderObj.Invoices[Selected].Details = repoObj.GetDetails(Selected);    // Haetaan kaikki laskun lisätiedot

            DataContext = holderObj.Invoices[Selected];                             // Asetetaan haettu lasku tiedosidonnan kohteeksi
            DtgLaskutusView.ItemsSource = holderObj.Invoices[Selected].Details;     // haetaan datagridin lähde oikeaksi ContractDetail olioksi
            // Tämä vaikuttaa ouodolta, tarkkaile jos parempi vaihtoehto NOTICE

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
                /*
                holderObj.Invoices.Add(new Invoice
                {

                });
                // TODO pitäisi lisätä uusi rivi tietokantaan
                */
                Selected--;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Tämä nappi tallentaa ainoastaan Invoice luokan tietoja
            var invoce = (Invoice)DataContext;                                      // Haetaan DataContect Invoice muodossa
            repoObj.UpdateInvoice(invoce);                                          // Tehdään päivitys tietokantaan

            repoObj.UpdateDetails(invoce);                                          // UUSi feature, päivittää laskun detailsit

            holderObj.Invoices = repoObj.GetInvoices();                             // Haetaan laskun tiedot uudestaan
            holderObj.Invoices[Selected].Details = repoObj.GetDetails(Selected);    // Haetaan kaikki laskun lisätiedot

            DataContext = holderObj.Invoices[Selected];                             // päivitetään DataContext uusimmilla tiedoilla
            DtgLaskutusView.ItemsSource = holderObj.Invoices[Selected].Details;     // päivitetään myös laskun lisätiedot
        }
    }
}
