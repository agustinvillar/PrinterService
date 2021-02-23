using System;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IDependencyResolver
    {
        void Register<TInterface, TImplementation>() where TImplementation : TInterface;

        void Register<TInterface, TImplementation>(Func<TImplementation> constructor) where TImplementation : TInterface;

        void Register<TImplementation>(Func<TImplementation> constructor);

        void Register<TImplementation>(Func<TImplementation> constructor, string name);

        void Register(Type type1, Type type2, string name);

        void RegisterByName<TInterface, TImplementation>(string name) where TImplementation : TInterface;

        void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface;

        void RegisterSingleton(Type type1, Type type2, string name);

        void RegisterSingleton<TImplementation>(Func<TImplementation> constructor);

        T Resolve<T>();

        IEnumerable<T> ResolveAll<T>();

        IEnumerable<object> ResolveAll(Type type);

        T ResolveByName<T>(string name);
    }
}
