using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegradorApi.Data.Migrations {
    /// <inheritdoc />
    public partial class ConvertEnumsToEnumDatabase : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("ALTER TABLE `conexoes` MODIFY COLUMN `fonte` ENUM('ORIGIN', 'DESTINATION') NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `conexoes` MODIFY COLUMN `integracao` ENUM('MANGA_EXTRACTOR', 'NOVEL_EXTRACTOR', 'COMICINFO', 'DECKSUBTITLE', 'TEXTO_INGLES', 'TEXTO_JAPONES') NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `conexoes` MODIFY COLUMN `conexao` ENUM('APIREST', 'MYSQL', 'POSTGRESSQL') NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("ALTER TABLE `conexoes` MODIFY COLUMN `fonte` INT NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `conexoes` MODIFY COLUMN `integracao` INT NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE `conexoes` MODIFY COLUMN `conexao` INT NOT NULL;");
        }
    }
}
