using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models.MangaExtractor;
using IntegradorApi.Data.Repositories.Interfaces;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace IntegradorApi.Data.Repositories.Jdbc;

public class MangaExtractorDaoJdbc : IMangaExtractorDao {
    private readonly MySqlConnection _conn;
    private readonly string _dbName;

    public MangaExtractorDaoJdbc(MySqlConnection connection, string dbName) {
        _conn = connection;
        _dbName = dbName;
    }

    #region SQL Constants
    private const string UPDATE_VOLUMES_SQL = "UPDATE {0}_volumes SET manga = @manga, volume = @volume, linguagem = @linguagem, arquivo = @arquivo, is_processado = @is_processado, atualizacao = @atualizacao WHERE id = @id";
    private const string UPDATE_CAPITULOS_SQL = "UPDATE {0}_capitulos SET manga = @manga, volume = @volume, capitulo = @capitulo, linguagem = @linguagem, scan = @scan, is_extra = @is_extra, is_raw = @is_raw, atualizacao = @atualizacao WHERE id = @id";
    private const string UPDATE_PAGINAS_SQL = "UPDATE {0}_paginas SET nome = @nome, numero = @numero, hash_pagina = @hash_pagina, atualizacao = @atualizacao WHERE id = @id";
    private const string UPDATE_TEXTO_SQL = "UPDATE {0}_textos SET sequencia = @sequencia, texto = @texto, posicao_x1 = @posicao_x1, posicao_y1 = @posicao_y1, posicao_x2 = @posicao_x2, posicao_y2 = @posicao_y2, atualizacao = @atualizacao WHERE id = @id";
    private const string UPDATE_CAPA_SQL = "UPDATE {0}_capas SET manga = @manga, volume = @volume, linguagem = @linguagem, arquivo = @arquivo, extensao = @extensao, capa = @capa, atualizacao = @atualizacao WHERE id = @id";

    private const string INSERT_VOLUMES_SQL = "INSERT INTO {0}_volumes (id, manga, volume, linguagem, arquivo, is_processado, atualizacao) VALUES (@id, @manga, @volume, @linguagem, @arquivo, @is_processado, @atualizacao)";
    private const string INSERT_CAPITULOS_SQL = "INSERT INTO {0}_capitulos (id, id_volume, manga, volume, capitulo, linguagem, scan, is_extra, is_raw, atualizacao) VALUES (@id, @id_volume, @manga, @volume, @capitulo, @linguagem, @scan, @is_extra, @is_raw, @atualizacao)";
    private const string INSERT_PAGINAS_SQL = "INSERT INTO {0}_paginas (id, id_capitulo, nome, numero, hash_pagina, atualizacao) VALUES (@id, @id_capitulo, @nome, @numero, @hash_pagina, @atualizacao)";
    private const string INSERT_TEXTO_SQL = "INSERT INTO {0}_textos (id, id_pagina, sequencia, texto, posicao_x1, posicao_y1, posicao_x2, posicao_y2, atualizacao) VALUES (@id, @id_pagina, @sequencia, @texto, @posicao_x1, @posicao_y1, @posicao_x2, @posicao_y2, @atualizacao)";
    private const string INSERT_CAPA_SQL = "INSERT INTO {0}_capas (id, id_volume, manga, volume, linguagem, arquivo, extensao, capa, atualizacao) VALUES (@id, @id_volume, @manga, @volume, @linguagem, @arquivo, @extensao, @capa, @atualizacao)";

    private const string DELETE_VOLUMES_SQL = "CALL sp_delete_volume('{0}', '{1}')";
    private const string DELETE_CAPITULOS_SQL = "CALL sp_delete_capitulos('{0}', '{1}')";

