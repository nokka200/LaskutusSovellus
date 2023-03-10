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

TO_DO

Yksittäisen laskun tietojen ylläpito (lisäys, muutos poisti)
Yksittäisen tuotteen tietojen ylläpito (lisäys, muutos poisti)


IN_PROGRESS
TallennusView Tallennus napin toiminta

DONE
UI
Ui Tallennus napin lisääminen LaskutusView 
Ui Poisto napin lisääminen LaskutusView 

Kaikkien laskutietojen hakeminen ja listaaminen
Kaikkien tuotetietojen hakeminen listaaminen
Kaikkien laskujen lisätietojen hakeminen ja listaaminen
Tietokannan tyhjennäs ja luonti kun ohjelma käynnistyy



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
        const string DROP_DB = "DROP DATABASE IF EXISTS projektityo_nn_2206189";
        const string CREATE_DB = "CREATE DATABASE IF NOT EXISTS projektityo_nn_2206189";

        const string CREATE_TABLE_INVOICE = "CREATE TABLE invoice(" +
                                                "invoice_id INT NOT NULL AUTO_INCREMENT," +
                                                "address_delivery VARCHAR(25) NOT NULL DEFAULT '-'," +
                                                "address_biller VARCHAR(25) NOT NULL DEFAULT 'Rakennus Oy'," +
                                                "date_bill DATE NOT NULL," +
                                                "date_due DATE NOT NULL," +
                                                "extra_information VARCHAR(50) NOT NULL DEFAULT '-'," +
                                                "PRIMARY KEY (invoice_id))";
        const string CREATE_TABLE_PRODUCT = "CREATE TABLE product(" +
                                                "product_id INT NOT NULL AUTO_INCREMENT," +
                                                "product_amount INT NOT NULL DEFAULT 0," +
                                                "product_cost DECIMAL(10,2) NOT NULL DEFAULT 0," +
                                                "product_name VARCHAR(25) NOT NULL DEFAULT '-'," +
                                                "PRIMARY KEY (product_id))";
        const string CREATE_TABLE_LASKUN_RIVIT = "CREATE TABLE laskun_rivit(" +
                                                    "rivi_id INT NOT NULL AUTO_INCREMENT," +
                                                    "invoice_id INT NOT NULL," +
                                                    "product_id INT NOT NULL," +
                                                    "PRIMARY KEY (rivi_id)," +
                                                    "FOREIGN KEY (invoice_id) REFERENCES invoice(invoice_id)," +
                                                    "FOREIGN KEY (product_id) REFERENCES product(product_id)," +
                                                    "UNIQUE (invoice_id, product_id))";

        const string INSERT_DEFAULT_INVOICE = "INSERT INTO invoice (address_delivery, date_bill, date_due, extra_information)" +
                                                "VALUES('Puuhala 2', '2023-08-03', '2023-08-17', 'Tällaista')," +
                                                        "('Puuhala 5', '2023-12-08', '2023-12-22', 'Simo')";
        const string INSERT_DEFAULT_PRODUCT = "INSERT INTO product (product_amount, product_cost, product_name)" +
                                                "VALUES(50, 5, 'Vasarointi')," +
                                                "(100, 0.5, 'Naulat')," +
                                                "(1, 500, 'Putkiremontti')";
        const string INSERT_DEFAULT_LASKUN_RIVIT = "INSERT INTO laskun_rivit (invoice_id, product_id)" +
                                                      "VALUES(1,1)," +
                                                        "(1,2)," +
                                                        "(2,3)";

        /// <summary>
        /// Tiputtaa ja luo tietokantaa projektin
        /// </summary>
        public void DropAndCreateProjectDb()
        {
            // Tiputetaan ja luodaan tietokanta projektityö_nn_2206189
            using(MySqlConnection connObj = new(LOCAL_CONNECT))
            {
                connObj.Open();

                MySqlCommand cmd = new(DROP_DB, connObj);
                cmd.ExecuteNonQuery();

                cmd = new(CREATE_DB, connObj);
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateInvoiceTable()
        {
            //luo Invoice taulun tietokantaan
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                connObj.Open();

                MySqlCommand cmd = new(CREATE_TABLE_INVOICE, connObj);
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateProductTable()
        {
            // luo product taulun tietokantaan
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                connObj.Open();
                MySqlCommand cmd = new(CREATE_TABLE_PRODUCT, connObj);
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateLaskunRivitTable()
        {
            // luo yhteisen laskun_rivit taulun tietokantaan
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                connObj.Open();
                MySqlCommand cmd = new(CREATE_TABLE_LASKUN_RIVIT, connObj);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddDefaultInvoice()
        {
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                connObj.Open();
                MySqlCommand cmd = new(INSERT_DEFAULT_INVOICE, connObj);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddDefaultProduct()
        {
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                connObj.Open();
                MySqlCommand cmd = new(INSERT_DEFAULT_PRODUCT, connObj);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddDefaultLaskunRivit()
        {
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                connObj.Open();
                MySqlCommand cmd = new(INSERT_DEFAULT_LASKUN_RIVIT, connObj);
                cmd.ExecuteNonQuery();
            }
        }




        /// <summary>
        /// Hakee kaikki laskut
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Hakee Laskun Details it
        /// </summary>
        /// <param name="keyValue">Minkä laskun haetaan</param>
        /// <returns></returns>
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

        public void UpdateInvoice(Invoice toUpdate)
        {
            // TODO Update to database

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
