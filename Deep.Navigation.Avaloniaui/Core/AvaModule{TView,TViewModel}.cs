using Deep.Navigation.Abstracts;
using Deep.Navigation.Core;
using System.Diagnostics.CodeAnalysis;

namespace Deep.Navigation.Avaloniaui.Core;

public abstract class AvaModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>
    : Module<TView, TViewModel>
    where TViewModel : IModuleNavigationAware
    where TView : IView
{
    public AvaModule(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }
}