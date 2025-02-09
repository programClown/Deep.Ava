using Deep.Navigation.Core;
using System.Collections.ObjectModel;

namespace Deep.Navigation.Abstracts
{
    public interface IModuleManager
    {
        IModule? CurrentModule { get; }
        IEnumerable<IModule> Modules
        {
            get;
        }
        ObservableCollection<IModule> ActiveModules
        {
            get;
        }
        IView CreateView(IModule module);
        IView GetOrCreateView(IModule module, string regionName);
        void RequestNavigate(string moduleName, NavigationParameters parameters);
        void RequestNavigate(IModule module, NavigationParameters parameters);
    }
}
