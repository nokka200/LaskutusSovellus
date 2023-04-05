using MySqlConnector;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
    /// Hallinoi yhteyksiä tietokantaan
    /// </summary>
    public class InvoiceRepository
    {

        const string LOCAL_CONNECT = @"Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1;";
        const string LOCAL_CONNECT_DB = @"Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=projektityo2_nn_2206189;";

        const string SELECT_ALL_INVOICE = "SELECT * FROM invoice";
        const string DROP_DB = "DROP DATABASE IF EXISTS projektityo2_nn_2206189";
        const string CREATE_DB = "CREATE DATABASE IF NOT EXISTS projektityo2_nn_2206189";

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
                                                "product_unit_cost DECIMAL(10,2) NOT NULL DEFAULT 0," +
                                                "product_name VARCHAR(25) NOT NULL DEFAULT '-'," +
                                                "PRIMARY KEY (product_id))";
        const string CREATE_TABLE_LASKUN_RIVIT = "CREATE TABLE laskun_rivit(" +
                                                    "rivi_id INT NOT NULL AUTO_INCREMENT," +
                                                    "product_amount INT NOT NULL DEFAULT 0," +
                                                    "invoice_id INT NOT NULL," +
                                                    "product_id INT NOT NULL," +
                                                    "PRIMARY KEY (rivi_id)," +
                                                    "FOREIGN KEY (invoice_id) REFERENCES invoice(invoice_id) ON DELETE cascade," +
                                                    "FOREIGN KEY (product_id) REFERENCES product(product_id) ON DELETE cascade," +
                                                    "UNIQUE (invoice_id, product_id))";

        const string INSERT_DEFAULT_INVOICE = "INSERT INTO invoice (address_delivery, date_bill, date_due, extra_information)" +
                                                "VALUES('Puuhala 2', '2023-08-03', '2023-08-17', 'Tällaista')," +
                                                        "('Puuhala 5', '2023-12-08', '2023-12-22', 'Simo')";
        const string INSERT_DEFAULT_PRODUCT = "INSERT INTO product (product_unit_cost, product_name)" +
                                                "VALUES(25, 'Vasarointi')," +
                                                "(1, 'Naulat')," +
                                                "(500, 'Putkiremontti')";
        const string INSERT_DEFAULT_LASKUN_RIVIT = "INSERT INTO laskun_rivit (product_amount, invoice_id, product_id)" +
                                                      "VALUES(8, 1,1)," +
                                                        "(50, 1,2)," +
                                                        "(1, 2,3)";

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

        /// <summary>
        /// Luo oletus Invoice taulun
        /// </summary>
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

        /// <summary>
        /// Luo oletus Product taulun
        /// </summary>
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

        /// <summary>
        /// Luo oletus laskun_rivit taulun 
        /// </summary>
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

        /// <summary>
        /// Antaa oletusarvot Invoice tauluun
        /// </summary>
        public void AddDefaultInvoice()
        {
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                connObj.Open();
                MySqlCommand cmd = new(INSERT_DEFAULT_INVOICE, connObj);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Antaa oletusarvot Product tauluun
        /// </summary>
        public void AddDefaultProduct()
        {
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                connObj.Open();
                MySqlCommand cmd = new(INSERT_DEFAULT_PRODUCT, connObj);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Antaa oletusarvot laskutus_rivit tauluun
        /// </summary>
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
                var cmdObj = SqlExecuteReader(connObj, $"SELECT p.product_id, p.product_unit_cost, p.product_name, lr.product_amount " +
                    $"FROM product p " +
                    $"JOIN laskun_rivit lr ON p.product_id = lr.product_id " +
                    $"WHERE p.product_id IN (SELECT product_id FROM laskun_rivit WHERE invoice_id = {keyValue})");
                var dr = cmdObj.ExecuteReader();

                while (dr.Read())
                {
                    details.Add(new ContractDetails
                    {
                        ProductId = dr.GetInt32("product_id"),
                        ProductName = dr.GetString("product_name"),
                        ProductAmount = dr.GetInt32("product_amount"),
                        ProductUnitCost = dr.GetDouble("product_unit_cost"),
                    });
                }

            }
            return details;
        }

        public ObservableCollection<ContractDetails> GetAllDetails()
        {
            // hakee Product taulusta kaikki tietueet
            var details = new ObservableCollection<ContractDetails>();
            using (MySqlConnection connObj = new(LOCAL_CONNECT_DB))
            {
                var cmdObj = SqlExecuteReader(connObj, "SELECT p.product_id, p.product_unit_cost, p.product_name, lr.product_amount FROM product p JOIN laskun_rivit lr ON p.product_id = lr.product_id");
                var dr = cmdObj.ExecuteReader();

                while (dr.Read())
                {
                    details.Add(new ContractDetails
                    {
                        ProductId = dr.GetInt32("product_id"),
                        ProductName = dr.GetString("product_name"),
                        ProductAmount = dr.GetInt32("product_amount"),
                        ProductUnitCost = dr.GetDouble("product_unit_cost"),
                    });
                }
            }
            return details;
        }
        
        public void UpdateInvoice(Invoice invoiceToUpdate)
        {
            // TODO Update to database päivämäärien muutos
            // tällä hetkellä päivittää kaikki muut paitsi ID ja päivämäärät
            using (MySqlConnection conn = new MySqlConnection(LOCAL_CONNECT_DB))
            {
                conn.Open();

                // Päivämäärien muutos jätetty pois
                MySqlCommand cmd = new MySqlCommand($"UPDATE invoice SET address_delivery = @address_delivery, extra_information = @extra_information where invoice_id = @invoice_id", conn);
                cmd.Parameters.AddWithValue("@address_delivery", invoiceToUpdate.AddressDelivery);
                cmd.Parameters.AddWithValue("@extra_information", invoiceToUpdate.ExtraInformation);
                cmd.Parameters.AddWithValue("@invoice_id", invoiceToUpdate.Id);
                cmd.ExecuteNonQuery();
            }
        }

        // JATKA tästä!
        public void UpdateDetails(Invoice invoiceToUpdate)
        {
            // Päivittää Invoice Details osion
            // Pitää ensin päivittää Product tauluun tiedot ja sen jälkeen tehdä yhteys laskun_rivit tauluun kyseisen avatun laskun kanssa
            using (MySqlConnection conn = new MySqlConnection(LOCAL_CONNECT_DB))
            {
                conn.Open();

                foreach(var line in invoiceToUpdate.Details)
                {
                    if(line.ProductId == 0)
                    {
                        // Tämä luo uuden rivin Product pöytään
                        MySqlCommand cmdIns = new MySqlCommand("INSERT INTO product (product_unit_cost, product_name) VALUES(@product_unit_cost, @product_name)", conn);
                        cmdIns.Parameters.AddWithValue("@product_unit_cost", line.ProductUnitCost);
                        cmdIns.Parameters.AddWithValue("@product_name", line.ProductName);
                        cmdIns.ExecuteNonQuery();
                        Debug.WriteLine(line.ProductId);

                        int re = GetNewProductId(conn);

                        conn.Open();            // joku bugi että yhteys pitää sulkea ja avata uusiksi

                        // HUOM tämä ehkä turhaa jos ON UPDATE CASCADE mahdollisesti ajaa samanaa asiaa
                        // Tämä yhdistää luodun productin avoinna olevaan Invoice laskuun
                        MySqlCommand cmdInsLaskunRivit = new MySqlCommand("INSERT INTO laskun_rivit (product_amount, invoice_id, product_id) VALUES(@product_amount, @invoice_id, @product_id)", conn);
                        cmdInsLaskunRivit.Parameters.AddWithValue("@product_amount", line.ProductAmount);
                        cmdInsLaskunRivit.Parameters.AddWithValue("@invoice_id", invoiceToUpdate.Id);
                        cmdInsLaskunRivit.Parameters.AddWithValue("@product_id", re);            
                        cmdInsLaskunRivit.ExecuteNonQuery();
                    }
                    else
                    {
                        // jos ei ole uusi rivi, päivitetään vanhat HUOM rivit jotka eivät kuulu avattuun laskuun!
                        MySqlCommand cmdUpdate = new MySqlCommand("UPDATE product SET product_unit_cost = @product_unit_cost, product_name = @product_name WHERE product_id = @product_id", conn);
                        cmdUpdate.Parameters.AddWithValue("@product_unit_cost", line.ProductUnitCost);
                        cmdUpdate.Parameters.AddWithValue("@product_name", line.ProductName);
                        cmdUpdate.Parameters.AddWithValue("@product_id", line.ProductId);
                        cmdUpdate.ExecuteNonQuery();

                        
                        MySqlCommand cmdUpdateRivi = new MySqlCommand("UPDATE laskun_rivit SET product_amount = @product_amount WHERE product_id = @product_id", conn);
                        cmdUpdateRivi.Parameters.AddWithValue("@product_amount", line.ProductAmount);
                        cmdUpdateRivi.Parameters.AddWithValue("@product_id", line.ProductId);
                        cmdUpdateRivi.ExecuteNonQuery();
                        // TODO mahdollisesti toimen update käsky koska määrä on nyt laskun_rivit taulussa!
                    }
                }
            }
        }

        public void AddInvoice()
        {
            // Päivittää ja lisää MainWindow uuden laskun
            using (MySqlConnection conn = new MySqlConnection(LOCAL_CONNECT_DB))
            {
                Invoice tempObj = new();
                conn.Open();
                MySqlCommand cmdAddInvoice = new MySqlCommand("INSERT INTO invoice (address_delivery, date_bill, date_due) VALUES('-', @date_bill, @date_due)", conn);
                cmdAddInvoice.Parameters.AddWithValue("@date_bill", tempObj.DateBill);
                cmdAddInvoice.Parameters.AddWithValue("@date_due", tempObj.DateDue);
                cmdAddInvoice.ExecuteNonQuery();
            }
        }

        public void DeleteDetails(int selected)
        {
            using (MySqlConnection conn = new MySqlConnection(LOCAL_CONNECT_DB))
            {
                conn.Open();

                MySqlCommand cmdDel = new MySqlCommand($"DELETE FROM product WHERE product_id = {selected}", conn);
                cmdDel.ExecuteNonQuery();
            }
        }

        public void DeleteInvoice(int selected)
        {
            using (MySqlConnection conn = new MySqlConnection(LOCAL_CONNECT_DB))
            {
                conn.Open();

                MySqlCommand cmdDel = new MySqlCommand($"DELETE FROM invoice WHERE invoice_id = {selected}", conn);
                cmdDel.ExecuteNonQuery();
            }
        }

        private static int GetNewProductId(MySqlConnection conn)
        {
            // Hakee uusimman product_id että saadaan yhdistettyä lasku ja lisätiedot
            var newestProductId = new MySqlCommand("SELECT MAX(product_id) AS product_id FROM product", conn);
            var dr = newestProductId.ExecuteReader();
            dr.Read();
            int re = dr.GetInt32("product_id");
            conn.Close();
            return re;
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
