using Deep.Navigation.Core;

namespace Deep.Navigation.Abstracts
{
    public interface IViewNavigationService
    {
        IDisposable BindingViewNavigationHandler(IViewNavigationHandler handler);
        void RequestViewNavigation(string regionName, 
            string viewKey, 
            bool requestNew = false);
        void RequestViewNavigation(string regionName,
            string viewKey,
            NavigationParameters parameters,
            bool requestNew = false);
    }
}
