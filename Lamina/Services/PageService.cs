using Lamina.ViewModels;
using Lamina.Views;

using CommunityToolkit.Mvvm.ComponentModel;
using Lamina.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

namespace Lamina.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<CalculatorViewModel, CalculatorPage>();
        Configure<HFViewModel, HFPage>();
        Configure<SIViewModel, SIPage>();
        Configure<CIViewModel, CIPage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<SplashViewModel, SplashPage>();
        Configure<FactorialViewModel, FactorialPage>();
        Configure<ApproxViewModel, ApproxPage>();
        Configure<AreaViewModel, AreaPage>();
        Configure<ETAreaViewModel, ETAreaPage>();
        Configure<ITAreaViewModel, ITAreaPage>();
        Configure<SquareAreaViewModel, SquareAreaPage>();
        Configure<RectAreaViewModel, RectAreaPage>();
        Configure<RhombusAreaViewModel, RhombusAreaPage>();
        Configure<CircleAreaViewModel, CircleAreaPage>();
        Configure<SCircleAreaViewModel, SCircleAreaPage>();
        Configure<VolumeViewModel, VolumePage>();
        Configure<SurfaceAreaViewModel, SurfaceAreaPage>();
        Configure<CubeVolumeViewModel, CubeVolumePage>();
        Configure<CuboidVolumeViewModel, CuboidVolumePage>();
        Configure<CylinderVolumeViewModel, CylinderVolumePage>();
        Configure<ConeVolumeViewModel, ConeVolumePage>();
        Configure<SphereViewModel, SpherePage>();
        Configure<CubeSAViewModel, CubeSAPage>();
        Configure<CuboidSAViewModel, CuboidSAPage>();
        Configure<CylinderSAViewModel, CylinderSAPage>();
        Configure<ConeSAViewModel, ConeSAPage>();
        Configure<SphereSAViewModel, SphereSAPage>();
        Configure<DiagonalViewModel, DiagonalPage>();
        Configure<PerimeterViewModel, PerimeterPage>();
        Configure<ETPermViewModel, ETPermPage>();
        Configure<ITPermViewModel, ITPermPage>();
        Configure<SquarePermViewModel, SquarePermPage>();
        Configure<RectPermViewModel, RectPermPage>();
        Configure<RhombusPermViewModel, RhombusPermPage>();
        Configure<CirclePermViewModel, CirclePermPage>();
        Configure<SCirclePermViewModel, SCirclePermPage>();
        Configure<SquareDiagViewModel, SquareDiagPage>();
        Configure<RectDiagViewModel, RectDiagPage>();
        Configure<CubeDiagViewModel, CubeDiagPage>();
        Configure<CuboidDiagViewModel, CuboidDiagPage>();
        Configure<PrimeCheckViewModel, PrimeCheckPage>();
        Configure<AgeCalcViewModel, AgeCalcPage>();
        Configure<TriAreaViewModel, TriAreaPage>();
        Configure<RTCheckViewModel, RTCheckPage>();
        Configure<RoomAreaViewModel, RoomAreaPage>();
        Configure<CSurfaceAreaViewModel, CSurfaceAreaPage>();
        Configure<CylinderCSAViewModel, CylinderCSAPage>();
        Configure<ConeCSAViewModel, ConeCSAPage>();
        Configure<SphereCSAViewModel, SphereCSAPage>();
        Configure<DicountViewModel, DiscountPage>();
        Configure<DateDifferenceViewModel, DateDifferencePage>();
        Configure<UnitConverterViewModel, UnitConverterPage>();
        Configure<BaseConverterViewModel, BaseConverterPage>();
        Configure<QuadEqnViewModel, QuadEqnPage>();
        Configure<CurrencyViewModel, CurrencyPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
