using Splat;
using ReactiveUI;
using Xamarin.Forms;
using ReactiveSearch.Pages;
using ReactiveSearch.ViewModels;
using Refit;
using ReactiveSearch.Services.Api;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace ReactiveSearch
{
    public partial class App : Application, IScreen
    {
        private readonly IConfigProvider configProvider;

        public App(IConfigProvider configProvider)
        {
            InitializeComponent();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new SearchPage(), typeof(IViewFor<SearchViewModel>));
            Locator.CurrentMutable.RegisterLazySingleton(
                () => RestService.For<IStationSearchApi>(Constants.SearchEndpoint),
                typeof(IStationSearchApi));

            Router.Navigate.Execute(new SearchViewModel(Locator.Current.GetService<IStationSearchApi>()));
            MainPage = new ReactiveUI.XamForms.RoutedViewHost();
            this.configProvider = configProvider;
        }

        public RoutingState Router { get; } = new RoutingState();

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start(configProvider.AppCenterApiToken, typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
