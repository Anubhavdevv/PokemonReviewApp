using Microsoft.AspNetCore.Mvc;
using PokemonReview.Dto;
using PokemonReview.Interface;
using PokemonReview.Models;

namespace PokemonReview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IPokemonRepository _pokemonRepository;
    
    public ReviewController(IReviewRepository reviewRepository, 
        IPokemonRepository pokemonRepository,
        IReviewerRepository reviewerRepository)
    {
        _reviewRepository = reviewRepository;
        _reviewerRepository = reviewerRepository;
        _pokemonRepository = pokemonRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    public IActionResult GetReviews()
    {
        var reviews = _reviewRepository.GetReviews();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviews);
    }
    
    [HttpGet("{reviewId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetPokemon(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();

        var review = _reviewRepository.GetReview(reviewId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(review);
    }
    [HttpGet("/pokemon/{pokeId}")]
    [ProducesResponseType(200, Type = typeof(Review))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsForAPokemon(int pokeId)
    {
        var reviews = _reviewRepository.GetReviewsOfAPokemon(pokeId);

        if (!ModelState.IsValid)
            return BadRequest();

        return Ok(reviews);
    }
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]

    public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery]int pokeId, [FromBody] ReviewDto reviewCreate)
    {
        if (reviewCreate == null)
            return BadRequest(ModelState);

        var reviews = _reviewRepository.GetReviews()
            .Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
            .FirstOrDefault();

        if (reviews != null)
        {
            ModelState.AddModelError("", "Review already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reviewMap = reviewCreate;
        var reviewInstance = new Review
        {
            Title = reviewMap.Title,
            Text = reviewMap.Text,
            Rating = reviewMap.Rating,
            Pokemon = _pokemonRepository.GetPokemon(pokeId),
            Reviewer = _reviewerRepository.GetReviewer(reviewerId)
        };


        if (!_reviewRepository.CreateReview(reviewInstance))
        {
            ModelState.AddModelError("", "Something went wrong while savin");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }
    
    [HttpPut("{reviewId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto updatedReview)
    {
        if (updatedReview == null)
            return BadRequest(ModelState);

        if (reviewId != updatedReview.Id)
        {
            return BadRequest(ModelState);
        }

        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();

        var reviewMap = updatedReview;
        var reviewMapInstance = new Review
        {
            Id = reviewMap.Id,
            Text = reviewMap.Text,
            Rating = reviewMap.Rating,
            Title = reviewMap.Title
        };

        if (!_reviewRepository.UpdateReview(reviewMapInstance))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    [HttpDelete("{reviewId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReview(int reviewId)
    {
        if (!_reviewRepository.ReviewExists(reviewId))
            return NotFound();

        var reviewToDelete = _reviewRepository.GetReview(reviewId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_reviewRepository.DeleteReview(reviewToDelete))
        {
            ModelState.AddModelError("", "Something went wrong while deleting");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
}