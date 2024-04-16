using Microsoft.AspNetCore.Mvc;
using PokemonReview.Dto;
using PokemonReview.Interface;
using PokemonReview.Models;

namespace PokemonReview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OwnerController : ControllerBase
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IcountryRepository _countryRepository;
    public OwnerController(IOwnerRepository ownerRepository, IcountryRepository countryRepository)
    {
        _ownerRepository = ownerRepository;
        _countryRepository = countryRepository;
    }
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    public IActionResult GetOwners()
    {
        var owners = _ownerRepository.GetOwners();
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        return Ok(owners);
    }
    
    [HttpGet("{ownerId}")]
    [ProducesResponseType(200, Type = typeof(Owner))]
    [ProducesResponseType(400)]
    public IActionResult GetOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
        {
            return NotFound();
        }

        var owner = _ownerRepository.GetOwner(ownerId);
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        return Ok(owner);
    }

    [HttpGet("{ownerId}/pokemon")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
    [ProducesResponseType(400)]

    public IActionResult GetPokemonByOwner(int ownerId)
    {
        if(!_ownerRepository.OwnerExists(ownerId))
        {
            return NotFound();
        }
        
        var owner = _ownerRepository.GetPokemonByOwner(ownerId);
        
        if(!ModelState.IsValid)
            return BadRequest();
        return Ok(owner);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]

    public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
    {
        if (ownerCreate == null)
        {
            return BadRequest(ModelState);
        }

        var owner = _ownerRepository.GetOwners()
            .Where(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper()).FirstOrDefault();

        if (owner != null)
        {
            ModelState.AddModelError("", "Owner already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerMap = ownerCreate;

        var ownerEntity = new Owner
        {
            FirstName = ownerMap.FirstName,
            LastName = ownerMap.LastName,
            Gym = ownerMap.Gym,
        };

        ownerEntity.Country = _countryRepository.GetCountry(countryId);

        if (!_ownerRepository.CreateOwner(ownerEntity))
        {
            ModelState.AddModelError("", "Something went wrong saving the owner");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully Created Owner");
    }
    
    [HttpPut("{ownerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdateOwner(int ownerId, [FromBody] OwnerDto updatedOwner)
    {
        if (updatedOwner == null)
            return BadRequest(ModelState);

        if (ownerId != updatedOwner.Id)
            return BadRequest(ModelState);

        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest();

        var ownerMap = updatedOwner;

        var ownerEntity = new Owner
        {
            Id = ownerMap.Id,
            FirstName = ownerMap.FirstName,
            LastName = ownerMap.LastName,
            Gym = ownerMap.Gym,
        };

        if (!_ownerRepository.UpdateOwner(ownerEntity))
        {
            ModelState.AddModelError("", "Something went wrong updating the owner");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    [HttpDelete("{ownerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeleteOwner(int ownerId)
    {
        if (!_ownerRepository.OwnerExists(ownerId))
            return NotFound();

        var ownerToDelete = _ownerRepository.GetOwner(ownerId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        if (!_ownerRepository.DeleteOwner(ownerToDelete))
        {
            ModelState.AddModelError("", "Something went wrong deleting the owner");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}