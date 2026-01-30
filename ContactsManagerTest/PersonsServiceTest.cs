using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using Serilog;

namespace CRUDTests
{
  public class PersonsServiceTest
  {
    //private fields
    private readonly IPersonsService _personService;
    
    private readonly Mock<IPersonsRepository> _personsRepositoryMock;
    private readonly IPersonsRepository _personsRepository;
    
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture ;

    //constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
      _fixture = new Fixture();
      _personsRepositoryMock = new Mock<IPersonsRepository>();
      _personsRepository = _personsRepositoryMock.Object;
      
      var loggerMock = new Mock<ILogger<PersonsService>>();
      var diagnosticContextMock = new Mock<IDiagnosticContext>();
      
      _personService = new PersonsService(_personsRepository,loggerMock.Object,diagnosticContextMock.Object);
      
      _testOutputHelper = testOutputHelper;
      
      // var countries = new List<Country>();
      // var persons = new List<Person>();
 
      // DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
      //   new DbContextOptionsBuilder<ApplicationDbContext>().Options
      // );
      //
      // ApplicationDbContext dbContext = dbContextMock.Object;
      // dbContextMock.CreateDbSetMock(temp => temp.Countries, countries);
      // dbContextMock.CreateDbSetMock(temp => temp.Persons, persons);
      

    }

    #region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
      //Arrange
      PersonAddRequest? personAddRequest = null;
      
      Func<Task> act = async () => await _personService.AddPerson(personAddRequest);

      await act.Should().ThrowAsync<ArgumentNullException>();

