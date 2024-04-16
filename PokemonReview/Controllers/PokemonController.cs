using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PokemonReview.Dto;
using PokemonReview.Interface;
using PokemonReview.Models;

namespace PokemonReview.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PokemonController:ControllerBase
{
    private readonly IPokemonRepository _pokemonRepository;
    private readonly IReviewRepository _reviewRepository;
    public PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository)
    {
        _pokemonRepository = pokemonRepository;
        _reviewRepository = reviewRepository;
    }
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    public IActionResult GetPokemons()
    {
        var pokemons = _pokemonRepository.GetPokemons();
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        return Ok(pokemons);
    }

    [HttpGet("{pokeId}")]
    [ProducesResponseType(200, Type = typeof(Pokemon))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int pokeId)
    {
        if (!_pokemonRepository.PokemonExists(pokeId))
        {
            return NotFound();
        }

        var pokemon = _pokemonRepository.GetPokemon(pokeId);
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        return Ok(pokemon);
    }

    [HttpGet("{pokeId}/rating")]
    [ProducesResponseType(200, Type = typeof(decimal))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemonRating(int pokeId)
    {
        if (!_pokemonRepository.PokemonExists(pokeId))
        {
            return NotFound();
        }

        var rating = _pokemonRepository.GetPokemonRating(pokeId);
        
        if (!ModelState.IsValid)
            return BadRequest(); 

        return Ok(rating);
    }
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
    {
        if(pokemonCreate == null)
        {
            return BadRequest(ModelState);
        }

        var pokemons = _pokemonRepository.GetPokemons()
            .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();

        if (pokemons != null)
        {
            ModelState.AddModelError("", "Pokemon already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pokemonMap = pokemonCreate;
        
        var pokemonEntity = new Pokemon
        {
            Name = pokemonMap.Name,
            BirthDate = pokemonMap.BirthDate
        };

        if (!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonEntity))
        {
            ModelState.AddModelError("", $"Something went wrong saving the pokemon {pokemonEntity.Name}");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created pokemon");
    }
    
    [HttpPut("{pokeId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult UpdatePokemon(int pokeId, [FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto updatedPokemon)
    {
        if (updatedPokemon == null)
        {
            return BadRequest(ModelState);
        }

        if (pokeId != updatedPokemon.Id)
        {
            return BadRequest(ModelState);
        }

        if (!_pokemonRepository.PokemonExists(pokeId))
        {
            return NotFound();
        }

        var pokemonMap = updatedPokemon;
        var pokemonEntity = new Pokemon
        {
            Id = pokemonMap.Id,
            Name = pokemonMap.Name,
            BirthDate = pokemonMap.BirthDate
        };

        if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokemonEntity))
        {
            ModelState.AddModelError("", $"Something went wrong updating the pokemon {pokemonEntity.Name}");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updated pokemon");
    }
    
    [HttpDelete("{pokeId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeletePokemon(int pokeId)
    {
        if (!_pokemonRepository.PokemonExists(pokeId))
        {
            return NotFound();
        }

        var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);

        var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        if(!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
        {
            ModelState.AddModelError("", "Something went wrong deleting the reviews");
            return StatusCode(500, ModelState);
        }

        if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
        {
            ModelState.AddModelError("", $"Something went wrong deleting the pokemon {pokemonToDelete.Name}");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully deleted pokemon");
    }
}