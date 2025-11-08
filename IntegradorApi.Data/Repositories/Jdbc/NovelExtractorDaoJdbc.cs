using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models.NovelExtractor;
using IntegradorApi.Data.Repositories.Interfaces;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace IntegradorApi.Data.Repositories.Jdbc;

public class NovelExtractorDaoJdbc : INovelExtractorDao {
    private readonly MySqlConnection _conn;
    private readonly string _dbName;

    public NovelExtractorDaoJdbc(MySqlConnection connection, string dbName) {
        _conn = connection;
        _dbName = dbName;
    }

    #region SQL Constants
    private const string UPDATE_VOLUMES_SQL = "UPDATE {0}_volumes SET novel = @novel, titulo = @titulo, titulo_alternativo = @titulo_alternativo, serie = @serie, descricao = @descricao, autor = @autor, editora = @editora, volume = @volume, linguagem = @linguagem, arquivo = @arquivo, is_processado = @is_processado, atualizacao = @atualizacao WHERE id = @id";
    private const string UPDATE_CAPITULOS_SQL = "UPDATE {0}_capitulos SET novel = @novel, volume = @volume, capitulo = @capitulo, descricao = @descricao, sequencia = @sequencia, linguagem = @linguagem, atualizacao = @atualizacao WHERE id = @id";
    private const string UPDATE_TEXTO_SQL = "UPDATE {0}_textos SET sequencia = @sequencia, texto = @texto, atualizacao = @atualizacao WHERE id = @id";
    private const string UPDATE_CAPA_SQL = "UPDATE {0}_capas SET novel = @novel, volume = @volume, linguagem = @linguagem, arquivo = @arquivo, extensao = @extensao, capa = @capa, atualizacao = @atualizacao WHERE id = @id";

    private const string INSERT_VOLUMES_SQL = "INSERT INTO {0}_volumes (id, novel, titulo, titulo_alternativo, serie, descricao, autor, editora, volume, linguagem, arquivo, is_processado, atualizacao) VALUES (@id, @novel, @titulo, @titulo_alternativo, @serie, @descricao, @autor, @editora, @volume, @linguagem, @arquivo, @is_processado, @atualizacao)";
    private const string INSERT_CAPITULOS_SQL = "INSERT INTO {0}_capitulos (id, id_volume, novel, volume, capitulo, descricao, sequencia, linguagem, atualizacao) VALUES (@id, @id_volume, @novel, @volume, @capitulo, @descricao, @sequencia, @linguagem, @atualizacao)";
    private const string INSERT_TEXTO_SQL = "INSERT INTO {0}_textos (id, id_capitulo, sequencia, texto, atualizacao) VALUES (@id, @id_capitulo, @sequencia, @texto, @atualizacao)";
    private const string INSERT_CAPA_SQL = "INSERT INTO {0}_capas (id, id_volume, novel, volume, linguagem, arquivo, extensao, capa, atualizacao) VALUES (@id, @id_volume, @novel, @volume, @linguagem, @arquivo, @extensao, @capa, @atualizacao)";

    private const string DELETE_VOLUMES_SQL = "CALL sp_delete_volume('{0}', '{1}')";
    private const string DELETE_CAPITULOS_SQL = "CALL sp_delete_capitulos('{0}', '{1}')";

    private const string SELECT_VOLUMES_SQL = "SELECT id, novel, titulo, titulo_alternativo, serie, descricao, editora, autor, volume, linguagem, arquivo, is_favorito, is_processado, atualizacao FROM {0}_volumes";
    private const string SELECT_CAPITULOS_SQL = "SELECT id, novel, volume, capitulo, descricao, sequencia, linguagem, atualizacao FROM {0}_capitulos WHERE id_volume = @id_volume";
    private const string SELECT_TEXTOS_SQL = "SELECT id, sequencia, texto, atualizacao FROM {0}_textos WHERE id_capitulo = @id_capitulo";
    private const string SELECT_CAPA_SQL = "SELECT id, novel, volume, linguagem, arquivo, extensao, capa, atualizacao FROM {0}_capas WHERE id_volume = @id_volume";

