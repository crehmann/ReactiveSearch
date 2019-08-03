using ReactiveSearch.Models;
using ReactiveSearch.Services.Api;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ReactiveSearch.ViewModels
{
    internal class SearchViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IStationSearchApi _stationSearchApi;
        private ReactiveCommand<string, List<Station>> _searchCommand;
        private ObservableCollection<Station> _searchResult;
        private string _searchQuery;

        public SearchViewModel(IStationSearchApi stationSearchApi)
        {
            _stationSearchApi = stationSearchApi ?? throw new ArgumentNullException(nameof(stationSearchApi));

            SearchResults = new ObservableCollection<Station>();
            Search = ReactiveCommand.CreateFromTask<string, List<Station>>(SearchAsync, CanSearch());
            Search.ObserveOn(RxApp.MainThreadScheduler).Subscribe(LoadSearchResult);
            Search.ThrownExceptions.Subscribe(async ex => await SearchError.Handle("An error occured."));

            this.WhenAnyValue(x => x.SearchQuery)
                .Throttle(TimeSpan.FromMilliseconds(300), TaskPoolScheduler.Default)
                .Do(x => System.Diagnostics.Debug.WriteLine($"Throttle fired for {x}"))
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(Search);
        }

        #region Properties

        public string SearchQuery
        {
            get { return _searchQuery; }
            set { this.RaiseAndSetIfChanged(ref _searchQuery, value); }
        }

        public ObservableCollection<Station> SearchResults
        {
            get { return _searchResult; }
            set { this.RaiseAndSetIfChanged(ref _searchResult, value); }
        }

        public ReactiveCommand<string, List<Station>> Search
        {
            get { return _searchCommand; }
            set { this.RaiseAndSetIfChanged(ref _searchCommand, value); }
        }

        public Interaction<string, Unit> SearchError { get; } = new Interaction<string, Unit>();

        public string UrlPathSegment => "Search";

        public IScreen HostScreen { get; set; }

        #endregion

        private async Task<List<Station>> SearchAsync(string searchQuery)
        {
            var searchResult = await _stationSearchApi.SearchAsync(searchQuery).ConfigureAwait(false);
            return searchResult.Stations.ToList();
        }

        private IObservable<bool> CanSearch()
            => Observable
                    .CombineLatest(
                        this.WhenAnyValue(vm => vm.SearchQuery)
                            .Select(searchQuery => !string.IsNullOrEmpty(searchQuery))
                            .DistinctUntilChanged(),
                        this.WhenAnyObservable(x => x.Search.IsExecuting)
                            .DistinctUntilChanged(),
                        (hasSearchQuery, isExecuting) => hasSearchQuery && !isExecuting)
                    .Do(cps => System.Diagnostics.Debug.WriteLine($"Can Perform Search: {cps}"))
                    .DistinctUntilChanged();

        private void LoadSearchResult(IEnumerable<Station> searchResult)
        {
            SearchResults.Clear();
            foreach (var item in searchResult)
            {
                SearchResults.Add(item);
            }
        }
    }
}
