using IntegradorApi.Data.Models;
using IntegradorApi.Data.Services;
using Serilog;

namespace IntegradorApi.Sync.Interfaces;

public interface ISyncRunner {
    Type EntityType { get; }
    string ResourceName { get; }
    Task RunSyncAsync(IEnumerable<ISyncRunner> destinationServices, Connection connectionOrigin, DatabaseService databaseService, ILogger logger);
}
