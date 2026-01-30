using Moq;
using RepositoryContracts;
using ServiceContracts;
using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;

namespace CRUDTests;

public class PersonsControllerTest
{
    
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    
    private readonly Mock<IPersonsService> _personsServiceMock;
    private readonly Mock<ICountriesService> _countriesServiceMock;
    
    private readonly Fixture _fixture;

    public PersonsControllerTest()
    {
        _fixture = new Fixture();
        
        _countriesServiceMock = new Mock<ICountriesService>();
        _personsServiceMock = new Mock<IPersonsService>();
        
        _countriesService = _countriesServiceMock.Object;
        _personsService = _personsServiceMock.Object;
        
    }


    #region Index

    [Fact]
    public async Task Index_ShouldReturnIndexViewWithPersonsList()
    {
        
        //Arrange
        List<PersonResponse> personResponsesList = _fixture.Create <List<PersonResponse>>();
        
        var loggerMock = new Mock<ILogger<PersonsController>>();
        
        PersonsController personsController = new PersonsController(_personsService, _countriesService,loggerMock.Object);
        
        _personsServiceMock.
            Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(),It.IsAny<string>()))
            .ReturnsAsync(personResponsesList);
        
        _personsServiceMock.Setup(temp => 
                temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(),
                    It.IsAny<string>(),It.IsAny<SortOrderOptions>()))
            .ReturnsAsync(personResponsesList);
        
        
        //Act

        IActionResult result = await personsController.Index(_fixture.Create<string>(), 
            _fixture.Create<string>(),_fixture.Create<string>(),
            _fixture.Create<SortOrderOptions>());;
        
        //Assert
        
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        
        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        
        viewResult.ViewData.Model.Should().Be(personResponsesList);

    }
    
    

    #endregion


    #region Create

    [Fact]
    public async Task Create_InfoModelErrors_ShouldReturnCreateView()
    {
        
        //Arrange
        PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
        PersonResponse personResponse = _fixture.Create<PersonResponse>();
        
        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();
        
        _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
        
        _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponse);
        
        List<PersonResponse> personResponsesList = _fixture.Create <List<PersonResponse>>();

        var loggerMock = new Mock<ILogger<PersonsController>>();
        
        PersonsController personsController = new PersonsController(_personsService, _countriesService,loggerMock.Object);

        //Act
        
        personsController.ModelState.AddModelError("PersonName","PersonName is required");

        IActionResult result = await personsController.Create(personAddRequest);
        
        //Assert
        
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        
        viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
        
        viewResult.ViewData.Model.Should().Be(personAddRequest);

    }
    
    [Fact]
    public async Task Create_IfNoModelErrors_ShouldRedirectToIndex()
    {
        
        //Arrange
        PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
        PersonResponse personResponse = _fixture.Create<PersonResponse>();
        
        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();
        
        _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
        
        _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personResponse);
        
        List<PersonResponse> personResponsesList = _fixture.Create <List<PersonResponse>>();

        
        var loggerMock = new Mock<ILogger<PersonsController>>();
        
        PersonsController personsController = new PersonsController(_personsService, _countriesService,loggerMock.Object);

        
        //Act

        IActionResult result = await personsController.Create(personAddRequest);
        
        //Assert
        
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        
        redirectResult.ActionName.Should().Be("Index");
        
        // redirectResult.ControllerName.Should().Be("Persons");

    }

    #endregion
    
    
}