namespace Deep.Shell;

public interface INavigationBarProvider
{
    NavigationBar? NavigationBar { get; }
    NavigationBar? AttachedNavigationBar { get; }
}