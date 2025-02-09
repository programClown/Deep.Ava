using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Deep.Ava.ViewModels;

public class ViewModelBase : ObservableObject, IDisposable
{
    public virtual void Dispose()
    {
    }
}