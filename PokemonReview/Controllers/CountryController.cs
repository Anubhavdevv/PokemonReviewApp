using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PokemonReview.Dto;
using PokemonReview.Interface;
using PokemonReview.Models;

namespace PokemonReview.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly IcountryRepository _countryRepository;
    public CountryController(IcountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
    public IActionResult GetCountries()
    {
        var countries = _countryRepository.GetCountries();
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        return Ok(countries);
    }
    
    [HttpGet("{countryId}")]
    [ProducesResponseType(200, Type = typeof(Country))]
    [ProducesResponseType(400)]
    public IActionResult GetCountry(int countryId)
    {
        if (!_countryRepository.CountryExists(countryId))
        {
            return NotFound();
        }

        var country = _countryRepository.GetCountry(countryId);
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        return Ok(country);
    }
    [HttpGet("/owners/{ownerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(200, Type = typeof(Country))]
    public IActionResult GetCountryOfAnOwner(int ownerId)
    {
        var country = _countryRepository.GetCountryByOwner(ownerId);

        if (!ModelState.IsValid)
            return BadRequest();

        return Ok(country);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]

    public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
    {
        if(countryCreate == null)
        {
            return BadRequest(ModelState);
        }
        var country = _countryRepository.GetCountries()
            .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();

        if (country != null)
        {
            ModelState.AddModelError("", "Country already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var countryMap = countryCreate;
        var countryEntity = new Country
        {
            Name = countryMap.Name
        };
        if (!_countryRepository.CreateCountry(countryEntity))
        {
            ModelState.AddModelError("", "Something went wrong saving the country");
            return StatusCode(500, ModelState);
        }
        
        return Ok("Successful Created");
    }

    [HttpPut("{countryId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]

    public IActionResult UpdateCountry(int countryId, [FromBody] CountryDto updatedCountry)
    {
        if (updatedCountry == null)
        {
            return BadRequest(ModelState);
        }

        if (countryId != updatedCountry.Id)
        {
            return BadRequest(ModelState);
        }

        if (!_countryRepository.CountryExists(countryId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var countryMap = updatedCountry;
        
        var countryInstance = new Country
        {
            Id = countryMap.Id,
            Name = countryMap.Name
        };
        
        if(!_countryRepository.updateCountry(countryInstance))
        {
            ModelState.AddModelError("", "Something went wrong updating country");
            return StatusCode(500, ModelState);
        }
        
        return NoContent();
    }

    [HttpDelete("{countryId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeleteCountry(int countryId)
    {
        
        if(!_countryRepository.CountryExists(countryId)) 
        {
            return NotFound();
        }

        var countryToDelete = _countryRepository.GetCountry(countryId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (_countryRepository.DeleteCountry(countryToDelete))
        {
            ModelState.AddModelError("", "Something went wrong deleting country");
            return StatusCode(500, ModelState);
        }

        return NoContent();

    }
}