using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoIngles;

[Table("vocabulario")]
public class VocabularioIngles {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [StringLength(250)]
    [Column("vocabulario")]
    public string Vocabulario { get; set; }

    [Required]
    [StringLength(250)]
    [Column("leitura")]
    public string Leitura { get; set; }

    [Required]
    [Column("portugues")]
    public string Portugues { get; set; }

    [Column("atualizacao")]
    public DateTime Atualizacao { get; set; }

    public VocabularioIngles() {
        Vocabulario = string.Empty;
        Leitura = string.Empty;
        Portugues = string.Empty;
        Atualizacao = DateTime.Now;
    }
}