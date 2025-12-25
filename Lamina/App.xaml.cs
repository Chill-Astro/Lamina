using System;
using System.Threading.Tasks;
using Lamina.Core.Contracts.Services;
using Lamina.Core.Services;
using Lamina.ViewModels;
using Lamina.Views;

using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Lamina.Models;
using Lamina.Contracts.Services;
using Lamina.Services;
using Lamina.Activation;

namespace Lamina
{
    public partial class App : Application
    {
        public IHost Host
        {
            get;
        }
        public static T GetService<T>()
            where T : class
        {
            if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
            }

            return service;
        }
        public static WindowEx MainWindow { get; } = new MainWindow();

        public static UIElement? AppTitlebar
        {
            get; set;
        }
        // File: App.xaml.cs
        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.
            CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers

                // Services
                services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();
                services.AddSingleton<IMicaService, MicaService>();
                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();
                // Core Services
                services.AddSingleton<IFileService, FileService>();

                // Views and ViewModels
                services.AddTransient<CurrencyViewModel>();
                services.AddTransient<CurrencyPage>();
                services.AddTransient<QuadEqnViewModel>();
                services.AddTransient<QuadEqnPage>();
                services.AddTransient<BaseConverterViewModel>();
                services.AddTransient<BaseConverterPage>();
                services.AddTransient<UnitConverterViewModel>();
                services.AddTransient<UnitConverterPage>();
                services.AddTransient<DateDifferenceViewModel>();
                services.AddTransient<DateDifferencePage>();
                services.AddTransient<DicountViewModel>();
                services.AddTransient<DiscountPage>();
                services.AddTransient<SphereCSAViewModel>();
                services.AddTransient<SphereCSAPage>();
                services.AddTransient<ConeCSAViewModel>();
                services.AddTransient<ConeCSAPage>();
                services.AddTransient<CylinderCSAViewModel>();
                services.AddTransient<CylinderCSAPage>();
                services.AddTransient<CSurfaceAreaViewModel>();
                services.AddTransient<CSurfaceAreaPage>();
                services.AddTransient<RoomAreaViewModel>();
                services.AddTransient<RoomAreaPage>();
                services.AddTransient<RTCheckViewModel>();
                services.AddTransient<RTCheckPage>();
                services.AddTransient<TriAreaViewModel>();
                services.AddTransient<TriAreaPage>();
                services.AddTransient<AgeCalcViewModel>();
                services.AddTransient<AgeCalcPage>();
                services.AddTransient<PrimeCheckViewModel>();
                services.AddTransient<PrimeCheckPage>();
                services.AddTransient<CuboidDiagViewModel>();
                services.AddTransient<CuboidDiagPage>();
                services.AddTransient<CubeDiagViewModel>();
                services.AddTransient<CubeDiagPage>();
                services.AddTransient<RectDiagViewModel>();
                services.AddTransient<RectDiagPage>();
                services.AddTransient<SquareDiagViewModel>();
                services.AddTransient<SquareDiagPage>();
                services.AddTransient<SCirclePermViewModel>();
                services.AddTransient<SCirclePermPage>();
                services.AddTransient<CirclePermViewModel>();
                services.AddTransient<CirclePermPage>();
                services.AddTransient<RhombusPermViewModel>();
                services.AddTransient<RhombusPermPage>();
                services.AddTransient<RectPermViewModel>();
                services.AddTransient<RectPermPage>();
                services.AddTransient<SquarePermViewModel>();
                services.AddTransient<SquarePermPage>();
                services.AddTransient<ITPermViewModel>();
                services.AddTransient<ITPermPage>();
                services.AddTransient<ETPermViewModel>();
                services.AddTransient<ETPermPage>();
                services.AddTransient<PerimeterViewModel>();
                services.AddTransient<PerimeterPage>();
                services.AddTransient<DiagonalViewModel>();
                services.AddTransient<DiagonalPage>();
                services.AddTransient<SphereSAViewModel>();
                services.AddTransient<SphereSAPage>();
                services.AddTransient<ConeSAViewModel>();
                services.AddTransient<ConeSAPage>();
                services.AddTransient<CylinderSAViewModel>();
                services.AddTransient<CylinderSAPage>();
                services.AddTransient<CuboidSAViewModel>();
                services.AddTransient<CuboidSAPage>();
                services.AddTransient<CubeSAViewModel>();
                services.AddTransient<CubeSAPage>();
                services.AddTransient<SphereViewModel>();
                services.AddTransient<SpherePage>();
                services.AddTransient<ConeVolumeViewModel>();
                services.AddTransient<ConeVolumePage>();
                services.AddTransient<CylinderVolumeViewModel>();
                services.AddTransient<CylinderVolumePage>();
                services.AddTransient<CuboidVolumeViewModel>();
                services.AddTransient<CuboidVolumePage>();
                services.AddTransient<CubeVolumeViewModel>();
                services.AddTransient<CubeVolumePage>();
                services.AddTransient<SurfaceAreaViewModel>();
                services.AddTransient<SurfaceAreaPage>();
                services.AddTransient<VolumeViewModel>();
                services.AddTransient<VolumePage>();
                services.AddTransient<SCircleAreaViewModel>();
                services.AddTransient<SCircleAreaPage>();
                services.AddTransient<CircleAreaViewModel>();
                services.AddTransient<CircleAreaPage>();
                services.AddTransient<RhombusAreaViewModel>();
                services.AddTransient<RhombusAreaPage>();
                services.AddTransient<RectAreaViewModel>();
                services.AddTransient<RectAreaPage>();
                services.AddTransient<SquareAreaViewModel>();
                services.AddTransient<SquareAreaPage>();
                services.AddTransient<ITAreaViewModel>();
                services.AddTransient<ITAreaPage>();
                services.AddTransient<ETAreaViewModel>();
                services.AddTransient<ETAreaPage>();
                services.AddTransient<AreaViewModel>();
                services.AddTransient<AreaPage>();
                services.AddTransient<ApproxViewModel>();
                services.AddTransient<ApproxPage>();
                services.AddTransient<FactorialViewModel>();
                services.AddTransient<FactorialPage>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<CIViewModel>();
                services.AddTransient<CIPage>();
                services.AddTransient<SIViewModel>();
                services.AddTransient<SIPage>();
                services.AddTransient<HFViewModel>();
                services.AddTransient<HFPage>();
                services.AddTransient<CalculatorViewModel>();
                services.AddTransient<CalculatorPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<SplashPage>();
                services.AddTransient<SplashViewModel>();

                // Configuration
                services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            }).
            Build();

            UnhandledException += App_UnhandledException;
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            // Load Mica setting
            var micaService = App.GetService<IMicaService>();
            await micaService.LoadMicaSettingAsync();

            // Display the splash screen Page
            var splashScreen = new SplashPage(); // Instantiate your SplashPage
            MainWindow.Content = splashScreen;
            MainWindow.Activate();

            // Wait for the specified delay
            await Task.Delay(1300);

            // Start the fade-out animation
            var fadeOutStoryboard = (Storyboard)splashScreen.Resources["FadeOutStoryboard"];
            fadeOutStoryboard.Begin();

            // Wait for the duration of the fade-out animation
            await Task.Delay(400);

            // Navigate to the main page and activate
            var shellViewModel = App.GetService<ShellViewModel>();
            MainWindow.Content = new ShellPage(shellViewModel);
            await App.GetService<IActivationService>().ActivateAsync(args);
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {       
        }        
    }
}
