﻿using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Deep.Navigation.Abstracts;
using Deep.Navigation.Core;
using Deep.Navigation.Framework;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Deep.Navigation.Avaloniaui.Regions
{
    public abstract class AvaloniauiRegion : IRegion
    {
        private readonly ConcurrentDictionary<string, IView> _viewCache = new();
        private readonly ConcurrentItem<(IView View, INavigationAware NavigationAware)> _current = new();

        public AvaloniauiRegion()
        {
            RegionTemplate = CreateRegionDataTemplate();
        }

        public abstract string Name { get; }
        public abstract ObservableCollection<NavigationContext> Contexts { get; }

        public IDataTemplate? RegionTemplate { get; set; }

        public abstract void Activate(NavigationContext target);
        public abstract void DeActivate(NavigationContext target);

        private IDataTemplate CreateRegionDataTemplate()
        {
            return new FuncDataTemplate<NavigationContext>((context, np) =>
            {
                if (context == null)
                    return null;

                bool needNewView = context.RequestNew ||
                    !_viewCache.TryGetValue(context.TargetViewName, out IView? view);

                if (needNewView)
                {
                    // Create new view and navigation aware instance
                    view = context.ServiceProvider.GetRequiredKeyedService<IView>(context.TargetViewName);
                    var navigationAware = context.ServiceProvider.GetRequiredKeyedService<INavigationAware>(context.TargetViewName);

                    // Handle previous navigation
                    if (_current.TryTakeData(out var previousData))
                    {
                        previousData.NavigationAware.OnNavigatedFrom(context);
                    }

                    // Validate navigation target
                    if (!navigationAware.IsNavigationTarget(context))
                        return null;

                    // Setup new view
                    view.DataContext = navigationAware;
                    navigationAware.OnNavigatedTo(context);
                    _current.SetData((view, navigationAware));
                    _viewCache.TryAdd(context.TargetViewName, view);
                }
                else
                {
                    view = _viewCache[context.TargetViewName];
                }

                return view as Control;
            });
        }
    }
}