using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class getpersons_storedprocedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sqlQuery = @"
        create or replace function get_all_persons()
    returns TABLE(
        ""PersonID"" uuid,
        ""PersonName"" text,
        ""Email"" text,
        ""DateOfBirth"" timestamp without time zone,
        ""Gender"" text,
        ""CountryID"" uuid,
        ""Address"" text,
        ""ReceiveNewsLetters"" boolean,
        ""TIN"" text
    )
    language sql
as
$$
SELECT
    ""PersonID"",
    ""PersonName""::text,
    ""Email""::text,
    ""DateOfBirth"",
    ""Gender""::text,
    ""CountryID"",
    ""Address""::text,
    ""ReceiveNewsLetters"",
    ""TIN""::text
FROM ""Persons"";
$$;
    ";
            
            migrationBuilder.Sql(sqlQuery);
            
   

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DROP FUNCTION IF EXISTS get_all_persons();
    ");

        }
    }
}
