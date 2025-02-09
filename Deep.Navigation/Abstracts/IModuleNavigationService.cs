using Deep.Navigation.Core;

namespace Deep.Navigation.Abstracts
{
    public interface IModuleNavigationService
    {
        IDisposable BindingNavigationHandler(IModuleNavigationHandler handler);
        void RequestModuleNavigate(string moduleKey, NavigationParameters parameters);
    }
}
