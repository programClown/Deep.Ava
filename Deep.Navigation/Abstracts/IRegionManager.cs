
using Deep.Navigation.Core;

namespace Deep.Navigation.Abstracts
{
    public interface IRegionManager: IObservable<NavigationContext>, IObservable<IRegion>
    {
        void AddRegion(string regionName, IRegion region);
        IRegion? GetRegion(string regionName);
        void RequestNavigate(string regionName, string viewName, bool requestNae, NavigationParameters? parameters = null);
    }
}
