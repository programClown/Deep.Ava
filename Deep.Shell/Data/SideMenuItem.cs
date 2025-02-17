using Avalonia.Media;

namespace Deep.Shell.Data;

public class SideMenuItem : IItem
{
    public string Title { get; set; }
    public string Path { get; set; }
    public IImage? Icon { get; set; }
}