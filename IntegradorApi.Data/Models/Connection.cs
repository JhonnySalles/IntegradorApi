using IntegradorApi.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models;

[Table("conexoes")]
public class Connection {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("fonte")]
    public DataSourceType TypeDataSource { get; set; }

    [Column("descricao")]
    public string Description { get; set; }

    [Required]
    [Column("conexao")]
    public ConnectionType TypeConnection { get; set; }

    [Required]
    [Column("url")]
    public string Address { get; set; }

    [Column("usuario")]
    public string User { get; set; }

    [Column("senha")]
    public string Password { get; set; }

    [Column("opcional")]
    public string Optional { get; set; }

    [Required]
    [Column("integracao")]
    public IntegrationType TypeIntegration { get; set; }

    [Column("delete")]
    public Boolean Delete { get; set; }

    [Required]
    [Column("ativo")]
    public Boolean Enabled { get; set; }

    public ICollection<Sincronization> Sincronizations { get; set; } = new List<Sincronization>();

}
