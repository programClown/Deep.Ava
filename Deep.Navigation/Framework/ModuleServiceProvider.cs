﻿using Deep.Navigation.Abstracts;

namespace Deep.Navigation.Internals
{
    internal sealed class ModuleServiceProvider : IModuleServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public ModuleServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? GetService(Type serviceType)
        {
            return GetServiceInternal(serviceType);
        }

        private object? GetServiceInternal(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
    }
}
