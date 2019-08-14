using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Extensions;
using Utilities.Logging;

namespace Bookstore.Identity.Entities {

	public static class ServiceCollectionExtensions {

		private static List<Type> domainInterfaces = new List<Type> {
			typeof(IRepository)
		};

		public static void UseEntities(this IServiceCollection services) {
			UseEntities(services, typeof(ServiceCollectionExtensions).Assembly);
		}

		public static void UseEntities(this IServiceCollection services, params Assembly[] assemblies) {
			services.AddTransient<Seed>();
			foreach (Assembly assembly in assemblies) {
				Logger.Debug(typeof(ServiceCollectionExtensions), "Scanning assembly " + assembly.GetName().Name);
				foreach (Type type in assembly.ExportedTypes) {
					foreach (Type interfaceToRegister in domainInterfaces) {
						if (!type.IsAbstract && type.Implements(interfaceToRegister)) {
							Logger.Info(typeof(ServiceCollectionExtensions), type.FullName + " implements " + interfaceToRegister.FullName);
							services.AddSingleton(type); // add the type as itself, the interface is just a marker
						}
					}
				}
			}
		}

	}

}