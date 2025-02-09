using Deep.Navigation.Core;
using System.Collections.ObjectModel;

namespace Deep.Navigation.Abstracts
{
    public interface IRegion
    {
        string Name { get; }
        ObservableCollection<NavigationContext> Contexts 
        { 
            get;
        }
        void Activate(NavigationContext target);
        void DeActivate(NavigationContext target);
    }
}
