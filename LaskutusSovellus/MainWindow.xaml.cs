using Microsoft.VisualBasic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

/*

TO_DO


IN_PROGRESS


DONE
UI
Ui Tallennus napin lisääminen LaskutusView 
Ui Poisto napin lisääminen LaskutusView 

Kaikkien laskutietojen hakeminen ja listaaminen
Kaikkien tuotetietojen hakeminen listaaminen
Kaikkien laskujen lisätietojen hakeminen ja listaaminen
Kaikkien tuotetietojen (Product) taulun tietojen hakeminen
Tietokannan tyhjennäs ja luonti kun ohjelma käynnistyy

LaskutusView Tallennus napin toiminta Invoice tietojen osalta
LaskutusView Tallennus napin toiminta Invoice.Details uusien tietojen osalta
LaskutusView Tallennus napin toiminta Invoice.Details vanhojen tietojen päivitys
LaskutusView Details rivin poistaminen
LaskutusView Details kokonaishinta

MainWindow yhden laskun poisto 
MainWidow uuden laskun lisääminen

Yksittäisen laskun tietojen ylläpito (lisäys, muutos poisti)
Yksittäisen tuotteen tietojen ylläpito (lisäys, muutos poisti)

Bonus
MENU
Close toiminto lisätty


*/
namespace LaskutusSovellus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InvoiceRepository repoObj;  // Hoitaa tietokannan kanssa juttelemisen
        InvoiceHolder holderObj;    // collection joka pitää sisällä kaikki Invoice oliot

        public MainWindow()
        {
            InitializeComponent();

            repoObj = new();
            holderObj = new();
            DropAndCreateDb();
            InsertDefaultData();

            holderObj.Invoices = repoObj.GetInvoices();
            DataContext = holderObj;
            DtgMain.ItemsSource = holderObj.Invoices;
        }

        private void DropAndCreateDb()
        {
            // Tämä metodi tiputtaa ja luo alustavan tietokannana testailua varten
            repoObj.DropAndCreateProjectDb();
            repoObj.CreateInvoiceTable();
            repoObj.CreateProductTable();
            repoObj.CreateLaskunRivitTable();
        }

        private void InsertDefaultData()
        {
            // Tämä metodi lisää oletus datan tietokantaan
            repoObj.AddDefaultInvoice();
            repoObj.AddDefaultProduct();
            repoObj.AddDefaultLaskunRivit();
        }

        private void Btn_OpenInformationWindow(object sender, RoutedEventArgs e)
        {
            // Otetaan valittu rivi talteen josta avataan lisätiedot
            int re = DtgMain.SelectedIndex;
            if(re == -1)
            {
                MessageBox.Show("Riviä ei ole valittu");
                return;
            }

            LaskutusView laskutusView = new(re);
            bool? showDialogRe = laskutusView.ShowDialog();

            if (!showDialogRe == true)
            {
                UpdateMainWindow();
            }
            
        }

        private void Btn_DeleteInformationWindow(object sender, RoutedEventArgs e)
        {
            // Poistaa valitun rivin
            Invoice re;
            try
            {
                re = (Invoice)DtgMain.SelectedItem;
                if (re != null)
                {
                    var check = MessageBox.Show("Halutako varmasti poistaa?", "Poisto", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (check == MessageBoxResult.Yes)
                    {
                        repoObj.DeleteInvoice(re.Id);
                        // poistaa tiedot taulusta mutta ei päivitä käyttöliittymää, liittyy varmaankin DataBindingiin 
                        UpdateMainWindow();
                        // tämä kanittaa jotenkin DataContex ei päivitä ruuta ilman että itemSources lisätään tähän
                    }

                }
                else
                {
                    MessageBox.Show("Valitse lasku joka poistetaan");
                }
            } 
            catch(System.InvalidCastException)
            {
                MessageBox.Show("Tyhjä rivi valittu");
            }
        }

        private void UpdateMainWindow()
        {
            // päivittää ikkunan tiedot
            holderObj.Invoices = repoObj.GetInvoices();
            DataContext = holderObj;
            DtgMain.ItemsSource = holderObj.Invoices;
        }

        private void Btn_SaveNewInvoice(object sender, RoutedEventArgs e)
        {
            // lisää uuden tyhjän laskun
            repoObj.AddInvoice();
            UpdateMainWindow();
        }

        private void Btn_OpenAllProducts(object sender, RoutedEventArgs e)
        {
            // Avaa kaikki kaikki tuotetiedot eli Product table
            ProductView prodWin = new();
            prodWin.ShowDialog();
        }

        private void MenClose(object sender, RoutedEventArgs e)
        {
            // sulkee sovelluksen
            Close();
        }

        private void OpenManual(object sender, RoutedEventArgs e)
        {
            ManualView manView = new();
            manView.ShowDialog();

        }
    }
}
