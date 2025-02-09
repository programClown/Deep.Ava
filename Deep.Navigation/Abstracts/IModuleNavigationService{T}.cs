using Deep.Navigation.Core;

namespace Deep.Navigation.Abstracts
{
    public interface IModuleNavigationService<T> : IModuleNavigationService where T : IModule
    {
        IDisposable BindingNavigationHandler(IModuleNavigationHandler<T> handler);
        void RequestModuleNavigate(T module, NavigationParameters parameters);
    }
}
