using System.Collections.ObjectModel;

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
    /// Pitää tiedot kaikista laskuista
    /// </summary>
    public class InvoiceHolder
    {
        // käytetään databindingissa MainWindows.xaml, tästä olisi tarkoitus hakea kaikki laskut datagridiin
        public ObservableCollection<Invoice> Invoices { get; set; }

        public InvoiceHolder()
        {
            Invoices = new();
        }
    }
}
