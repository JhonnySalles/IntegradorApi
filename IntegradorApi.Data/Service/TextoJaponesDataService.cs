using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IntegradorApi.Data.Data;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.TextoJapones;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Data.Services;

public class TextoJaponesDataService : IAsyncDisposable {
    private readonly TextoJaponesDbContext _context;
    private readonly ILogger _logger;

    public TextoJaponesDataService(Connection connection, ILogger logger) {
        _logger = logger;
        
        var builder = new MySqlConnectionStringBuilder {
            Server = connection.Address,
            Database = connection.Optional,
            UserID = connection.User,
            Password = connection.Password,
            Pooling = true, 
            ConnectionTimeout = 30
        };

        var connectionString = builder.ConnectionString;
        var optionsBuilder = new DbContextOptionsBuilder<TextoJaponesDbContext>();
        
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        _context = new TextoJaponesDbContext(optionsBuilder.Options);
    }

    public async ValueTask DisposeAsync() {
        if (_context != null)
            await _context.DisposeAsync();
    }

    #region EstatisticaJapones
    public async Task<List<EstatisticaJapones>> SelectAllEstatisticaAsync(DateTime since) {
        return await _context.Estatisticas.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveEstatisticaAsync(List<EstatisticaJapones> entities) {
        _logger.Information("Salvando {Count} estatisticas", entities.Count);
        foreach (var entity in entities) {
            var existing = await _context.Estatisticas.FindAsync(entity.Id);
            if (existing != null)
                _context.Entry(existing).CurrentValues.SetValues(entity);
            else
                await _context.Estatisticas.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteEstatisticaAsync(List<EstatisticaJapones> entities) {
        _logger.Information("Deletando {Count} estatisticas", entities.Count);
        _context.Estatisticas.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region ExclusaoJapones
    public async Task<List<ExclusaoJapones>> SelectAllExclusaoAsync(DateTime since) {
        return await _context.Exclusoes.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveExclusaoAsync(List<ExclusaoJapones> entities) {
        _logger.Information("Salvando {Count} exclusoes", entities.Count);
        foreach (var entity in entities) {
            var existing = await _context.Exclusoes.FindAsync(entity.Id);
            if (existing != null)
                _context.Entry(existing).CurrentValues.SetValues(entity);
            else
                await _context.Exclusoes.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteExclusaoAsync(List<ExclusaoJapones> entities) {
        _logger.Information("Deletando {Count} exclusoes", entities.Count);
        _context.Exclusoes.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region FilaSqlJapones
    public async Task<List<FilaSqlJapones>> SelectAllFilaSqlAsync(DateTime since) {
        return await _context.FilasSql.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveFilaSqlAsync(List<FilaSqlJapones> entities) {
        _logger.Information("Salvando {Count} filas sql", entities.Count);
        foreach (var entity in entities) {
            var existing = await _context.FilasSql.FindAsync(entity.Id);
            if (existing != null)
                _context.Entry(existing).CurrentValues.SetValues(entity);
            else
                await _context.FilasSql.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteFilaSqlAsync(List<FilaSqlJapones> entities) {
        _logger.Information("Deletando {Count} filas sql", entities.Count);
        _context.FilasSql.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region KanjaxPt
    public async Task<List<KanjaxPt>> SelectAllKanjaxAsync(DateTime since) {
        return await _context.Kanjax.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveKanjaxAsync(List<KanjaxPt> entities) {
        _logger.Information("Salvando {Count} kanjax", entities.Count);
        foreach (var entity in entities) {
            var existing = await _context.Kanjax.FindAsync(entity.Id);
            if (existing != null)
                _context.Entry(existing).CurrentValues.SetValues(entity);
            else
                await _context.Kanjax.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteKanjaxAsync(List<KanjaxPt> entities) {
        _logger.Information("Deletando {Count} kanjax", entities.Count);
        _context.Kanjax.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region KanjiInfo
    public async Task<List<KanjiInfo>> SelectAllKanjiInfoAsync(DateTime since) {
        return await _context.KanjiInfos.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveKanjiInfoAsync(List<KanjiInfo> entities) {
        _logger.Information("Salvando {Count} kanji infos", entities.Count);
        foreach (var entity in entities) {
            var existing = await _context.KanjiInfos.FindAsync(entity.Id);
            if (existing != null)
                _context.Entry(existing).CurrentValues.SetValues(entity);
            else
                await _context.KanjiInfos.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteKanjiInfoAsync(List<KanjiInfo> entities) {
        _logger.Information("Deletando {Count} kanji infos", entities.Count);
        _context.KanjiInfos.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region RevisarJapones
    public async Task<List<RevisarJapones>> SelectAllRevisarAsync(DateTime since) {
        return await _context.Revisar.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveRevisarAsync(List<RevisarJapones> entities) {
        _logger.Information("Salvando {Count} revisar", entities.Count);
        foreach (var entity in entities) {
            var existing = await _context.Revisar.FindAsync(entity.Id);
            if (existing != null)
                _context.Entry(existing).CurrentValues.SetValues(entity);
            else
                await _context.Revisar.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteRevisarAsync(List<RevisarJapones> entities) {
        _logger.Information("Deletando {Count} revisar", entities.Count);
        _context.Revisar.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region VocabularioJapones
    public async Task<List<VocabularioJapones>> SelectAllVocabularioAsync(DateTime since) {
        return await _context.Vocabularios.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveVocabularioAsync(List<VocabularioJapones> entities) {
        _logger.Information("Salvando {Count} vocabularios", entities.Count);
        foreach (var entity in entities) {
            var existing = await _context.Vocabularios.FindAsync(entity.Id);
            if (existing != null)
                _context.Entry(existing).CurrentValues.SetValues(entity);
            else
                await _context.Vocabularios.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteVocabularioAsync(List<VocabularioJapones> entities) {
        _logger.Information("Deletando {Count} vocabularios", entities.Count);
        _context.Vocabularios.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion
}
