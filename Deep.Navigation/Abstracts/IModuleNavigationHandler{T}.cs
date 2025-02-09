using Deep.Navigation.Core;

namespace Deep.Navigation.Abstracts
{
    public interface IModuleNavigationHandler<in T> :IModuleNavigationHandler where T : IModule
    {
        void OnNavigateTo(T module, NavigationParameters parameter);
    }
}
