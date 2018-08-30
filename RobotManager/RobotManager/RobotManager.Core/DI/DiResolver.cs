using System;
using Autofac;

namespace RobotManager.Core.DI
{
	public class DiResolver : IDiResolver
	{
		public static IDiResolver Instance = new DiResolver();

		private readonly ContainerBuilder _builder;

		private IContainer _container;

		private DiResolver()
		{
			_builder = new ContainerBuilder();
		}

		public void Register<T, K>() where T : class where K : class
		{
			_builder.RegisterType<T>().As<K>();
		}

		public void Register<T, K>(T instance) where T : class where K : class
		{
			_builder.RegisterInstance(instance).As<K>();
		}

		public void Register<T>(T instance) where T : class
		{
			_builder.RegisterInstance(instance).As<T>();
		}

		public void Register<T>() where T : class
		{
			_builder.RegisterType<T>();
		}

		public void RegisterGeneric(Type implementer, Type abstration)
		{
			_builder.RegisterGeneric(implementer).As(abstration);
		}

		public T Resolve<T>() where T : class
		{
			if (_container == null) _container = _builder.Build();

			return _container.Resolve<T>();
		}

		public object Resolve(Type type)
		{
			if (_container == null) _container = _builder.Build();

			return _container.Resolve(type);
		}

		public void RegisterAsSingleton<T>() where T : class
		{
			_builder.RegisterType<T>().SingleInstance();
		}

		public void RegisterAsSingleton<T, K>() where T : class where K : class
		{
			_builder.RegisterType<T>().As<K>().SingleInstance();
		}
	}
}