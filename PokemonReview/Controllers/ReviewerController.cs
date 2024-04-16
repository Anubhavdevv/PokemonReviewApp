using Microsoft.AspNetCore.Mvc;
using PokemonReview.Dto;
using PokemonReview.Interface;
using PokemonReview.Models;

namespace PokemonReview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewerController : ControllerBase
{
    private readonly IReviewerRepository _reviewerRepository;
    
    public ReviewerController(IReviewerRepository reviewerRepository)
    {
        _reviewerRepository = reviewerRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
    public IActionResult GetReviewers()
    {
        var reviews = _reviewerRepository.GetReviewers();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(reviews);
    }

    [HttpGet("{reviewerId}")]
    [ProducesResponseType(200, Type = typeof(Reviewer))]
    [ProducesResponseType(400)]

    public IActionResult GetReviewer(int reviewerId)
    {
        if(!_reviewerRepository.ReviewerExists(reviewerId))
        {
            return NotFound();
        }
        
        var reviewer = _reviewerRepository.GetReviewer(reviewerId);
        if(!ModelState.IsValid)
            return BadRequest();
        return Ok(reviewer);
    }

    [HttpGet("{reviewerId}/reviews")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
    [ProducesResponseType(400)]
    public IActionResult GetReviewsByAReviewer(int reviewerId)
    {
        if(!_reviewerRepository.ReviewerExists(reviewerId))
        {
            return NotFound();
        }

        var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);
        
        if(!ModelState.IsValid)
            return BadRequest();
        return Ok(reviews);
    }

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]

    public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
    {
        if (reviewerCreate == null)
        {
            return BadRequest(ModelState);
        }
        var country = _reviewerRepository.GetReviewers()
            .Where(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.TrimEnd().ToUpper())
            .FirstOrDefault();

        if (country != null)
        {
            ModelState.AddModelError("", "Country already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var reviewMap = reviewerCreate;

        var reviewInstance = new Reviewer
        {
            FirstName = reviewMap.FirstName,
            LastName = reviewMap.LastName,
        };

        if (!_reviewerRepository.CreateReviewer(reviewInstance))
        {
            ModelState.AddModelError("", $"Something went wrong saving the reviewer {reviewInstance.FirstName} {reviewInstance.LastName}");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created reviewer");
    }
    
    [HttpPut("{reviewerId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    
    public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updatedReviewer)
    {
        if (updatedReviewer == null)
        {
            return BadRequest(ModelState);
        }

        if (reviewerId != updatedReviewer.Id)
        {
            return BadRequest(ModelState);
        }

        if (!_reviewerRepository.ReviewerExists(reviewerId))
        {
            return NotFound();
        }

        var reviewerMap = updatedReviewer;

        var reviewerInstance = new Reviewer
        {
            FirstName = reviewerMap.FirstName,
            LastName = reviewerMap.LastName,
        };

        if (!_reviewerRepository.UpdateReviewer(reviewerInstance))
        {
            ModelState.AddModelError("", $"Something went wrong updating the reviewer");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    [HttpDelete("{reviewerId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult DeleteReviewer(int reviewerId)
    {
        if (!_reviewerRepository.ReviewerExists(reviewerId))
        {
            return NotFound();
        }

        var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
        {
            ModelState.AddModelError("", "Something went wrong deleting the reviewer");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
}