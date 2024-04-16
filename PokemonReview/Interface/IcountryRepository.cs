using PokemonReview.Models;

namespace PokemonReview.Interface;

public interface IcountryRepository
{
    ICollection<Country> GetCountries();
    
    Country GetCountry(int id);
    
    Country GetCountryByOwner(int ownerId);
    
    ICollection<Owner> GetOwnersFromACountry(int countryId);
    bool CountryExists(int id);
    
    bool CreateCountry(Country country);

    bool updateCountry(Country country);

    bool DeleteCountry(Country country);
    bool Save();
}