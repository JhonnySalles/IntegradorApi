using IntegradorApi.Data.Models.TextoJapones;
using Microsoft.EntityFrameworkCore;

namespace IntegradorApi.Data.Data;

public class TextoJaponesDbContext : DbContext {
    public DbSet<EstatisticaJapones> Estatisticas { get; set; }
    public DbSet<ExclusaoJapones> Exclusoes { get; set; }
    public DbSet<FilaSqlJapones> FilasSql { get; set; }
    public DbSet<KanjaxPt> Kanjax { get; set; }
    public DbSet<KanjiInfo> KanjiInfos { get; set; }
    public DbSet<RevisarJapones> Revisar { get; set; }
    public DbSet<VocabularioJapones> Vocabularios { get; set; }

    public TextoJaponesDbContext(DbContextOptions<TextoJaponesDbContext> options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
    }
}
