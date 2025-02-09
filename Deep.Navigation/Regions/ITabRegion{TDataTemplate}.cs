using Deep.Navigation.Abstracts;

namespace Deep.Navigation.Regions
{
    public interface ITabRegion<TDataTemplate> : IRegion
    {
        object? SelectedItem
        {
            get;
            set;
        }
        TDataTemplate? RegionTemplate
        {
            get;
            set;
        }
        void Add(object item);
    }
}
