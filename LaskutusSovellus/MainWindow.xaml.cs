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
UI OK
Tietokanta OK, tietokannan luonti puuttuu codebehindissä, testi tietokanta OK
Yhteys toimii ensimmäisellä sivulla, ei päivitä vielä mitään
Yhteys toiselle sivulle, tiedot toimii mutta rivit eivät vielä

 */
namespace LaskutusSovellus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InvoiceRepository repoObj;
        InvoiceHolder holderObj;

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
            int re = DtgMain.SelectedIndex;

            LaskutusView laskutusView = new(re);
            laskutusView.ShowDialog();
        }

        private void Selected()
        {
            int re = DtgMain.SelectedIndex;
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
        public double CostTotal { get; set; }

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

        public ObservableCollection<Invoice> GetInvoices()
        {
            var invoices = new ObservableCollection<Invoice>();

            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                var cmdObj = SqlExecuteReader(connObj, "SELECT * FROM invoice");
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

        public ObservableCollection<ContractDetails> GetDetails()
        {
            var details = new ObservableCollection<ContractDetails>();

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
