using System.Collections.Generic;

namespace ReactiveSearch.Models
{
    internal class StationSearchResult
    {
        public StationSearchResult(IEnumerable<Station> stations)
        {
            Stations = stations ?? new Station[0];
        }

        public IEnumerable<Station> Stations { get; }
    }
}
