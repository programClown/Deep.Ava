namespace Deep.Shell;

public class HostNavigationChain : NavigationChain
{
    public List<NavigationChain> Nodes { get; } = new();

    public IEnumerable<NavigationChain> AggregatedNodes => Nodes.SelectMany(chain =>
        chain is HostNavigationChain host
            ? host.AggregatedNodes.Append(chain)
            : new[] { chain }).Append(this);
}