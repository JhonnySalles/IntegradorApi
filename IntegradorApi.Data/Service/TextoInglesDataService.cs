using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IntegradorApi.Data.Data;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.TextoIngles;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IntegradorApi.Data.Services;

public class TextoInglesDataService : IAsyncDisposable {
    private readonly TextoInglesDbContext _context;
    private readonly ILogger _logger;

    public TextoInglesDataService(Connection connection, ILogger logger) {
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
        var optionsBuilder = new DbContextOptionsBuilder<TextoInglesDbContext>();
        
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        _context = new TextoInglesDbContext(optionsBuilder.Options);
    }

    public async ValueTask DisposeAsync() {
        if (_context != null)
            await _context.DisposeAsync();
    }

    #region VocabularioIngles
    public async Task<List<VocabularioIngles>> SelectAllVocabularioAsync(DateTime since) {
        return await _context.Vocabularios.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveVocabularioAsync(List<VocabularioIngles> entities) {
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

    public async Task DeleteVocabularioAsync(List<VocabularioIngles> entities) {
        _logger.Information("Deletando {Count} vocabularios", entities.Count);
        _context.Vocabularios.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region ValidoIngles
    public async Task<List<ValidoIngles>> SelectAllValidoAsync(DateTime since) {
        return await _context.Validos.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveValidoAsync(List<ValidoIngles> entities) {
        _logger.Information("Salvando {Count} validos", entities.Count);
        foreach (var entity in entities) {
            var existing = await _context.Validos.FindAsync(entity.Id);
            if (existing != null)
                _context.Entry(existing).CurrentValues.SetValues(entity);
            else
                await _context.Validos.AddAsync(entity);
        }
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    public async Task DeleteValidoAsync(List<ValidoIngles> entities) {
        _logger.Information("Deletando {Count} validos", entities.Count);
        _context.Validos.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region RevisarIngles
    public async Task<List<RevisarIngles>> SelectAllRevisarAsync(DateTime since) {
        return await _context.Revisar.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveRevisarAsync(List<RevisarIngles> entities) {
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

    public async Task DeleteRevisarAsync(List<RevisarIngles> entities) {
        _logger.Information("Deletando {Count} revisar", entities.Count);
        _context.Revisar.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region ExclusaoIngles
    public async Task<List<ExclusaoIngles>> SelectAllExclusaoAsync(DateTime since) {
        return await _context.Exclusoes.AsNoTracking().Where(x => x.Atualizacao >= since).ToListAsync();
    }

    public async Task SaveExclusaoAsync(List<ExclusaoIngles> entities) {
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

    public async Task DeleteExclusaoAsync(List<ExclusaoIngles> entities) {
        _logger.Information("Deletando {Count} exclusoes", entities.Count);
        _context.Exclusoes.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    #endregion
}
