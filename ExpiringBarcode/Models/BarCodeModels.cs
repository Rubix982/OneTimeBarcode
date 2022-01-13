namespace ExpiringBarcodes.Models
{
    public class Barcode
    {
        public string barcode { get; set; }

        public Barcode(string barcode)
        {
            this.barcode = barcode;
        }
    }
}