    private const string SELECT_VOLUME_SQL = "SELECT id, novel, titulo, titulo_alternativo, serie, descricao, editora, autor, volume, linguagem, arquivo, is_favorito, is_processado, atualizacao FROM {0}_volumes WHERE id = @id";
    private const string SELECT_CAPITULO_SQL = "SELECT id, novel, volume, capitulo, descricao, sequencia, linguagem, atualizacao FROM {0}_capitulos WHERE id = @id";
    private const string SELECT_TEXTO_SQL_BY_ID = "SELECT id, sequencia, texto, atualizacao FROM {0}_textos WHERE id = @id";

    private const string SELECT_VOCABULARIO_SQL = "SELECT id, palavra, portugues, ingles, leitura, revisado, d.atualizacao FROM {0}_vocabularios V INNER JOIN _vocabularios D ON V.id_vocabulario = D.id WHERE V.{1} = @id;";
    private const string INSERT_VOCABULARIO_SQL = "INSERT INTO {0}_vocabularios ({1}, id_vocabulario) VALUES (@id, @id_vocabulario);";
    private const string INSERT_VOCABULARIOS_SQL = "INSERT IGNORE INTO _vocabularios (id, palavra, portugues, ingles, leitura, revisado, atualizacao) VALUES (@id, @palavra, @portugues, @ingles, @leitura, @revisado, @atualizacao);";

    private const string WHERE_DATE_SYNC_SQL = " WHERE atualizacao >= @atualizacao";

    private const string CREATE_TABLE_SQL = "CALL sp_create_table(@tableName)";
    private const string EXISTS_TABLE_SQL = "SELECT fn_table_exists(@tableName)";
    private const string TABLES_SYNC_SQL = "CALL vw_sincronizacao()";
    private const string LAST_TABLES_SYNC_SQL = "CALL sp_sincronizacao(@since)";
    private const string EXISTS_VOLUME_SQL = "CALL sp_exists_volume(@tableName, @id)";
    #endregion

    #region Private Helper Methods
    private async Task<NovelVolume> GetVolumeFromReaderAsync(DbDataReader reader) {
        var id = reader.GetGuid("id");
        return new NovelVolume {
            Id = id,
            Novel = reader.GetString("novel"),
            Titulo = reader.GetString("titulo"),
            TituloAlternativo = reader.GetString("titulo_alternativo"),
            Serie = reader.IsDBNull("serie") ? string.Empty : reader.GetString("serie"),
            Descricao = reader.GetString("descricao"),
            Arquivo = reader.GetString("arquivo"),
            Editora = reader.GetString("editora"),
            Autor = reader.IsDBNull("autor") ? string.Empty : reader.GetString("autor"),
            Volume = reader.GetFloat("volume"),
            Lingua = Enum.Parse<Linguagens>(reader.GetString("linguagem"), true),
            Favorito = reader.GetBoolean("is_favorito"),
            Processado = reader.GetBoolean("is_processado"),
            Capa = await SelectCapaAsync(_dbName, id),
            Capitulos = await SelectAllCapitulosAsync(_dbName, id),
            Vocabularios = await SelectVocabularioAsync(_dbName, idVolume: id),
            Atualizacao = reader.GetDateTime("atualizacao")
        };
    }

    private async Task<NovelCapitulo> GetCapituloFromReaderAsync(DbDataReader reader) {
        var id = reader.GetGuid("id");
        return new NovelCapitulo {
            Id = id,
            Novel = reader.GetString("novel"),
            Volume = reader.GetFloat("volume"),
            Capitulo = reader.GetFloat("capitulo"),
            Descricao = reader.GetString("descricao"),
            Sequencia = reader.GetInt32("sequencia"),
            Lingua = Enum.Parse<Linguagens>(reader.GetString("linguagem"), true),
            Textos = await SelectAllTextosAsync(_dbName, id),
            Vocabularios = await SelectVocabularioAsync(_dbName, idCapitulo: id),
            Atualizacao = reader.GetDateTime("atualizacao")
        };
    }

    private NovelTexto GetTextoFromReader(DbDataReader reader) {
        return new NovelTexto {
            Id = reader.GetGuid("id"),
            Texto = reader.GetString("texto"),
            Sequencia = reader.GetInt32("sequencia"),
            Atualizacao = reader.GetDateTime("atualizacao")
        };
    }

