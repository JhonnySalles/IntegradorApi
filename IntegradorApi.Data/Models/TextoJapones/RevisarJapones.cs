using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoJapones;

[Table("revisar")]
public class RevisarJapones {
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
    [Column("forma_basica")]
    public string FormaBasica { get; set; }

    [Required]
    [StringLength(250)]
    [Column("leitura")]
    public string Leitura { get; set; }

    [Required]
    [StringLength(250)]
    [Column("leitura_novel")]
    public string LeituraNovel { get; set; }

    [Required]
    [Column("ingles")]
    public string Ingles { get; set; }

    [Required]
    [Column("portugues")]
    public string Portugues { get; set; }

    [Required]
    [Column("revisado")]
    public bool Revisado { get; set; }

    [Required]
    [Column("aparece")]
    public int Aparece { get; set; }

    [Required]
    [Column("isAnime")]
    public bool IsAnime { get; set; }

    [Required]
    [Column("isManga")]
    public bool IsManga { get; set; }

    [Column("atualizacao")]
    public DateTime Atualizacao { get; set; }

    public RevisarJapones() {
        Vocabulario = string.Empty;
        FormaBasica = string.Empty;
        Leitura = string.Empty;
        LeituraNovel = string.Empty;
        Ingles = string.Empty;
        Portugues = string.Empty;
        Atualizacao = DateTime.Now;
    }
}