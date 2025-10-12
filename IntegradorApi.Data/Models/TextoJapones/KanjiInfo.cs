using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoJapones;

[Table("words_kanji_info")]
public class KanjiInfo {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("sequencia")]
    public long Sequencia { get; set; }

    [Required]
    [StringLength(100)]
    [Column("word")]
    public string Word { get; set; }

    [Required]
    [StringLength(350)]
    [Column("read_info")]
    public string ReadInfo { get; set; }

    [Required]
    [Column("frequency")]
    public int Frequency { get; set; }

    [Required]
    [Column("tabela")]
    public string Tabela { get; set; }

    [Column("atualizacao")]
    public DateTime Atualizacao { get; set; }

    public KanjiInfo() {
        Word = string.Empty;
        ReadInfo = string.Empty;
        Tabela = string.Empty;
        Atualizacao = DateTime.Now;
    }
}