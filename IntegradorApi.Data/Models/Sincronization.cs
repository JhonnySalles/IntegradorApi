using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Models;

[Table("sincronizacoes")]
public class Sincronization {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("identificador_recurso")]
    public string ResourceIdentifier { get; set; }

    [Column("sincronizacao")]
    public DateTime LastSyncronization { get; set; }

    // Chave Estrangeira para a Conexão relacionada
    [Column("id_conexao")]
    public int ConnectionId { get; set; }

    // Propriedade de Navegação
    [ForeignKey("ConnectionId")]
    public Microsoft.EntityFrameworkCore.DbLoggerCategory.Database.Connection Connection { get; set; }
}
