using Deep.Navigation.Abstracts;

namespace Deep.Navigation.Regions
{
    public interface IContentRegion<TDataTemplate> : IRegion
    {
        public object? Content
        {
            get;
            set;
        }
        TDataTemplate? RegionTemplate
        {
            get;
            set;
        }
    }
}
