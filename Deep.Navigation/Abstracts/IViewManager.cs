namespace Deep.Navigation.Abstracts
{
    public interface IViewManager : IObservable<IView>
    {
        IEnumerable<IView> Views { get; }
    }
}
