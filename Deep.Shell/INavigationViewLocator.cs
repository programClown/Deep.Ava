namespace Deep.Shell;

public interface INavigationViewLocator
{
    object GetView(NavigationNode navigationItem);
}