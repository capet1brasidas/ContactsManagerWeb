using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            string sqlQuery = @"
CREATE OR REPLACE FUNCTION insert_person(
    p_person_id uuid,
    p_person_name text,
    p_email text,
    p_date_of_birth timestamp without time zone,
    p_gender text,
    p_country_id uuid,
    p_address text,
    p_receive_newsletters boolean
)
RETURNS void
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO ""Persons""(
        ""PersonID"",
        ""PersonName"",
        ""Email"",
        ""DateOfBirth"",
        ""Gender"",
        ""CountryID"",
        ""Address"",
        ""ReceiveNewsLetters""
    )
    VALUES (
        p_person_id,
        p_person_name,
        p_email,
        p_date_of_birth,
        p_gender,
        p_country_id,
        p_address,
        p_receive_newsletters
    );
END;
$$;
";

            migrationBuilder.Sql(sqlQuery);
            

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DROP FUNCTION IF EXISTS insert_person();
    ");
        }
    }
}
