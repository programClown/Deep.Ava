using Avalonia.Animation;
using Deep.Shell.Platform.Windows;

namespace Deep.Shell.Platform;

public class PlatformSetup
{
    public static IPageTransition TransitionForPage
    {
        get
        {
            // if (OperatingSystem.IsAndroid())
            //     return AndroidDefaultPageSlide.Instance;
            // if (OperatingSystem.IsIOS())
            //     return DefaultIosPageSlide.Instance;
            if (OperatingSystem.IsWindows())
                return EntranceNavigationTransition.Instance;

            //Default for the moment
            return DrillInNavigationTransition.Instance;
        }
    }

    public static IPageTransition TransitionForList =>
        // if (OperatingSystem.IsAndroid())
        //     return MaterialListPageSlide.Instance;
        // if (OperatingSystem.IsIOS())
        //     return DefaultIosPageSlide.Instance;
        //Default for the moment
        ListSlideNavigationTransition.Instance;

    public static IPageTransition? TransitionForTab
    {
        get
        {
            if (OperatingSystem.IsIOS()) return null;
            if (OperatingSystem.IsMacOS()) return null;
            if (OperatingSystem.IsMacCatalyst()) return null;

            //Default for the moment
            // return MaterialListPageSlide.Instance;
            return null;
        }
    }

    public static IPageTransition TransitionForModal
    {
        get
        {
            // if (OperatingSystem.IsAndroid())
            //     return AndroidDefaultPageSlide.Instance;
            // if (OperatingSystem.IsIOS())
            //     return AlertTransition.Instance;
            if (OperatingSystem.IsWindows())
                return DrillInNavigationTransition.Instance;

            //Default for the moment
            return DrillInNavigationTransition.Instance;
        }
    }

    public static NavigationBarAttachType NavigationBarAttachType => NavigationBarAttachType.ToFirstHostThenPage;
}