    private const string SELECT_VOLUMES_SQL = "SELECT id, manga, volume, linguagem, arquivo, is_processado, atualizacao FROM {0}_volumes";
    private const string SELECT_CAPITULOS_SQL = "SELECT id, manga, volume, capitulo, linguagem, scan, is_extra, is_raw, atualizacao FROM {0}_capitulos WHERE id_volume = @id_volume";
    private const string SELECT_PAGINAS_SQL = "SELECT id, nome, numero, hash_pagina, atualizacao FROM {0}_paginas WHERE id_capitulo = @id_capitulo";
    private const string SELECT_TEXTOS_SQL = "SELECT id, sequencia, texto, posicao_x1, posicao_y1, posicao_x2, posicao_y2, atualizacao FROM {0}_textos WHERE id_pagina = @id_pagina";
    private const string SELECT_CAPA_SQL = "SELECT id, manga, volume, linguagem, arquivo, extensao, capa, atualizacao FROM {0}_capas WHERE id_volume = @id_volume";

    private const string SELECT_VOLUME_SQL = "SELECT id, manga, volume, linguagem, arquivo, is_processado, atualizacao FROM {0}_volumes WHERE id = @id";
    private const string SELECT_CAPITULO_SQL = "SELECT id, manga, volume, capitulo, linguagem, scan, is_extra, is_raw, atualizacao FROM {0}_capitulos WHERE id = @id";
    private const string SELECT_PAGINA_SQL = "SELECT id, nome, numero, hash_pagina, atualizacao FROM {0}_paginas WHERE id = @id";
    private const string SELECT_TEXTO_SQL_BY_ID = "SELECT id, sequencia, texto, posicao_x1, posicao_y1, posicao_x2, posicao_y2, atualizacao FROM {0}_textos WHERE id = @id";

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
    private async Task<MangaVolume> GetVolumeFromReaderAsync(DbDataReader reader) {
        var id = reader.GetGuid("id");
        return new MangaVolume {
            Id = id,
            Manga = reader.GetString("manga"),
            Volume = reader.GetInt32("volume"),
            Lingua = Enum.Parse<Linguagens>(reader.GetString("linguagem"), true),
            Capitulos = await SelectAllCapitulosAsync(_dbName, id),
            Vocabularios = await SelectVocabularioAsync(_dbName, idVolume: id),
            Arquivo = reader.IsDBNull("arquivo") ? string.Empty : reader.GetString("arquivo"),
            Processado = reader.GetBoolean("is_processado"),
            Capa = await SelectCapaAsync(_dbName, id),
            Atualizacao = reader.GetDateTime("atualizacao")
        };
    }

    private async Task<MangaCapitulo> GetCapituloFromReaderAsync(DbDataReader reader) {
        var id = reader.GetGuid("id");
        return new MangaCapitulo {
            Id = id,
            Manga = reader.GetString("manga"),
            Volume = reader.GetInt32("volume"),
            Capitulo = reader.GetFloat("capitulo"),
            Lingua = Enum.Parse<Linguagens>(reader.GetString("linguagem"), true),
            Scan = reader.GetString("scan"),
            Paginas = await SelectAllPaginasAsync(_dbName, id),
            IsExtra = reader.GetBoolean("is_extra"),
            IsRaw = reader.GetBoolean("is_raw"),
            Vocabularios = await SelectVocabularioAsync(_dbName, idCapitulo: id),
            Atualizacao = reader.GetDateTime("atualizacao")
        };
    }

    private async Task<MangaPagina> GetPaginaFromReaderAsync(DbDataReader reader) {
        var id = reader.GetGuid("id");
        return new MangaPagina {
            Id = id,
            NomePagina = reader.GetString("nome"),
            Numero = reader.GetInt32("numero"),
            Hash = reader.GetString("hash_pagina"),
            Textos = await SelectAllTextosAsync(_dbName, id),
            Vocabularios = await SelectVocabularioAsync(_dbName, idPagina: id),
            Atualizacao = reader.GetDateTime("atualizacao")
        };
    }

