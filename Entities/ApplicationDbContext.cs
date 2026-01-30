using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Entities
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Country>().ToTable("Countries");
      modelBuilder.Entity<Person>().ToTable("Persons");
      
      modelBuilder.Entity<Person>()
        .Property(p => p.DateOfBirth)
        .HasColumnType("timestamp without time zone"); // 或者 "date"
      
      string countriesJson =  File.ReadAllText("countries.json"); 
      
      List<Country> Countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

      foreach (Country country in Countries)
      {
        modelBuilder.Entity<Country>().HasData(country);
      }
      
      string personsJson =  File.ReadAllText("persons.json"); 
      
      List<Person> Persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

      foreach (Person person in Persons)
      {
        modelBuilder.Entity<Person>().HasData(person);
        
        //fluent api
        modelBuilder.Entity<Person>().Property(temp => temp.TIN).HasColumnName("TaxIdentificationNumber")
          .HasColumnType("varchar(8)")
          .HasDefaultValue("ABC12345");

        modelBuilder.Entity<Person>(entity =>
        {
          entity.HasOne<Country>(p => p.Country).WithMany(p => p.Persons)
            .HasForeignKey(p => p.CountryID);
        });
      }

      // modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique();
      modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "char_length(\"TaxIdentificationNumber\") = 8");
    }
    
    public List<Person> sp_GetAllPersons()
    {
      // PostgreSQL function created by your migration:
      //   CREATE OR REPLACE FUNCTION get_all_persons() RETURNS TABLE (...)
      // Call it and map the result back to the Person entity
    

      // Convert to response DTOs (and enrich with Country name)
      return Persons
        .FromSqlRaw("SELECT * FROM get_all_persons()").ToList();
    }
    
    public int sp_InserPerson(Person person)
    {
      NpgsqlParameter[] parameters =  new NpgsqlParameter[]
      {
        new NpgsqlParameter("PersonID", NpgsqlDbType.Uuid) { Value = person.PersonID },
        new NpgsqlParameter("PersonName", NpgsqlDbType.Text) { Value = (object?)person.PersonName ?? DBNull.Value },
        new NpgsqlParameter("Email", NpgsqlDbType.Text) { Value = (object?)person.Email ?? DBNull.Value },
        new NpgsqlParameter("DateOfBirth", NpgsqlDbType.Timestamp) { Value = (object?)person.DateOfBirth ?? DBNull.Value },
        new NpgsqlParameter("Gender", NpgsqlDbType.Text) { Value = (object?)person.Gender ?? DBNull.Value },
        new NpgsqlParameter("CountryID", NpgsqlDbType.Uuid) { Value = (object?)person.CountryID ?? DBNull.Value },
        new NpgsqlParameter("Address", NpgsqlDbType.Text) { Value = (object?)person.Address ?? DBNull.Value },
        new NpgsqlParameter("ReceiveNewsLetters", NpgsqlDbType.Boolean) { Value = person.ReceiveNewsLetters }
      }; 
      return Database.ExecuteSqlRaw("select insert_person(@PersonID, @PersonName, @Email," +
                                    " @DateOfBirth, @Gender,  @CountryID ,@Address, @ReceiveNewsLetters);"
        , parameters);
      
    }
  }
}
