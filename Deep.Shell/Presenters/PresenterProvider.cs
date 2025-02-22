namespace Deep.Shell.Presenters;

public class PresenterProvider : IPresenterProvider
{
    public IPresenter For(NavigateType type)
    {
        return type switch
        {
            NavigateType.Modal => new ModalPresenter(),
            _ => new GenericPresenter()
        };
    }

    public IPresenter Remove()
    {
        return new RemovePresenter();
    }
}