using Deep.Navigation.Core;

namespace Deep.Navigation.Abstracts
{
    public interface IViewNavigationHandler
    {
        void OnNavigateTo(string regionName, 
            string viewName, 
            bool requestNew);
        void OnNavigateTo(string regionName, 
            string viewName,
            NavigationParameters parameters,
            bool requestNew);
    }
}
