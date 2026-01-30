using System.Linq.Expressions;
using Entities;

namespace RepositoryContracts;

/// <summary>
/// Represents an abstraction for a repository that manages operations related to persons.
/// </summary>
public interface IPersonsRepository
{
    /// <summary>
    /// Adds a new person to the repository.
    /// </summary>
    /// <param name="person">The person entity to be added.</param>
    /// <returns>Returns the added person entity.</returns>
    Task<Person> AddPerson(Person person);

    /// <summary>
    /// Retrieves all persons from the repository.
    /// </summary>
    /// <returns>Returns a list of person entities.</returns>
    Task<List<Person>> GetAllPersons();


    /// <summary>
    /// Retrieves a person from the repository based on the specified person ID.
    /// </summary>
    /// <param name="personID">The unique identifier of the person to be retrieved.</param>
    /// <returns>Returns the person entity if found, otherwise returns null.</returns>
    Task<Person?> GetPersonByPersonID(Guid personID);


    /// <summary>
    /// Retrieves a filtered list of persons from the repository based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to filter the persons.</param>
    /// <returns>Returns a list of persons that match the given predicate condition.</returns>
    Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);


    /// <summary>
    /// Deletes a person from the repository based on the specified person ID.
    /// </summary>
    /// <param name="personID">The unique identifier of the person to be deleted.</param>
    /// <returns>Returns a boolean value indicating whether the deletion was successful.</returns>
    Task<bool> DeletePersonByPersonID(Guid personID);

    /// <summary>
    /// Updates an existing person in the repository.
    /// </summary>
    /// <param name="person">The person entity with updated data.</param>
    /// <returns>Returns the updated person entity after the changes are saved.</returns>
    Task<Person> UpdatePerson(Person person);
}