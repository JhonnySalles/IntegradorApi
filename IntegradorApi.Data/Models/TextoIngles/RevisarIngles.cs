using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoIngles;

[Table("revisar")]
public class RevisarIngles {
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

    public RevisarIngles() {
        Vocabulario = string.Empty;
        Leitura = string.Empty;
        Portugues = string.Empty;
        Atualizacao = DateTime.Now;
    }
}