using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

ToDo
Yksittäisen laskun tietojen ylläpito (lisäys, muutos poisti)
Yksittäisen tuotteen tietojen ylläpito (lisäys, muutos poisti)


InProgress
Tietokannan tyhjennäs ja luonti kun ohjelma käynnistyy

Done
UI
Kaikkien laskutietojen hakeminen ja listaaminen
Kaikkien tuotetietojen hakeminen listaaminen
Kaikkien laskujen lisätietojen hakeminen ja listaaminen


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

            holderObj.Invoices = repoObj.GetInvoices();
            DataContext = holderObj;
        }

        private void OpenInformationWindow(object sender, RoutedEventArgs e)
        {
            // Otetaan valittu rivi talteen josta avataan lisätiedot
            int re = DtgMain.SelectedIndex;

            LaskutusView laskutusView = new(re);
            laskutusView.ShowDialog();
        }
    }

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

    public class Invoice
    {
        // Laskun tiedot haetaan databindingilla MainWindowiin Id ja AddressBiller loput haetaan LaskutusView ikkunaan
        public int Id { get; set; }
        public string? AddressDelivery { get; set; }
        public string? AddressBiller { get; set; }
        public DateTime DateBill { get; set; }
        public DateTime DateDue { get; set; }
        public string? ExtraInformation { get; set; }

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

    public class ContractDetails
    {
        public string? ProductName { get; set; }
        public int ProductAmount { get; set; }
        public double ProductCost { get; set; }

        public ContractDetails()
        {
            ProductName = string.Empty;
        }
    }

    /// <summary>
    /// Hallinoi yhteyksiä tietokantaan
    /// </summary>
    public class InvoiceRepository
    {

        const string LOCAL_CONNECT = @"Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1;";
        const string LOCAL_CONNECT_DB = @"Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=projektityo_nn_2206189;";

        const string SELECT_ALL_INVOICE = "SELECT * FROM invoice";


        public ObservableCollection<Invoice> GetInvoices()
        {
            var invoices = new ObservableCollection<Invoice>();

            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                var cmdObj = SqlExecuteReader(connObj, SELECT_ALL_INVOICE);
                var dr = cmdObj.ExecuteReader();

                while (dr.Read())
                {
                    invoices.Add(new Invoice
                    {
                        Id = dr.GetInt32("invoice_id"),
                        AddressDelivery = dr.GetString("address_delivery"),
                        AddressBiller = dr.GetString("address_biller"),
                        DateBill = dr.GetDateTime("date_bill"),
                        DateDue = dr.GetDateTime("date_due"),
                        ExtraInformation = dr.GetString("extra_information"),
                    });
                }
            }

            return invoices;
        }

        public ObservableCollection<ContractDetails> GetDetails(int keyValue)
        {
            // Hakee tiedot Invoice.Details property olioihin eli muodostaa ContractDetails oliot tietokannan perusteella
            keyValue++;

            var details = new ObservableCollection<ContractDetails>();
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                var cmdObj = SqlExecuteReader(connObj, $"SELECT product_amount, product_cost, product_name FROM product WHERE product_id IN (SELECT product_id FROM laskun_rivit WHERE invoice_id = {keyValue})");
                var dr = cmdObj.ExecuteReader();

                while (dr.Read())
                {
                    details.Add(new ContractDetails
                    {
                        ProductName = dr.GetString("product_name"),
                        ProductAmount = dr.GetInt32("product_amount"),
                        ProductCost = dr.GetDouble("product_cost"),
                    });
                }

            }
            
            return details;
        }

        private static MySqlCommand SqlExecuteReader(MySqlConnection connObj, string command)
        {
            // metodi Sql komentojen luomiseen

            connObj.Open();
            MySqlCommand cmdObj = new(command, connObj);
            return cmdObj;
        }
    }
}
