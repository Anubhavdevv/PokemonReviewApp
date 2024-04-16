using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PokemonReview.Dto;
using PokemonReview.Interface;
using PokemonReview.Models;

namespace PokemonReview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IcategoryRepository _categoryRepository;
    public CategoryController(IcategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
        
    }
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
    public IActionResult GetCategories()
    {
        var categories = _categoryRepository.GetCategories();
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        return Ok(categories);
    }
    
    [HttpGet("{categoryId}")]
    [ProducesResponseType(200, Type = typeof(Category))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int categoryId)
    {
        if (!_categoryRepository.CategoryExists(categoryId))
        {
            return NotFound();
        }

        var category = _categoryRepository.GetCategory(categoryId);
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        return Ok(category);
    }

    [HttpGet("pokemon/{categoryId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
    [ProducesResponseType(400)]

    public IActionResult GetPokemonByCategoryId(int categoryId)
    {
        var pokemons = _categoryRepository.GetPokemonByCategory(categoryId);
        
        if(!ModelState.IsValid)
            return BadRequest();
        return Ok(pokemons);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
    {
        if (categoryCreate == null)
            return BadRequest(ModelState);

        var category = _categoryRepository.GetCategories()
            .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();
        
        if (category != null)
        {
            ModelState.AddModelError("", "Category already exists");
            return StatusCode(422, ModelState);
        }
        
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var categoryMap = categoryCreate;
        var categoryEntity = new Category
        {
            Name = categoryMap.Name
        };
        
        if (!_categoryRepository.CreateCategory(categoryEntity))
        {
            ModelState.AddModelError("", "Something went wrong saving the category");
            return StatusCode(500, ModelState);
        }
        
        return Ok("Successful Created");
    }
    
    [HttpPut("{categoryId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult UpdateCategory(int categoryId, [FromBody]CategoryDto updatedCategory)
    {
        if (updatedCategory == null)
            return BadRequest(ModelState);

        if (categoryId != updatedCategory.Id)
            return BadRequest(ModelState);

        if (!_categoryRepository.CategoryExists(categoryId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest();

        var categoryMap = updatedCategory;
        
        var categoryInstance = new Category
        {
            Id = categoryMap.Id,
            Name = categoryMap.Name
        };

        if(!_categoryRepository.UpdateCategory(categoryInstance))
        {
            ModelState.AddModelError("", "Something went wrong updating category");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }

    [HttpDelete("{categoryId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]

    public IActionResult DeleteCategory(int categoryId)
    {
        if (!_categoryRepository.CategoryExists(categoryId))
        {
            return NotFound();
        }

        var categoryToDelete = _categoryRepository.GetCategory(categoryId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        if(!_categoryRepository.DeleteCategory(categoryToDelete))
        {
            ModelState.AddModelError("", "Something went wrong deleting category");
            return StatusCode(500, ModelState);
        }
        
        return NoContent();
    }

    
}