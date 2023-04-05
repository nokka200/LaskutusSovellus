using System.Collections.ObjectModel;

namespace LaskutusSovellus
{
    /// <summary>
    /// Luokka joka on ainoastaan lista kaikille Invoice luokille, käytetään tietosidonnassa.
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
