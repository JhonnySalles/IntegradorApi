using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoJapones;

[Table("fila_sql")]
public class FilaSqlJapones {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("sequencial")]
    public long Sequencial { get; set; }

    [Required]
    [Column("selectSQL")]
    public string SelectSQL { get; set; }

    [Required]
    [Column("updateSQL")]
    public string UpdateSQL { get; set; }

    [Required]
    [Column("deleteSQL")]
    public string DeleteSQL { get; set; }

    [Required]
    [Column("vocabulario")]
    public string Vocabulario { get; set; }

    [Required]
    [Column("isExporta")]
    public bool IsExporta { get; set; }

    [Required]
    [Column("isLimpeza")]
    public bool IsLimpeza { get; set; }

    [Column("atualizacao")]
    public DateTime Atualizacao { get; set; }

    public FilaSqlJapones() {
        SelectSQL = string.Empty;
        UpdateSQL = string.Empty;
        DeleteSQL = string.Empty;
        Vocabulario = string.Empty;
        Atualizacao = DateTime.Now;
    }
}