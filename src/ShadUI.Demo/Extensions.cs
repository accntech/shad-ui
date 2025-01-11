﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Avalonia.Data.Converters;
using Microsoft.Extensions.DependencyInjection;
using ShadUI.Demo.ViewModels;

namespace ShadUI.Demo;

public static class Extensions
{
    public static void AddServices(this IServiceCollection collection)
    {
        collection.AddTransientFromNamespace("ShadUI.Demo.ViewModels", typeof(Extensions).Assembly);
    }

    public static IServiceCollection AddTransientFromNamespace(
        this IServiceCollection services,
        string namespaceName,
        params Assembly[] assemblies
    )
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly
                .GetTypes()
                .Where(x =>
                    x is { IsClass: true, Namespace: not null, IsAbstract: false } // use is IsAbstract to exclude static classes   
                    && !x.IsAssignableTo(typeof(IValueConverter)) // to exclude converters classes
                    && !x.IsAssignableTo(typeof(IMultiValueConverter)) // to exclude multi-value converters classes
                    && !x.IsAssignableTo(typeof(ValidationAttribute)) // to exclude validator classes
                    && x.Namespace.StartsWith(namespaceName, StringComparison.InvariantCultureIgnoreCase)
                );

            foreach (var type in types)
            {
                if (services.Any(x => x.ServiceType == type)) continue;

                if (type == typeof(ViewModelBase)) continue;

                _ = services.AddTransient(type);
            }
        }

        return services;
    }
}