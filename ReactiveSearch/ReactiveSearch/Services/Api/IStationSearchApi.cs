using ReactiveSearch.Models;
using Refit;
using System.Threading.Tasks;

namespace ReactiveSearch.Services.Api
{
    internal interface IStationSearchApi
    {
        [Get("/v1/locations?query={query}")]
        Task<StationSearchResult> Search(string query);
    }
}