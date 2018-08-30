using System;

namespace RobotManager.Core.DI
{
	public interface IDiResolver
	{
		void Register<T, K>() where T : class where K : class;

		void Register<T>() where T : class;

		void RegisterAsSingleton<T, K>() where T : class where K : class;

		void Register<T, K>(T instance) where T : class where K : class;

		void Register<T>(T instance) where T : class;

		void RegisterAsSingleton<T>() where T : class;

		void RegisterGeneric(Type implementer, Type abstraction);

		/// <summary>
		/// Resolve this instance. IoC container is builded after Resolve method firs call and you can't register after this.
		/// </summary>
		T Resolve<T>() where T : class;

		/// <summary>
		/// Resolve this instance. IoC container is builded after Resolve method firs call and you can't register after this.
		/// </summary>
		object Resolve(Type type);
	}
}