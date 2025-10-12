using IntegradorApi.Data.Enums.ComicInfo;
using IntegradorApi.Data.Models.ProcessaTexto;
using IntegradorApi.Data.Repositories.Interfaces;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace IntegradorApi.Data.Repositories.Jdbc;

public class ComicInfoDaoJdbc : IComicInfoDao {
    private readonly MySqlConnection _conn;

    public ComicInfoDaoJdbc(MySqlConnection connection) {
        _conn = connection;
    }

    #region SQL Constants
    private const string INSERT_SQL = "INSERT IGNORE INTO comicinfo (id, comic, idMal, series, title, publisher, genre, imprint, seriesGroup, storyArc, maturityRating, alternativeSeries, language) VALUES (@id, @comic, @idMal, @series, @title, @publisher, @genre, @imprint, @seriesGroup, @storyArc, @maturityRating, @alternativeSeries, @language);";
    private const string UPDATE_SQL = "UPDATE comicinfo SET comic = @comic, idMal = @idMal, series = @series, title = @title, publisher = @publisher, genre = @genre, imprint = @imprint, seriesGroup = @seriesGroup, storyArc = @storyArc, maturityRating = @maturityRating, alternativeSeries = @alternativeSeries, language = @language WHERE id = @id;";
    private const string SELECT_BY_COMIC_AND_LANGUAGE_SQL = "SELECT id, comic, idMal, series, title, publisher, genre, imprint, seriesGroup, storyArc, maturityRating, alternativeSeries, language FROM comicinfo WHERE comic like @comic AND language = @language;";
    private const string SELECT_BY_ID_OR_COMIC_SQL = "SELECT id, comic, idMal, series, title, publisher, genre, imprint, seriesGroup, storyArc, maturityRating, alternativeSeries, language FROM comicinfo WHERE id = @id OR (language = @language AND (UPPER(comic) LIKE @searchTerm or UPPER(series) LIKE @searchTerm or UPPER(title) LIKE @searchTerm));";
    private const string FIND_FOR_UPDATE_SQL = "SELECT id, comic, idMal, series, title, publisher, genre, imprint, seriesGroup, storyArc, maturityRating, alternativeSeries, language FROM comicinfo WHERE atualizacao >= @lastUpdate";
    #endregion

    public async Task SaveAsync(ComicInfo obj) {
        if (obj.Id == null)
            obj.Id = Guid.NewGuid();

        await using var updateCommand = new MySqlCommand(UPDATE_SQL, _conn);
        AddParameters(updateCommand, obj);
        var rowsAffected = await updateCommand.ExecuteNonQueryAsync();

        if (rowsAffected == 0) {
            await using var insertCommand = new MySqlCommand(INSERT_SQL, _conn);
            AddParameters(insertCommand, obj);
            await insertCommand.ExecuteNonQueryAsync();
        }
    }

    public async Task<ComicInfo?> FindAsync(string comic, string linguagem) {
        await using var command = new MySqlCommand(SELECT_BY_COMIC_AND_LANGUAGE_SQL, _conn);
        command.Parameters.AddWithValue("@comic", $"%{comic}%");
        command.Parameters.AddWithValue("@language", linguagem);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return MapReaderToComicInfo(reader);
        }
        return null;
    }

    public async Task<ComicInfo?> FindAsync(Guid id, string comic, string linguagem) {
        await using var command = new MySqlCommand(SELECT_BY_ID_OR_COMIC_SQL, _conn);
        command.Parameters.AddWithValue("@id", id.ToString());
        command.Parameters.AddWithValue("@language", linguagem);
        command.Parameters.AddWithValue("@searchTerm", $"%{comic.ToUpper()}%");

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync()) {
            return MapReaderToComicInfo(reader);
        }
        return null;
    }

    public async Task<List<ComicInfo>> FindForUpdateAsync(DateTime lastUpdate) {
        var result = new List<ComicInfo>();
        await using var command = new MySqlCommand(FIND_FOR_UPDATE_SQL, _conn);
        command.Parameters.AddWithValue("@lastUpdate", lastUpdate);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
            result.Add(MapReaderToComicInfo(reader));
        }
        return result;
    }

    private void AddParameters(MySqlCommand command, ComicInfo obj) {
        command.Parameters.AddWithValue("@id", obj.Id?.ToString());
        command.Parameters.AddWithValue("@comic", obj.Comic);
        command.Parameters.AddWithValue("@idMal", obj.IdMal ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@series", obj.Series);
        command.Parameters.AddWithValue("@title", obj.Title);
        command.Parameters.AddWithValue("@publisher", obj.Publisher ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@genre", obj.Genre ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@imprint", obj.Imprint ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@seriesGroup", obj.SeriesGroup ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@storyArc", obj.StoryArc ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@maturityRating", obj.AgeRating?.ToString() ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@alternativeSeries", obj.AlternateSeries ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@language", obj.LanguageISO);
    }

    private ComicInfo MapReaderToComicInfo(DbDataReader reader) {
        return new ComicInfo {
            Id = reader.GetGuid("id"),
            Comic = reader.GetString("comic"),
            IdMal = reader.IsDBNull("idMal") ? null : reader.GetInt64("idMal"),
            Series = reader.GetString("series"),
            Title = reader.GetString("title"),
            Publisher = reader.IsDBNull("publisher") ? null : reader.GetString("publisher"),
            Genre = reader.IsDBNull("genre") ? null : reader.GetString("genre"),
            Imprint = reader.IsDBNull("imprint") ? null : reader.GetString("imprint"),
            SeriesGroup = reader.IsDBNull("seriesGroup") ? null : reader.GetString("seriesGroup"),
            StoryArc = reader.IsDBNull("storyArc") ? null : reader.GetString("storyArc"),
            AgeRating = reader.IsDBNull("maturityRating") ? null : Enum.Parse<AgeRating>(reader.GetString("maturityRating"), true),
            AlternateSeries = reader.IsDBNull("alternativeSeries") ? null : reader.GetString("alternativeSeries"),
            LanguageISO = reader.GetString("language")
        };
    }
}