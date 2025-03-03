using System.ComponentModel.DataAnnotations;

namespace NobetciEczaneGorsel.Models
{
    public class IlModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string IlAdi { get; set; }

        // İlişkisel bağlantılar
        public virtual ICollection<EczaneModel>? Eczaneler { get; set; }
    }
}
