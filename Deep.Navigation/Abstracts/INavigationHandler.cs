using System.Collections.ObjectModel;

namespace Deep.Navigation.Abstracts
{
    public interface INavigationHandler : IModuleNavigationHandler<IModule>, IViewNavigationHandler
    {
        IRegionManager RegionManager { get; }
        IModuleManager ModuleManager { get; }
    }
}
