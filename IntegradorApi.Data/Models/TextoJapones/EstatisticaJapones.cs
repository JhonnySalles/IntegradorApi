using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoJapones;

[Table("estatistica")]
public class EstatisticaJapones {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("sequencial")]
    public long? Sequencial { get; set; }

    [Required]
    [StringLength(10)]
    [Column("kanji")]
    public string Kanji { get; set; }

    [Required]
    [StringLength(10)]
    [Column("leitura")]
    public string Leitura { get; set; }

    [Required]
    [StringLength(10)]
    [Column("tipo")]
    public string Tipo { get; set; }

    [Required]
    [Column("quantidade")]
    public double Quantidade { get; set; }

    [Required]
    [Column("percentual")]
    public float Percentual { get; set; }

    [Required]
    [Column("media")]
    public double Media { get; set; }

    [Required]
    [Column("percentual_medio")]
    public float PercentualMedio { get; set; }

    [Required]
    [Column("cor_sequencial")]
    public int CorSequencial { get; set; }

    [Column("atualizacao")]
    public DateTime Atualizacao { get; set; }

    public EstatisticaJapones() {
        Kanji = string.Empty;
        Leitura = string.Empty;
        Tipo = string.Empty;
        Atualizacao = DateTime.Now;
    }
}