    private NovelVocabulario GetVocabularioFromReader(DbDataReader reader) {
        return new NovelVocabulario {
            Id = reader.GetGuid("id"),
            Palavra = reader.GetString("palavra"),
            Leitura = reader.GetString("leitura"),
            Ingles = reader.GetString("ingles"),
            Portugues = reader.GetString("portugues"),
            Revisado = reader.GetBoolean("revisado"),
            Atualizacao = reader.GetDateTime("atualizacao")
        };
    }
    #endregion

    public async Task<NovelVolume> UpdateVolumeAsync(string dbName, NovelVolume obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_VOLUMES_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@novel", obj.Novel);
        command.Parameters.AddWithValue("@titulo", obj.Titulo);
        command.Parameters.AddWithValue("@tituloAlternativo", obj.TituloAlternativo);
        command.Parameters.AddWithValue("@serie", obj.Serie);
        command.Parameters.AddWithValue("@descricao", obj.Descricao);
        command.Parameters.AddWithValue("@autor", obj.Autor);
        command.Parameters.AddWithValue("@editora", obj.Editora);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@arquivo", obj.Arquivo);
        command.Parameters.AddWithValue("@isProcessado", obj.Processado);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();

        if (obj.Capa != null)
            await UpdateCapaAsync(dbName, obj.Capa);
        await InsertVocabularioAsync(dbName, idVolume: obj.Id, vocabulario: obj.Vocabularios);
        return obj;
    }

    public async Task<NovelCapa> UpdateCapaAsync(string dbName, NovelCapa obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_CAPA_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@novel", obj.Novel);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@arquivo", obj.Arquivo);
        command.Parameters.AddWithValue("@extensao", obj.Extenssao);
        command.Parameters.AddWithValue("@capa", obj.Imagem ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();
        return obj;
    }

    public async Task<NovelCapitulo> UpdateCapituloAsync(string dbName, NovelCapitulo obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_CAPITULOS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@novel", obj.Novel);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@capitulo", obj.Capitulo);
        command.Parameters.AddWithValue("@descricao", obj.Descricao);
        command.Parameters.AddWithValue("@sequencia", obj.Sequencia);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();

