using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoJapones;

[Table("exclusao")]
public class ExclusaoJapones {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [StringLength(250)]
    [Column("palavra")]
    public string Exclusao { get; set; }

    [Column("atualizacao")]
    public DateTime Atualizacao { get; set; }

    public ExclusaoJapones() {
        Exclusao = string.Empty;
        Atualizacao = DateTime.Now;
    }
}