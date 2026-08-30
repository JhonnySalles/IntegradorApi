using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoIngles;

[Table("valido")]
public class ValidoIngles : Entity {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public new Guid Id { get; set; }

    [Required]
    [StringLength(250)]
    [Column("palavra")]
    public string Palavra { get; set; }

    [Column("atualizacao")]
    public DateTime Atualizacao { get; set; }

    public ValidoIngles() {
        Palavra = string.Empty;
        Atualizacao = DateTime.Now;
    }
}