        await InsertVocabularioAsync(dbName, idCapitulo: obj.Id, vocabulario: obj.Vocabularios);
        return obj;
    }

    public async Task<NovelTexto> UpdateTextoAsync(string dbName, NovelTexto obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_TEXTO_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@sequencia", obj.Sequencia);
        command.Parameters.AddWithValue("@texto", obj.Texto);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();
        return obj;
    }

    public async Task<Guid?> InsertVolumeAsync(string dbName, NovelVolume obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_VOLUMES_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@novel", obj.Novel);
        command.Parameters.AddWithValue("@titulo", obj.Titulo);
        command.Parameters.AddWithValue("@tituloAlternativo", obj.TituloAlternativo);
        command.Parameters.AddWithValue("@serie", obj.Serie);
        command.Parameters.AddWithValue("@descricao", obj.Descricao);
        command.Parameters.AddWithValue("@autor", obj.Autor);
        command.Parameters.AddWithValue("@editora", obj.Editora);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@arquivo", obj.Arquivo);
        command.Parameters.AddWithValue("@isProcessado", obj.Processado);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();

        if (obj.Capa != null)
            await InsertCapaAsync(dbName, obj.Id, obj.Capa);
        await InsertVocabularioAsync(dbName, idVolume: obj.Id, vocabulario: obj.Vocabularios);

        return obj.Id;
    }

    public async Task<Guid?> InsertCapaAsync(string dbName, Guid idVolume, NovelCapa obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_CAPA_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@id_volume", idVolume.ToString());
        command.Parameters.AddWithValue("@novel", obj.Novel);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@arquivo", obj.Arquivo);
        command.Parameters.AddWithValue("@extensao", obj.Extenssao);
        command.Parameters.AddWithValue("@capa", obj.Imagem ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();
        return obj.Id;
    }

    public async Task<Guid?> InsertCapituloAsync(string dbName, Guid idVolume, NovelCapitulo obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_CAPITULOS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@id_volume", idVolume.ToString());
        command.Parameters.AddWithValue("@novel", obj.Novel);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@capitulo", obj.Capitulo);
        command.Parameters.AddWithValue("@descricao", obj.Descricao);
        command.Parameters.AddWithValue("@sequencia", obj.Sequencia);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();

        await InsertVocabularioAsync(dbName, idCapitulo: obj.Id, vocabulario: obj.Vocabularios);

        return obj.Id;
    }

    public async Task<Guid?> InsertTextoAsync(string dbName, Guid idCapitulo, NovelTexto obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_TEXTO_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@id_capitulo", idCapitulo.ToString());
        command.Parameters.AddWithValue("@sequencia", obj.Sequencia);
        command.Parameters.AddWithValue("@texto", obj.Texto);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();
        return obj.Id;
    }

    public async Task<NovelVolume?> SelectVolumeAsync(string dbName, Guid id) {
        await using var command = new MySqlCommand(string.Format(SELECT_VOLUME_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return await GetVolumeFromReaderAsync(reader);

        return null;
    }

    public async Task<NovelCapa?> SelectCapaAsync(string dbName, Guid idVolume) {
        await using var command = new MySqlCommand(string.Format(SELECT_CAPA_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id_volume", idVolume.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return new NovelCapa {
                Id = reader.GetGuid("id"),
                Novel = reader.GetString("novel"),
                Volume = reader.GetInt32("volume"),
                Lingua = Enum.Parse<Linguagens>(reader.GetString("linguagem"), true),
                Arquivo = reader.GetString("arquivo"),
                Extenssao = reader.GetString("extensao"),
                Imagem = reader.IsDBNull("capa") ? null : reader.GetFieldValue<byte[]>("capa"),
                Atualizacao = reader.GetDateTime("atualizacao")
            };
        }
        return null;
    }

    public async Task<NovelCapitulo?> SelectCapituloAsync(string dbName, Guid id) {
        await using var command = new MySqlCommand(string.Format(SELECT_CAPITULO_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return await GetCapituloFromReaderAsync(reader);

        return null;
    }

    public async Task<NovelTexto?> SelectTextoAsync(string dbName, Guid id) {
        await using var command = new MySqlCommand(string.Format(SELECT_TEXTO_SQL_BY_ID, dbName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return GetTextoFromReader(reader);

        return null;
    }

    public async Task<List<NovelVolume>> SelectAllVolumesAsync(string dbName) {
        var list = new List<NovelVolume>();
        await using var command = new MySqlCommand(string.Format(SELECT_VOLUMES_SQL, dbName), _conn);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(await GetVolumeFromReaderAsync(reader));

        return list;
    }

    public async Task<List<NovelVolume>> SelectAllVolumesAsync(string dbName, DateTime since) {
        var list = new List<NovelVolume>();
        var sql = string.Format(SELECT_VOLUMES_SQL, dbName) + WHERE_DATE_SYNC_SQL;
        await using var command = new MySqlCommand(sql, _conn);
        command.Parameters.AddWithValue("@atualizacao", since);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(await GetVolumeFromReaderAsync(reader));

        return list;
    }

    public async Task<List<NovelCapitulo>> SelectAllCapitulosAsync(string dbName, Guid idVolume) {
        var list = new List<NovelCapitulo>();
        await using var command = new MySqlCommand(string.Format(SELECT_CAPITULOS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id_volume", idVolume.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(await GetCapituloFromReaderAsync(reader));

        return list;
    }

    public async Task<List<NovelTexto>> SelectAllTextosAsync(string dbName, Guid idCapitulo) {
        var list = new List<NovelTexto>();
        await using var command = new MySqlCommand(string.Format(SELECT_TEXTOS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id_capitulo", idCapitulo.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            list.Add(GetTextoFromReader(reader));

        return list;
    }

    public async Task DeleteVolumeAsync(string dbName, NovelVolume obj) {
        await using var command = new MySqlCommand(string.Format(DELETE_VOLUMES_SQL, dbName, obj.Id.ToString()), _conn);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteCapituloAsync(string dbName, NovelCapitulo obj) {
        await using var command = new MySqlCommand(string.Format(DELETE_CAPITULOS_SQL, dbName, obj.Id.ToString()), _conn);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<HashSet<NovelVocabulario>> SelectVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null) {
        var list = new HashSet<NovelVocabulario>();
        string? campo = null;
        Guid? id = null;

        if (idVolume.HasValue) {
            campo = "id_volume";
            id = idVolume;
        } else if (idCapitulo.HasValue) {
            campo = "id_capitulo";
            id = idCapitulo;
        }

        if (string.IsNullOrEmpty(campo) || !id.HasValue) {
            return list;
        }

        await using var command = new MySqlCommand(string.Format(SELECT_VOCABULARIO_SQL, dbName, campo), _conn);
        command.Parameters.AddWithValue("@id", id.Value.ToString());

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            list.Add(GetVocabularioFromReader(reader));
        }
        return list;
    }

    public async Task InsertVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null, ICollection<NovelVocabulario>? vocabulario = null) {
        if (vocabulario == null || vocabulario.Count == 0 || (!idVolume.HasValue && !idCapitulo.HasValue)) {
            return;
        }

        string campo = idVolume.HasValue ? "id_volume" : "id_capitulo";
        Guid id = idVolume ?? idCapitulo.Value;

        await using var transaction = await _conn.BeginTransactionAsync();
        try {
            foreach (var vocab in vocabulario) {
                await using var cmdVocab = new MySqlCommand(INSERT_VOCABULARIOS_SQL, _conn, transaction);
                cmdVocab.Parameters.AddWithValue("@id", vocab.Id.ToString());
                cmdVocab.Parameters.AddWithValue("@palavra", vocab.Palavra);
                cmdVocab.Parameters.AddWithValue("@portugues", vocab.Portugues);
                cmdVocab.Parameters.AddWithValue("@ingles", vocab.Ingles);
                cmdVocab.Parameters.AddWithValue("@leitura", vocab.Leitura);
                cmdVocab.Parameters.AddWithValue("@revisado", vocab.Revisado);
                cmdVocab.Parameters.AddWithValue("@atualizacao", vocab.Atualizacao ?? (object)DBNull.Value);
                await cmdVocab.ExecuteNonQueryAsync();

                await using var cmdLink = new MySqlCommand(string.Format(INSERT_VOCABULARIO_SQL, dbName, campo), _conn, transaction);
                cmdLink.Parameters.AddWithValue("@id", id.ToString());
                cmdLink.Parameters.AddWithValue("@id_vocabulario", vocab.Id.ToString());
                await cmdLink.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
        } catch (Exception) {
            await transaction.RollbackAsync();
            throw;
        }
    }

    async Task INovelExtractorDao.CreateTableAsync(string tableName) {
        await using var command = new MySqlCommand(CREATE_TABLE_SQL, _conn);
        command.Parameters.AddWithValue("@tableName", tableName);
        await command.ExecuteNonQueryAsync();
    }

    async Task<bool> INovelExtractorDao.ExistTableAsync(string tableName) {
        await using var command = new MySqlCommand(EXISTS_TABLE_SQL, _conn);
        command.Parameters.AddWithValue("@tableName", tableName);
        var result = await command.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
            return false;

        return Convert.ToBoolean(result);
    }

    async Task<List<string>> INovelExtractorDao.GetTablesAsync() {
        var tables = new List<string>();
        await using var command = new MySqlCommand(TABLES_SYNC_SQL, _conn);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            tables.Add(reader.GetString(0));
        return tables;
    }

    public async Task<List<string>> GetTablesAsync(DateTime since) {
        var tables = new List<string>();
        await using var command = new MySqlCommand(LAST_TABLES_SYNC_SQL, _conn);
        command.Parameters.AddWithValue("@since", since);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            tables.Add(reader.GetString(0));

        return tables;
    }

    public async Task<bool> ExistVolumeAsync(string tableName, Guid id) {
        await using var command = new MySqlCommand(EXISTS_VOLUME_SQL, _conn);
        command.Parameters.AddWithValue("@tableName", tableName);
        command.Parameters.AddWithValue("@id", id.ToString());

        var result = await command.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
            return false;

        return Convert.ToBoolean(result);
    }
}