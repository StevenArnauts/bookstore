using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Utilities.Extensions {

	public static class DbSetExtensions {

		public static void Clear<T>(this DbSet<T> set) where T : class {
			var items = set.ToList();
			set.RemoveRange(items);
		}

		public static void Clear<T>(this DbSet<T> set, Expression<Func<T, bool>> condition) where T : class {
			var items = set.Where(condition).ToList();
			set.RemoveRange(items);
		}

	}

}