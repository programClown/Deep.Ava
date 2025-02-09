using Deep.Navigation.Core;

namespace Deep.Navigation.Abstracts
{
    public interface IModuleNavigationHandler
    {
        void OnNavigateTo(string moduleKey, NavigationParameters parameters);
    }
}
