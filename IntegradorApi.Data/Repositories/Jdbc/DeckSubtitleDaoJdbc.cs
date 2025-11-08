using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models.DeckSubtitle;
using IntegradorApi.Data.Repositories.Interfaces;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace IntegradorApi.Data.Repositories.Jdbc;

public class DeckSubtitleDaoJdbc : IDeckSubtitleDao {
    private readonly MySqlConnection _conn;

    public DeckSubtitleDaoJdbc(MySqlConnection connection) {
        _conn = connection;
    }

    #region SQL Constants
    private const string INSERT_SQL = "INSERT INTO {0} (ID, Episodio, Linguagem, TempoInicial, TempoFinal, Texto, Traducao, Vocabulario) VALUES (@id, @episodio, @linguagem, @tempoInicial, @tempoFinal, @texto, @traducao, @vocabulario);";
    private const string UPDATE_SQL = "UPDATE {0} SET Episodio = @episodio, Linguagem = @linguagem, TempoInicial = @tempoInicial, TempoFinal = @tempoFinal, Texto = @texto, Traducao = @traducao, Vocabulario = @vocabulario WHERE id = @id;";
    private const string SELECT_SQL = "SELECT id, Episodio, Linguagem, TempoInicial, TempoFinal, Texto, Traducao, Vocabulario FROM {0} WHERE id = @id;";
    private const string DELETE_SQL = "DELETE FROM {0} WHERE id = @id;";
    private const string SELECT_ALL_SQL = "SELECT id, Episodio, Linguagem, TempoInicial, TempoFinal, Texto, Traducao, Vocabulario FROM {0}";
    private const string WHERE_DATE_SYNC_SQL = " WHERE atualizacao >= @atualizacao";
    private const string SELECT_LISTA_TABELAS_SQL = "SELECT Table_Name AS Tabela FROM information_schema.tables WHERE table_schema = @dbName GROUP BY Tabela";
    private const string FIND_BY_ID_SQL = "SELECT id, Episodio FROM {0} WHERE id = @id;";
    private const string CREATE_TABLE_SQL = "CALL create_table('{0}');";
    private const string TABLES_SYNC_SQL = "CALL sp_sincronizacao(@since)";
    private const string EXISTS_TABLE_SQL = "SELECT fn_table_exists(@tableName)";
    #endregion

    public async Task UpdateAsync(string dbName, Subtitle obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_SQL, dbName), _conn);
        AddParameters(command, obj);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();
    }

    public async Task<Guid?> InsertAsync(string dbName, Subtitle obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        AddParameters(command, obj);
        await command.ExecuteNonQueryAsync();
        return obj.Id;
    }

    public async Task<Subtitle?> SelectAsync(string dbName, Guid id) {
        await using var command = new MySqlCommand(string.Format(SELECT_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return MapReaderToSubtitle(reader);
        }
        return null;
    }

    public async Task<List<Subtitle>> SelectAllAsync(string dbName) {
        var result = new List<Subtitle>();
        await using var command = new MySqlCommand(string.Format(SELECT_ALL_SQL, dbName), _conn);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            result.Add(MapReaderToSubtitle(reader));
        }
        return result;
    }

    public async Task<List<Subtitle>> SelectAllAsync(string dbName, DateTime dateTime) {
        var result = new List<Subtitle>();
        var sql = string.Format(SELECT_ALL_SQL, dbName) + WHERE_DATE_SYNC_SQL;
        await using var command = new MySqlCommand(sql, _conn);
        command.Parameters.AddWithValue("@atualizacao", dateTime);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            result.Add(MapReaderToSubtitle(reader));
        }
        return result;
    }

    public async Task DeleteAsync(string dbName, Subtitle obj) {
        await using var command = new MySqlCommand(string.Format(DELETE_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();
    }

    public async Task CreateTableAsync(string tableName) {
        await using var command = new MySqlCommand(string.Format(CREATE_TABLE_SQL, tableName), _conn);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<string>> GetTablesAsync() {
        var result = new List<string>();
        await using var command = new MySqlCommand(SELECT_LISTA_TABELAS_SQL, _conn);
        command.Parameters.AddWithValue("@dbName", _conn.Database);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            result.Add(reader.GetString("Tabela"));
        }
        return result;
    }

    private void AddParameters(MySqlCommand command, Subtitle obj) {
        command.Parameters.AddWithValue("@episodio", obj.Episodio);
        command.Parameters.AddWithValue("@linguagem", obj.Linguagem.ToString());
        command.Parameters.AddWithValue("@tempoInicial", obj.Tempo);
        command.Parameters.AddWithValue("@tempoFinal", null);
        command.Parameters.AddWithValue("@texto", obj.Texto);
        command.Parameters.AddWithValue("@traducao", obj.Traducao);
        command.Parameters.AddWithValue("@vocabulario", obj.Vocabulario ?? (object)DBNull.Value);
    }

    private Subtitle MapReaderToSubtitle(DbDataReader reader) {
        return new Subtitle(reader.GetGuid("id")) {
            Episodio = reader.GetInt32("Episodio"),
            Linguagem = Enum.Parse<Linguagens>(reader.GetString("Linguagem"), true),
            Tempo = reader.GetString("TempoInicial"),
            Texto = reader.GetString("Texto"),
            Traducao = reader.GetString("Traducao"),
            Vocabulario = reader.IsDBNull("Vocabulario") ? null : reader.GetString("Vocabulario")
        };
    }

    public async Task<bool> ExistTableAsync(string tableName) {
        await using var command = new MySqlCommand(EXISTS_TABLE_SQL, _conn);
        command.Parameters.AddWithValue("@tableName", tableName);
        var result = await command.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
            return false;

        return Convert.ToBoolean(result);
    }

    public async Task<bool> ExistAsync(string tableName, Guid id) {
        await using var command = new MySqlCommand(string.Format(FIND_BY_ID_SQL, tableName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        return reader.HasRows;
    }

    public async Task<List<string>> GetTablesAsync(DateTime since) {
        var tables = new List<string>();
        await using var command = new MySqlCommand(TABLES_SYNC_SQL, _conn);
        command.Parameters.AddWithValue("@since", since);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            tables.Add(reader.GetString(0));

        return tables;
    }
}