using Deep.Navigation.Core;

namespace Deep.Navigation.Abstracts
{
    public interface INavigationAware
    {
        void OnNavigatedTo(NavigationContext navigationContext);
        bool IsNavigationTarget(NavigationContext navigationContext);
        void OnNavigatedFrom(NavigationContext navigationContext);
    }
}
