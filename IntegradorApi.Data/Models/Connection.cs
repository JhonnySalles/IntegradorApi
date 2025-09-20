using IntegradorApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Models;

[Table("conexoes")]
public class Connection {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("descricao")]
    public string Description { get; set; }

    [Required]
    [Column("tipo")]
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

    [Column("ativo")]
    public Boolean Enabled { get; set; }

}
