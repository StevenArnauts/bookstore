using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Extensions;
using Utilities.Logging;

namespace Utilities.Entities
{
	public static class ServiceCollectionExtensions
	{
		public static void AddDomain(this IServiceCollection collection, params Assembly[] assemblies)
		{
			AddImplementationsOf(collection, typeof(IFactory), assemblies);
			AddImplementationsOf(collection, typeof(IRepository), assemblies);
			AddImplementationsOf(collection, typeof(IDomainService), assemblies);
		}

		private static void AddImplementationsOf(IServiceCollection collection, Type @interface, IEnumerable<Assembly> assemblies)
		{
			foreach (Assembly assembly in assemblies)
			{
				foreach (Type type in assembly.ExportedTypes)
				{
					if (type.Implements(@interface))
					{
						collection.AddTransient(type);
						Logger.Debug(typeof(Entity), "Registered " + type.FullName);
					}
				}
			}
		}
	}
}