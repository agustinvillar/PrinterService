using Menoo.PrinterService.Infraestructure.Interfaces;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Menoo.PrinterService.Infraestructure.DI
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _unityContainer;

        public UnityDependencyResolver()
        {
            _unityContainer = new UnityContainer();
        }

        public UnityDependencyResolver(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer ?? throw new ArgumentNullException("unityContainer");
        }

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _unityContainer.RegisterType<TInterface, TImplementation>();
        }

        public void Register<TInterface, TImplementation>(Func<TImplementation> constructor) where TImplementation : TInterface
        {
            _unityContainer.RegisterType<TInterface, TImplementation>(new InjectionFactory(c =>
            {
                return constructor();
            }));
        }

        public void Register<TImplementation>(Func<TImplementation> constructor)
        {
            _unityContainer.RegisterType<TImplementation>(new InjectionFactory(c =>
            {
                return constructor();
            }));
        }

        public void Register(Type type1, Type type2, string name)
        {
            _unityContainer.RegisterType(type1, type2, name);
        }

        public void RegisterByName<TInterface, TImplementation>(string name) where TImplementation : TInterface
        {
            _unityContainer.RegisterType<TInterface, TImplementation>(name);
        }

        public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _unityContainer.RegisterType<TInterface, TImplementation>(new ContainerControlledLifetimeManager());
        }

        public void RegisterSingleton(Type type1, Type type2, string name)
        {
            _unityContainer.RegisterType(type1, type2, name, new ContainerControlledLifetimeManager());
        }

        public void RegisterSingleton<TImplementation>(Func<TImplementation> constructor)
        {
            _unityContainer.RegisterType<TImplementation>(new ContainerControlledLifetimeManager(),
                new InjectionFactory(c =>
                {
                    return constructor();
                }));
        }

        public T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _unityContainer.ResolveAll<T>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            return _unityContainer.ResolveAll(type);
        }

        public T ResolveByName<T>(string name)
        {
            return _unityContainer.Resolve<T>(name);
        }
    }
}