    private MangaTexto GetTextoFromReader(DbDataReader reader) {
        return new MangaTexto {
            Id = reader.GetGuid("id"),
            Texto = reader.GetString("texto"),
            Sequencia = reader.GetInt32("sequencia"),
            X1 = reader.GetInt32("posicao_x1"),
            Y1 = reader.GetInt32("posicao_y1"),
            X2 = reader.GetInt32("posicao_x2"),
            Y2 = reader.GetInt32("posicao_y2"),
            Atualizacao = reader.GetDateTime("atualizacao")
        };
    }

    private MangaVocabulario GetVocabularioFromReader(DbDataReader reader) {
        return new MangaVocabulario {
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

    public async Task<MangaVolume> UpdateVolumeAsync(string dbName, MangaVolume obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_VOLUMES_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@manga", obj.Manga);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@arquivo", obj.Arquivo);
        command.Parameters.AddWithValue("@is_processado", obj.Processado);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();

        if (obj.Capa != null)
            await UpdateCapaAsync(dbName, obj.Capa);
        await InsertVocabularioAsync(dbName, idVolume: obj.Id, vocabulario: obj.Vocabularios);
        return obj;
    }

    public async Task<MangaCapa> UpdateCapaAsync(string dbName, MangaCapa obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_CAPA_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@manga", obj.Manga);
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

    public async Task<MangaCapitulo> UpdateCapituloAsync(string dbName, MangaCapitulo obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_CAPITULOS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@manga", obj.Manga);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@capitulo", obj.Capitulo);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@scan", obj.Scan);
        command.Parameters.AddWithValue("@is_extra", obj.IsExtra);
        command.Parameters.AddWithValue("@is_raw", obj.IsRaw);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();

        await InsertVocabularioAsync(dbName, idCapitulo: obj.Id, vocabulario: obj.Vocabularios);
        return obj;
    }

    public async Task<MangaPagina> UpdatePaginaAsync(string dbName, MangaPagina obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_PAGINAS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@nome", obj.NomePagina);
        command.Parameters.AddWithValue("@numero", obj.Numero);
        command.Parameters.AddWithValue("@hash_pagina", obj.Hash);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();

        await InsertVocabularioAsync(dbName, idPagina: obj.Id, vocabulario: obj.Vocabularios);
        return obj;
    }

    public async Task<MangaTexto> UpdateTextoAsync(string dbName, MangaTexto obj) {
        await using var command = new MySqlCommand(string.Format(UPDATE_TEXTO_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@sequencia", obj.Sequencia);
        command.Parameters.AddWithValue("@texto", obj.Texto);
        command.Parameters.AddWithValue("@posicao_x1", obj.X1);
        command.Parameters.AddWithValue("@posicao_y1", obj.Y1);
        command.Parameters.AddWithValue("@posicao_x2", obj.X2);
        command.Parameters.AddWithValue("@posicao_y2", obj.Y2);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        await command.ExecuteNonQueryAsync();
        return obj;
    }

    public async Task<Guid?> InsertVolumeAsync(string dbName, MangaVolume obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_VOLUMES_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@manga", obj.Manga);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@arquivo", obj.Arquivo);
        command.Parameters.AddWithValue("@is_processado", obj.Processado);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();

        if (obj.Capa != null)
            await InsertCapaAsync(dbName, obj.Id, obj.Capa);
        await InsertVocabularioAsync(dbName, idVolume: obj.Id, vocabulario: obj.Vocabularios);

        return obj.Id;
    }

    public async Task<Guid?> InsertCapaAsync(string dbName, Guid idVolume, MangaCapa obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_CAPA_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@id_volume", idVolume.ToString());
        command.Parameters.AddWithValue("@manga", obj.Manga);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@arquivo", obj.Arquivo);
        command.Parameters.AddWithValue("@extensao", obj.Extenssao);
        command.Parameters.AddWithValue("@capa", obj.Imagem ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();
        return obj.Id;
    }

    public async Task<Guid?> InsertCapituloAsync(string dbName, Guid idVolume, MangaCapitulo obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_CAPITULOS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@id_volume", idVolume.ToString());
        command.Parameters.AddWithValue("@manga", obj.Manga);
        command.Parameters.AddWithValue("@volume", obj.Volume);
        command.Parameters.AddWithValue("@capitulo", obj.Capitulo);
        command.Parameters.AddWithValue("@linguagem", obj.Lingua.ToString());
        command.Parameters.AddWithValue("@scan", obj.Scan);
        command.Parameters.AddWithValue("@is_extra", obj.IsExtra);
        command.Parameters.AddWithValue("@is_raw", obj.IsRaw);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();

        await InsertVocabularioAsync(dbName, idCapitulo: obj.Id, vocabulario: obj.Vocabularios);

        return obj.Id;
    }

    public async Task<Guid?> InsertPaginaAsync(string dbName, Guid idCapitulo, MangaPagina obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_PAGINAS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@id_capitulo", idCapitulo.ToString());
        command.Parameters.AddWithValue("@nome", obj.NomePagina);
        command.Parameters.AddWithValue("@numero", obj.Numero);
        command.Parameters.AddWithValue("@hash_pagina", obj.Hash);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();

        await InsertVocabularioAsync(dbName, idPagina: obj.Id, vocabulario: obj.Vocabularios);

        return obj.Id;
    }

    public async Task<Guid?> InsertTextoAsync(string dbName, Guid idPagina, MangaTexto obj) {
        await using var command = new MySqlCommand(string.Format(INSERT_TEXTO_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", obj.Id.ToString());
        command.Parameters.AddWithValue("@id_pagina", idPagina.ToString());
        command.Parameters.AddWithValue("@sequencia", obj.Sequencia);
        command.Parameters.AddWithValue("@texto", obj.Texto);
        command.Parameters.AddWithValue("@posicao_x1", obj.X1);
        command.Parameters.AddWithValue("@posicao_y1", obj.Y1);
        command.Parameters.AddWithValue("@posicao_x2", obj.X2);
        command.Parameters.AddWithValue("@posicao_y2", obj.Y2);
        command.Parameters.AddWithValue("@atualizacao", obj.Atualizacao ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();
        return obj.Id;
    }

    public async Task<MangaVolume?> SelectVolumeAsync(string dbName, Guid id) {
        await using var command = new MySqlCommand(string.Format(SELECT_VOLUME_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return await GetVolumeFromReaderAsync(reader);
        }
        return null;
    }

    public async Task<MangaCapa?> SelectCapaAsync(string dbName, Guid idVolume) {
        await using var command = new MySqlCommand(string.Format(SELECT_CAPA_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id_volume", idVolume.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return new MangaCapa {
                Id = reader.GetGuid("id"),
                Manga = reader.GetString("manga"),
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

    public async Task<MangaCapitulo?> SelectCapituloAsync(string dbName, Guid id) {
        await using var command = new MySqlCommand(string.Format(SELECT_CAPITULO_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return await GetCapituloFromReaderAsync(reader);
        }
        return null;
    }

    public async Task<MangaPagina?> SelectPaginaAsync(string dbName, Guid id) {
        await using var command = new MySqlCommand(string.Format(SELECT_PAGINA_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return await GetPaginaFromReaderAsync(reader);
        }
        return null;
    }

    public async Task<MangaTexto?> SelectTextoAsync(string dbName, Guid id) {
        await using var command = new MySqlCommand(string.Format(SELECT_TEXTO_SQL_BY_ID, dbName), _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return GetTextoFromReader(reader);
        }
        return null;
    }

    public async Task<List<MangaVolume>> SelectAllVolumesAsync(string dbName) {
        var list = new List<MangaVolume>();
        await using var command = new MySqlCommand(string.Format(SELECT_VOLUMES_SQL, dbName), _conn);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            list.Add(await GetVolumeFromReaderAsync(reader));
        }
        return list;
    }

    public async Task<List<MangaVolume>> SelectAllVolumesAsync(string dbName, DateTime since) {
        var list = new List<MangaVolume>();
        var sql = string.Format(SELECT_VOLUMES_SQL, dbName) + WHERE_DATE_SYNC_SQL;
        await using var command = new MySqlCommand(sql, _conn);
        command.Parameters.AddWithValue("@atualizacao", since);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            list.Add(await GetVolumeFromReaderAsync(reader));
        }
        return list;
    }

    public async Task<List<MangaCapitulo>> SelectAllCapitulosAsync(string dbName, Guid idVolume) {
        var list = new List<MangaCapitulo>();
        await using var command = new MySqlCommand(string.Format(SELECT_CAPITULOS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id_volume", idVolume.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            list.Add(await GetCapituloFromReaderAsync(reader));
        }
        return list;
    }

    public async Task<List<MangaPagina>> SelectAllPaginasAsync(string dbName, Guid idCapitulo) {
        var list = new List<MangaPagina>();
        await using var command = new MySqlCommand(string.Format(SELECT_PAGINAS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id_capitulo", idCapitulo.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            list.Add(await GetPaginaFromReaderAsync(reader));
        }
        return list;
    }

    public async Task<List<MangaTexto>> SelectAllTextosAsync(string dbName, Guid idPagina) {
        var list = new List<MangaTexto>();
        await using var command = new MySqlCommand(string.Format(SELECT_TEXTOS_SQL, dbName), _conn);
        command.Parameters.AddWithValue("@id_pagina", idPagina.ToString());
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            list.Add(GetTextoFromReader(reader));
        }
        return list;
    }

    public async Task DeleteVolumeAsync(string dbName, MangaVolume obj) {
        await using var command = new MySqlCommand(string.Format(DELETE_VOLUMES_SQL, dbName, obj.Id.ToString()), _conn);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteCapituloAsync(string dbName, MangaCapitulo obj) {
        await using var command = new MySqlCommand(string.Format(DELETE_CAPITULOS_SQL, dbName, obj.Id.ToString()), _conn);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<HashSet<MangaVocabulario>> SelectVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null, Guid? idPagina = null) {
        var list = new HashSet<MangaVocabulario>();
        string? campo = null;
        Guid? id = null;

        if (idVolume.HasValue) {
            campo = "id_volume";
            id = idVolume;
        } else if (idCapitulo.HasValue) {
            campo = "id_capitulo";
            id = idCapitulo;
        } else if (idPagina.HasValue) {
            campo = "id_pagina";
            id = idPagina;
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

    public async Task InsertVocabularioAsync(string dbName, Guid? idVolume = null, Guid? idCapitulo = null, Guid? idPagina = null, ICollection<MangaVocabulario>? vocabulario = null) {
        if (vocabulario == null || vocabulario.Count == 0 || (!idVolume.HasValue && !idCapitulo.HasValue && !idPagina.HasValue)) {
            return;
        }

        string campo = idVolume.HasValue ? "id_volume" : (idCapitulo.HasValue ? "id_capitulo" : "id_pagina");
        Guid id = idVolume ?? idCapitulo ?? idPagina.Value;

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

    async Task IMangaExtractorDao.CreateTableAsync(string tableName) {
        await using var command = new MySqlCommand(CREATE_TABLE_SQL, _conn);
        command.Parameters.AddWithValue("@tableName", tableName);
        await command.ExecuteNonQueryAsync();
    }

    async Task<bool> IMangaExtractorDao.ExistTableAsync(string tableName) {
        await using var command = new MySqlCommand(EXISTS_TABLE_SQL, _conn);
        command.Parameters.AddWithValue("@tableName", tableName);
        var result = await command.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
            return false;

        return Convert.ToBoolean(result);
    }

    async Task<List<string>> IMangaExtractorDao.GetTablesAsync() {
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