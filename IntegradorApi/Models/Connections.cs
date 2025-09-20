using IntegradorApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Models;

[Table("conexoes")]
public class Connections {
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
    public int User { get; set; }

    [Column("senha")]
    public int Password { get; set; }

    [Column("opcional")]
    public int Optional { get; set; }

    [Column("ativo")]
    public int Enabled { get; set; }

}