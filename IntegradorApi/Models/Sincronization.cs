using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IntegradorApi.Models;

[Table("sincronizacoes")]
public class Sincronization {
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("identificador_recurso")]
    public string ResourceIdentifier { get; set; }

    [Column("sincronizacao")]
    public DateTime dtLastSyncronization { get; set; }

    // Chave Estrangeira para a Conexão relacionada
    [Column("id_conexao")]
    public int ConnectionId { get; set; }

    // Propriedade de Navegação
    [ForeignKey("ConnectionId")]
    public Connection Connection { get; set; }
}
