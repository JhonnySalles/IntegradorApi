using IntegradorApi.Data;
using IntegradorApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegradorApi.Services;

public class DatabaseService {
    public async Task<List<Connections>> GetConnectionsAsync() {
        await using var context = new AppDbContext();
        return await context.DbSetConnections.ToListAsync();
    }

    public async Task AddConnectionAsync(Connections novaConexao) {
        await using var context = new AppDbContext();
        context.DbSetConnections.Add(novaConexao);
        await context.SaveChangesAsync();
    }

    // Você pode adicionar outros métodos aqui (Update, Delete, GetRegistroSincronizacao, etc.)
}
