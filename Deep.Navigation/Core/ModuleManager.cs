﻿using Deep.Navigation.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deep.Navigation.Core
{
    public class ModuleManager : IModuleManager, INotifyPropertyChanged
    {
        private readonly ConcurrentDictionary<string, IModule> _modulesCache;
        private readonly ConcurrentDictionary<(string, string), IView> _regionCache;
        private readonly IServiceProvider _serviceProvider;
        public ModuleManager(IEnumerable<IModule> modules, IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
            _regionCache = [];
            _modulesCache = new ConcurrentDictionary<string, IModule>(modules.ToDictionary(m => m.Key, m => m));
            Modules = _modulesCache.Values;
            ActiveModules = new ObservableCollection<IModule>(_modulesCache
            .Where(m =>
            {
                return !m.Value.LoadOnDemand;
            })
            .Select(m =>
            {
                m.Value.Initialize();
                return m.Value;
            }));
        }
        public ObservableCollection<IModule> ActiveModules 
        { 
            get; 
        }

        public IEnumerable<IModule> Modules
        {
            get;
        }

        private IModule? _currentModule;
        public IModule? CurrentModule
        {
            get => _currentModule;
            set
            {
                if (_currentModule != value)
                {
                    _currentModule = value;
                    OnPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RequestNavigate(string moduleName, NavigationParameters parameters)
        {
            if (_modulesCache.TryGetValue(moduleName, out var module))
            {
                RequestNavigate(module, parameters);
            }
            else
            {
                throw new KeyNotFoundException(moduleName);
            }
        }

        public void RequestNavigate(IModule module, NavigationParameters parameters)
        {
            if (module.ForceNew)
            {
                module = _serviceProvider.GetKeyedService<IModule>(module.Key)!;
                ActiveModules.Add(module);
            }
            else
            {
                if (!ActiveModules.Contains(module))
                {
                    ActiveModules.Add(module);
                }
            }

            ///TODO:Consider an async implementation
            module.Initialize();
            module.IsActivated = true;
            CurrentModule = module;
        }

        public IView CreateView(IModule module)
        {
            var view = _serviceProvider.GetRequiredKeyedService<IView>(module.Key);
            var viewmodel = _serviceProvider.GetRequiredKeyedService<IModuleNavigationAware>(module.Key);
            view.DataContext = viewmodel;
            return view;
        }
        public IView GetOrCreateView(IModule module, string regionName)
        {
            if (module.ForceNew)
            {
                return CreateView(module);
            }
            if (_regionCache.TryGetValue((regionName, module.Key), out IView? cache))
            {
                return cache;
            }
            else
            {
                var view = CreateView(module);
                _regionCache.TryAdd((regionName, module.Key), view);
                return view;
            }
        }
    }
}
