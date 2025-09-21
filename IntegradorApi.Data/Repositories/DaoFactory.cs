using IntegradorApi.Data.Repositories.Interfaces;
using IntegradorApi.Data.Repositories.Jdbc;
using MySqlConnector;

namespace IntegradorApi.Data.Repositories;

public static class DaoFactory {
    public static IMangaExtractorDao CreateMangaExtractorDao(MySqlConnection connection, string dbName) {
        return new MangaExtractorDaoJdbc(connection, dbName);
    }

    public static INovelExtractorDao CreateNovelExtractorDao(MySqlConnection connection, string dbName) {
        return new NovelExtractorDaoJdbc(connection, dbName);
    }
}