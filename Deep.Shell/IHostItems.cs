using System.Collections;
using Avalonia.Controls;

namespace Deep.Shell;

public interface IHostItems
{
    IEnumerable? ItemsSource { get; set; }
    ItemCollection Items { get; }
}