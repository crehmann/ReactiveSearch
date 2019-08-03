using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using ReactiveSearch.ViewModels;
using ReactiveUI;
using System.Reactive;
using ReactiveUI.XamForms;

namespace ReactiveSearch.Pages  
{
    internal class SearchPage : ReactiveContentPage<SearchViewModel>
    {
        private readonly CompositeDisposable _bindingsDisposable = new CompositeDisposable();

        public SearchPage()
        {
            Title = "Station Search";
            Content = new StackLayout
            {
                Padding = new Thickness(8d),
                Children = {
                    (SearchEntry = new Entry{ Placeholder = "Enter location" }),
                    (SearchButton = new Button{ Text = "Search", IsEnabled = false }),
                    (ActivityIndicator = new ActivityIndicator()),
                    (SearchResults = new ListView {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        SelectionMode = ListViewSelectionMode.None
                    })
                }
            };
        }

        #region UI Controls

        private Entry SearchEntry { get; }
        private Button SearchButton { get; }
        private ListView SearchResults { get; }
        private ActivityIndicator ActivityIndicator { get; }

        #endregion

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeBindings();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _bindingsDisposable.Clear();
        }

        private void InitializeBindings()
        {
            // Search Query
            this.Bind(ViewModel, x => x.SearchQuery, c => c.SearchEntry.Text)
                .DisposeWith(_bindingsDisposable);

            // Results
            this.OneWayBind(ViewModel, x => x.SearchResults, c => c.SearchResults.ItemsSource)
                .DisposeWith(_bindingsDisposable);

            // Search Command
            this.BindCommand(ViewModel, x => x.Search, c => c.SearchButton, vm => vm.SearchQuery)
                .DisposeWith(_bindingsDisposable);

            // Activity Indicator
            this.WhenAnyObservable(x => x.ViewModel.Search.IsExecuting)
                .BindTo(ActivityIndicator, c => c.IsRunning)
                .DisposeWith(_bindingsDisposable);

            // Errors
            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Subscribe(vm =>
                {
                    vm.SearchError
                        .RegisterHandler(interaction =>
                        {
                            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", interaction.Input, "OK"));
                            interaction.SetOutput(Unit.Default);
                        });
                })
                .DisposeWith(_bindingsDisposable);
        }
    }
}
