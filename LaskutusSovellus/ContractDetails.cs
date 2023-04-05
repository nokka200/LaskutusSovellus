namespace LaskutusSovellus
{
    /// <summary>
    /// Tämä luokka sisältää laskun lisätiedot
    /// </summary>
    public class ContractDetails
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int ProductAmount { get; set; }
        public double ProductUnitCost { get; set; }
        public double ProductCost { get; set; }
        public int InvoiceId { get; set; }


        public ContractDetails()
        {
            ProductName = string.Empty;
        }
    }
}
