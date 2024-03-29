﻿using System;
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
            holderObj.Invoices[Selected].ProductTotal = GetDetailsSum(holderObj.Invoices[Selected].Details); // Lisätään Product total

            DataContext = holderObj.Invoices[Selected];                             // Asetetaan haettu lasku tiedosidonnan kohteeksi
            DtgLaskutusView.ItemsSource = holderObj.Invoices[Selected].Details;     // haetaan datagridin lähde oikeaksi ContractDetail olioksi
        }

        private double GetDetailsSum(ObservableCollection<ContractDetails> details)
        {
            double total = 0;

            foreach(var item in details)
            {
                total += item.ProductUnitCost * item.ProductAmount;
            }

            return total;
        }

        private void CheckSelected(int max)
        {
            if(Selected == -1)
            {
                Selected = 0;
            }
            if(Selected == max)
            {
                Selected--;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Tämä nappi tallentaa Invoice ja Details muokatut tiedot
            var invoce = (Invoice)DataContext;                                      // Haetaan DataContect Invoice muodossa
            repoObj.UpdateInvoice(invoce);                                          // Tehdään päivitys tietokantaan

            repoObj.UpdateDetails(invoce);                                          // UUSi feature, päivittää laskun detailsit

            MessageBox.Show("Tiedot tallennettu");
            UpdateView();
            e.Handled = true;
        }

        private void UpdateView()
        {
            holderObj.Invoices = repoObj.GetInvoices();                             // Haetaan laskun tiedot uudestaan
            holderObj.Invoices[Selected].Details = repoObj.GetDetails(Selected);    // Haetaan kaikki laskun lisätiedot
            holderObj.Invoices[Selected].ProductTotal = GetDetailsSum(holderObj.Invoices[Selected].Details); // Lisätään Product total

            DataContext = holderObj.Invoices[Selected];                             // päivitetään DataContext uusimmilla tiedoilla
            DtgLaskutusView.ItemsSource = holderObj.Invoices[Selected].Details;     // päivitetään myös laskun lisätiedot

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ContractDetails re;
            try
            {
                re = (ContractDetails)DtgLaskutusView.SelectedItem; // Haetaan valittu rivi ja muutetaan se ContractDetails että saamme Id numeron
                if(re != null)
                {
                    repoObj.DeleteDetails(re.ProductId);
                    e.Handled = true;
                    UpdateView();
                }
                if(re == null)
                {
                    MessageBox.Show("Valitse poistettava rivi");
                    e.Handled = true;
                }            
            } catch (InvalidCastException)
            {
                MessageBox.Show("Valitse poistettava rivi");
            }
            e.Handled = true;
        }
    }
}
