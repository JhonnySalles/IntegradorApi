using IntegradorApi.Data.Enums;

namespace IntegradorApi.Data.Models.DeckSubtitle;

[Serializable]
public class Subtitle : Entity {
    public int Sequencia { get; set; }
    public int Episodio { get; set; }
    public Linguagens Linguagem { get; set; }
    public string Tempo { get; set; }
    public string Texto { get; set; }
    public string Traducao { get; set; }
    public string? Vocabulario { get; set; }

    public Subtitle() {
        Id = null;
        Sequencia = 0;
        Episodio = 0;
        Linguagem = Linguagens.PORTUGUESE;
        Tempo = string.Empty;
        Texto = string.Empty;
        Traducao = string.Empty;
        Vocabulario = null;
    }

    public static Subtitle Create(Guid? id) {
        return new Subtitle { Id = id };
    }

    public void Merge(Subtitle source) {
        this.Sequencia = source.Sequencia;
        this.Episodio = source.Episodio;
        this.Tempo = source.Tempo;
        this.Linguagem = source.Linguagem;
        this.Traducao = source.Traducao;
        this.Vocabulario = source.Vocabulario;
        this.Texto = source.Texto;
    }

    public void Patch(Subtitle source) {
        if (source.Sequencia > 0)
            this.Sequencia = source.Sequencia;

        if (source.Episodio > 0)
            this.Episodio = source.Episodio;

        if (!string.IsNullOrEmpty(source.Tempo))
            this.Tempo = source.Tempo;

        if (!string.IsNullOrEmpty(source.Traducao))
            this.Traducao = source.Traducao;

        if (!string.IsNullOrEmpty(source.Vocabulario))
            this.Vocabulario = source.Vocabulario;

        if (!string.IsNullOrEmpty(source.Texto))
            this.Texto = source.Texto;
    }

    public override bool Equals(object? obj) {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        Subtitle other = (Subtitle)obj;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() {
        return Id?.GetHashCode() ?? 0;
    }
}