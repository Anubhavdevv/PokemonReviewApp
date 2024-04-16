using PokemonReview.Models;

namespace PokemonReview.Interface;

public interface IOwnerRepository
{
    ICollection<Owner> GetOwners();
    
    Owner GetOwner(int ownerId);

    ICollection<Owner> GetOwnerOfAPokemon(int pokeId);

    ICollection<Pokemon> GetPokemonByOwner(int ownerId);
    
    bool OwnerExists(int id);
    
    bool CreateOwner (Owner owner);
    
    bool UpdateOwner(Owner owner);
    
    bool DeleteOwner(Owner owner);
    bool Save();
}