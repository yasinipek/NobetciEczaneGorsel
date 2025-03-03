namespace NobetciEczaneGorsel.Models
{
    public class EczaneModel
    {
        public int Id { get; set; }
        public string Isim { get; set; }
        public int IlId { get; set; }
        public virtual IlModel Il { get; set; }
        public string Ilce { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public string Enlem { get; set; }
        public string Boylam { get; set; }
        public string Tarih { get; set; }
        public DateTime KayitZamani { get; set; } = DateTime.Now;
    }
}
