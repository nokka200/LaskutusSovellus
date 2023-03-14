using System;
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
    public class Invoice
    {
        // Laskun tiedot haetaan databindingilla MainWindowiin Id ja AddressBiller loput haetaan LaskutusView ikkunaan
        public int Id { get; set; }
        public string? AddressDelivery { get; set; }
        public string? AddressBiller { get; set; }
        public DateTime DateBill { get; set; }
        public DateTime DateDue { get; set; }
        public string? ExtraInformation { get; set; }
        public double ProductTotal { get; set; }

        public int TimeTillDueDate { get; set; } = 14;
        public ObservableCollection<ContractDetails> Details { get; set; }

        public Invoice()
        {
            // Annetaan tämä pv laskun luomisen ja eräpäivä oletuksena 14 pv pidemmällä
            AddressDelivery = string.Empty;
            AddressBiller = string.Empty;
            ExtraInformation = "Extra";
            DateBill = DateTime.Now;
            DateDue = DateTime.Now.AddDays(TimeTillDueDate);
            Details = new();
        }
    }
}
