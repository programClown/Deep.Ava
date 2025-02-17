namespace Deep.Shell;

public class DefaultNavigationViewLocator : INavigationViewLocator
{
    public object GetView(NavigationNode navigationItem)
    {
        return Activator.CreateInstance(navigationItem.Page)
               ?? throw new TypeLoadException("Cannot create instance of page type");
    }
}