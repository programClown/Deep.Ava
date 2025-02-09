using Microsoft.Extensions.DependencyInjection;

namespace Deep.Navigation.Abstracts
{
    public interface IModuleScope
    {
        IServiceCollection ScopeServiceCollection { get; }
        IServiceProvider ScopeServiceProvider { get; }
    }
}
