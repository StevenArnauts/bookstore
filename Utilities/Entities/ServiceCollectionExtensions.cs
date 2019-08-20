using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Extensions;
using Utilities.Logging;

namespace Utilities.Entities {

	public static class ServiceCollectionExtensions {

		public static void AddEntities(this IServiceCollection collection, params Assembly[] assemblies) {
			AddImplementationsOf(collection, typeof(IFactory), assemblies);
			AddImplementationsOf(collection, typeof(IRepository), assemblies);
			AddImplementationsOf(collection, typeof(IDomainService), assemblies);
			AddImplementationsOf(collection, typeof(ISeed), assemblies);
		}

		public static void UseSeed<TSeed>(this IApplicationBuilder app) where TSeed: ISeed {
			using(var scope = app.ApplicationServices.CreateScope()) {
				TSeed seed = scope.ServiceProvider.GetService<TSeed>();
				seed.Run();
			}		
		}

		private static void AddImplementationsOf(IServiceCollection collection, Type @interface, IEnumerable<Assembly> assemblies) {
			foreach (Assembly assembly in assemblies) {
				Logger.Debug(typeof(ServiceCollectionExtensions), "Looking for implementations of " + @interface.Name + " in assembly " + assembly.GetName().Name);
				foreach (Type type in assembly.ExportedTypes) {
					if (!type.IsAbstract && type.Implements(@interface)) {
						collection.AddScoped(type);
						Logger.Debug(typeof(Entity), "Registered " + type.FullName);
					}
				}
			}
		}

	}

}