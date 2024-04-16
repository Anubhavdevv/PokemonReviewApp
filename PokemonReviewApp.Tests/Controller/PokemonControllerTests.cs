using FakeItEasy;
using FluentAssertions;
using PokemonReview.Controllers;
using PokemonReview.Dto;
using PokemonReview.Interface;
using Microsoft.AspNetCore.Mvc;


namespace PokemonReviewApp.Tests.Controller;

public class PokemonControllerTests
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewRepository _reviewRepository;
    public PokemonControllerTests()
    {
        _pokemonRepository = A.Fake<IPokemonRepository>();
        _reviewRepository = A.Fake<IReviewRepository>();
    }

    [Fact]
    
    public void PokemonController_GetPokemons_ReturnOK()
    {
        //Arrange
        var pokemons = A.Fake<ICollection<PokemonDto>>();
        var pokemonList = A.Fake<List<PokemonDto>>();
        // A.CallTo(() => pokemons.ToList()).Returns(pokemonList);
        
        var controller = new PokemonController(_pokemonRepository, _reviewRepository);
        
        //Act
        var result = controller.GetPokemons();

        //Assert

        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(OkObjectResult));
    }
    // public void PokemonController_GetPokemons_ReturnOK()
    // {
    //     //Arrange
    //     var pokemons = A.Fake<ICollection<PokemonDto>>();
    //     var pokemonList = A.Fake<List<PokemonDto>>();
    //     
    //     
    //     // A.CallTo(() => <List<PokemonDto>>(pokemons)).Returns(pokemonList);
    //     var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);
    //     
    //     //Act
    //     var result = controller.GetPokemons();
    //
    //     //Assert
    //
    //     result.Should().NotBeNull();
    // }
}