      //Act
      // await Assert.ThrowsAsync<ArgumentNullException>(async () =>
      // {
      //   await _personService.AddPerson(personAddRequest);
      // });
    }


    //When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
    {
      //Arrange
      PersonAddRequest? personAddRequest =
        _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, (string?)null).Create();

      Person person = personAddRequest.ToPerson();
      _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
      
      Func<Task> act = async () => await _personService.AddPerson(personAddRequest);
      await act.Should().ThrowAsync<ArgumentException>();
      // //Act
      // await Assert.ThrowsAsync<ArgumentException>(async () =>
      // {
      //   await _personService.AddPerson(personAddRequest);
      // });
    }

    //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
      //Arrange
      // PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = "Person name...", Email = "person@example.com", Address = "sample address", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true };

      PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

      Person person = personAddRequest.ToPerson();
      PersonResponse person_response_expected = person.ToPersonResponse();
      
      //if we supply any argument value to the AddPerson method, it should return the same return value
      _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync((Person person) => person);
      
      
      //Act
      PersonResponse person_response_from_add =await _personService.AddPerson(personAddRequest);
      
      person_response_expected.PersonID = person_response_from_add.PersonID;

      // List<PersonResponse> persons_list =await _personService.GetAllPersons();

      //Assert
      // Assert.True(person_response_from_add.PersonID != Guid.Empty);

      person_response_from_add.PersonID.Should().NotBeEmpty();
      person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
      
      person_response_from_add.Should().Be(person_response_expected);
      
      

      // persons_list.Should().Contain(person_response_from_add);
      // Assert.Contains(person_response_from_add, persons_list);
    }

    #endregion


    #region GetPersonByPersonID

    //If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID()
    {
      //Arrange
      Guid? personID = null;

      //Act
      PersonResponse? person_response_from_get =await _personService.GetPersonByPersonID(personID);

      //Assert
      // Assert.Null(person_response_from_get);
      
      person_response_from_get.Should().BeNull();
      
    }
    


    //If we supply a valid person id, it should return the valid person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
    {
      //Arange
      // CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
      // CountryResponse country_response =await _countriesService.AddCountry(country_request);
      
      

      Person person = _fixture.Build<Person>()
        // .With(temp => temp.CountryID, country_response.CountryID)
        .With(temp => temp.Email,"email@sample.com")
        .With(temp => temp.Country, null as Country)
        .Create();
      
      PersonResponse person_response_expected = person.ToPersonResponse();
      
      _personsRepositoryMock.Setup(temp =>temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync( person);
        
      PersonResponse person_response_from_get =await _personService.GetPersonByPersonID(person.PersonID);

    
      //Assert
      // Assert.Equal(person_response_from_add, person_response_from_get);
      person_response_from_get.Should().Be(person_response_expected);
      // person_response_from_get.Should().Be(person_response_from_add);
      
    }
    
    

    #endregion


    #region GetAllPersons

    //The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_ToBeEmptyList()
    {
      
      _personsRepositoryMock.Setup(temp =>temp.GetAllPersons()).ReturnsAsync(new List<Person>());
      
      //Act
      List<PersonResponse> persons_from_get =await _personService.GetAllPersons();

      //Assert
      // Assert.Empty(persons_from_get);
      
      persons_from_get.Should().BeEmpty();
      
    }


    //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
    {
      //Arrange
      List<Person> persons_list = new List<Person>()
      {
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
      };


      List<PersonResponse> person_response_list_from_add =
        persons_list.Select(temp => temp.ToPersonResponse()).ToList();

      //print person_response_list_from_add
      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }
      
      _personsRepositoryMock.Setup(temp =>temp.GetAllPersons()).ReturnsAsync(persons_list);

      //Act
      List<PersonResponse> persons_list_from_get =await _personService.GetAllPersons();

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_get)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      {
        // Assert.Contains(person_response_from_add, persons_list_from_get);
        persons_list_from_get.Should().Contain(person_response_from_add);
        
      }
      
      persons_list_from_get.Should().BeEquivalentTo(person_response_list_from_add);
      
    }
    #endregion


    #region GetFilteredPersons

    //If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
      //Arrange
      List<Person> persons_list = new List<Person>()
      {
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
      };


      List<PersonResponse> person_response_list_from_add =
        persons_list.Select(temp => temp.ToPersonResponse()).ToList();
      

      //print person_response_list_from_add
      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }
      
      _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons_list);

      //Act
      List<PersonResponse> persons_list_from_search =await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_search)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      {
        // Assert.Contains(person_response_from_add, persons_list_from_search);
        persons_list_from_search.Should().Contain(person_response_from_add);
      }
      persons_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);
      
    }


    //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
    {
      //Arrange
      List<Person> persons_list = new List<Person>()
      {
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.PersonName ," maria")
          .With(temp => temp.Country, null as Country)
          .Create(),
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
      };


      List<PersonResponse> person_response_list_from_add =
        persons_list.Select(temp => temp.ToPersonResponse()).ToList();
      

      //print person_response_list_from_add
      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }
      
      _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons_list);

      //Act
      List<PersonResponse> persons_list_from_search =await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_search)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      foreach (PersonResponse person_response_from_add in person_response_list_from_add)
      {
        // Assert.Contains(person_response_from_add, persons_list_from_search);
        persons_list_from_search.Should().Contain(person_response_from_add);
      }
      persons_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);
      
    }

    #endregion


    #region GetSortedPersons

    //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons_ToBeSuccessful()
    {
      //Arrange
      List<Person> persons_list = new List<Person>()
      {
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.PersonName ," maria")
          .With(temp => temp.Country, null as Country)
          .Create(),
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .Create(),
      };


      List<PersonResponse> person_response_list_expected =
        persons_list.Select(temp => temp.ToPersonResponse()).ToList();
      
      _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons_list);

      //print person_response_list_from_add
      List<PersonResponse> allPersons =await _personService.GetAllPersons();
      //Act
      List<PersonResponse> persons_list_from_sort =await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

      //print persons_list_from_get
      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_sort)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }
     
      //Assert
      // for (int i = 0; i < person_response_list_from_add.Count; i++)
      // {
      //   Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
      // }
      
      persons_list_from_sort.Should().BeEquivalentTo(person_response_list_expected);
      
      persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
      
      
    }
    #endregion


    #region UpdatePerson

    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = null;

      //Assert
      // await Assert.ThrowsAsync<ArgumentNullException>(async () => {
      //   //Act
      //   await _personService.UpdatePerson(person_update_request);
      // });
      
      Func<Task> act = async () => await _personService.UpdatePerson(person_update_request);
      await act.Should().ThrowAsync<ArgumentNullException>();
      
    }


    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = new PersonUpdateRequest() {  PersonID = Guid.NewGuid() };

      //Assert
      // await Assert.ThrowsAsync<ArgumentException>(async () => {
      //   //Act
      //  await _personService.UpdatePerson(person_update_request);
      // });
      
      Func<Task> act = async () => await _personService.UpdatePerson(person_update_request);
      await act.Should().ThrowAsync<ArgumentException>();
    }


    //When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull_ToBeArugmentException()
    {
      //Arrange


      Person person =
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.PersonName, null as string)
          .With(temp => temp.Country, null as Country)
          .With(temp => temp.Gender, "Male")
          .Create();
  
      PersonResponse person_response_from_add =person.ToPersonResponse();
      //Assert
      // await Assert.ThrowsAsync<ArgumentException>(async () => {
      //   //Act
      //  await _personService.UpdatePerson(person_update_request);
      // });
      
      PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
      
      Func<Task> act = async () => await _personService.UpdatePerson(person_update_request);
      await act.Should().ThrowAsync<ArgumentException>();
      
    }


    //First, add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_PersonFullDetailsUpdation()
    {
      //Arrange
      Person person =
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .With(temp => temp.Gender, "Male")
          .Create();
      
      PersonResponse person_response_expected =person.ToPersonResponse();
      
      PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

      _personsRepositoryMock.Setup(temp =>temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
      _personsRepositoryMock.Setup(temp =>temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);
      //Act
      PersonResponse person_response_from_update =await _personService.UpdatePerson(person_update_request);

      // PersonResponse? person_response_from_get =await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

      //Assert
      // Assert.Equal(person_response_from_get, person_response_from_update);
      
      person_response_from_update.Should().Be(person_response_expected);

    }

    #endregion


    #region DeletePerson

    //If you supply an valid PersonID, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
    {
      //Arrange
      Person person =
        _fixture.Build<Person>()
          .With(temp => temp.Email, "someone1@example.com")
          .With(temp => temp.Country, null as Country)
          .With(temp => temp.Gender, "Male")
          .Create();


      _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
      _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);  
      
      // PersonResponse person_response_from_add =person.ToPersonResponse();
      //Act
      bool isDeleted =await _personService.DeletePerson(person.PersonID);
      
      

      //Assert
      // Assert.True(isDeleted);
      isDeleted.Should().BeTrue();
    }


    //If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
      //Act
      bool isDeleted =await _personService.DeletePerson(Guid.NewGuid());

      //Assert
      // Assert.False(isDeleted);
      isDeleted.Should().BeFalse();
    }

    #endregion
  }
}
