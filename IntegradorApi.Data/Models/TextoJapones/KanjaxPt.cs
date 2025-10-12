using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegradorApi.Data.Models.TextoJapones;

[Table("kanjax_pt")]
public class KanjaxPt {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("Sequencia")]
    public long Sequencia { get; set; }

    [Required]
    [StringLength(10)]
    [Column("kanji")]
    public string Kanji { get; set; }

    [Required]
    [StringLength(100)]
    [Column("keyword")]
    public string Keyword { get; set; }

    [Required]
    [StringLength(250)]
    [Column("meaning")]
    public string Meaning { get; set; }

    [Required]
    [Column("koohii1")]
    public string Koohii1 { get; set; }

    [Required]
    [Column("koohii2")]
    public string Koohii2 { get; set; }

    [Required]
    [StringLength(100)]
    [Column("onyomi")]
    public string Onyomi { get; set; }

    [Required]
    [StringLength(100)]
    [Column("kunyomi")]
    public string Kunyomi { get; set; }

    [Required]
    [Column("onwords")]
    public string Onwords { get; set; }

    [Required]
    [Column("kunwords")]
    public string Kunwords { get; set; }

    [Required]
    [Column("jlpt")]
    public int Jlpt { get; set; }

    [Required]
    [Column("grade")]
    public int Grade { get; set; }

    [Required]
    [Column("freq")]
    public int Freq { get; set; }

    [Required]
    [Column("strokes")]
    public int Strokes { get; set; }

    [Required]
    [StringLength(100)]
    [Column("variants")]
    public string Variants { get; set; }

    [Required]
    [StringLength(100)]
    [Column("radical")]
    public string Radical { get; set; }

    [Required]
    [StringLength(100)]
    [Column("parts")]
    public string Parts { get; set; }

    [Required]
    [StringLength(10)]
    [Column("utf8")]
    public string Utf8 { get; set; }

    [Required]
    [StringLength(5)]
    [Column("sjis")]
    public string Sjis { get; set; }

    [Column("traduzido")]
    public bool IsTraduzido { get; set; }

    [Column("checado")]
    public bool IsChecado { get; set; }

    [Column("revisado")]
    public bool IsRevisado { get; set; }

    [Column("sinalizado")]
    public bool IsSinaliza { get; set; }

    [Column("data_traducao")]
    public DateTime DataTraducao { get; set; }

    [Column("data_correcao")]
    public DateTime DataCorrecao { get; set; }

    [StringLength(100)]
    [Column("obs")]
    public string Observacao { get; set; }

    [Required]
    [Column("kanjaxOriginal")]
    public bool KanjaxOriginal { get; set; }

    [Required]
    [StringLength(100)]
    [Column("palavra")]
    public string Palavra { get; set; }

    [Required]
    [StringLength(250)]
    [Column("significado")]
    public string Significado { get; set; }

    [Column("atualizacao")]
    public DateTime Atualizacao { get; set; }

    public KanjaxPt() {
        Kanji = string.Empty;
        Keyword = string.Empty;
        Meaning = string.Empty;
        Koohii1 = string.Empty;
        Koohii2 = string.Empty;
        Onyomi = string.Empty;
        Kunyomi = string.Empty;
        Onwords = string.Empty;
        Kunwords = string.Empty;
        Variants = string.Empty;
        Radical = string.Empty;
        Parts = string.Empty;
        Utf8 = string.Empty;
        Sjis = string.Empty;
        DataTraducao = DateTime.Now;
        DataCorrecao = DateTime.Now;
        Observacao = string.Empty;
        Palavra = string.Empty;
        Significado = string.Empty;
        Atualizacao = DateTime.Now;
    }
}