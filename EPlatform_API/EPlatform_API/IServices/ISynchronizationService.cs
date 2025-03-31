using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models.ShopOwners;

namespace EPlatform_API.IServices
{
    public interface ISynchronizationService
    {
        Task<bool> InsertOrUpdateSearchProductAnalysic(AutocompleteProduct autocomplete);
        Task UpdateAutocompleteData();
        
    }
}