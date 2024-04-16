namespace PokemonReview.Models;


//Pokemon Owner Join
public class PokemonOwner
{
    public int PokemonId { get; set; }
    public int OwnerId { get; set; }
    public Pokemon Pokemon { get; set; } //one to one relationship
    public Owner Owner { get; set; }
}