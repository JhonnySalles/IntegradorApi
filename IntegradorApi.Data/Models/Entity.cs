/// <summary>
/// Classe base abstrata para todas as entidades de dados.
/// Garante que toda entidade tenha uma propriedade de identificação.
/// </summary>
public abstract class Entity {
    public Guid Id { get; set; }
}