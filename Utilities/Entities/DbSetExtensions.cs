using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Utilities.Extensions {

	public static class DbSetExtensions {

		public static void Clear<T>(this DbSet<T> set) where T : class {
			var items = set.ToList();
			set.RemoveRange(items);
		}

	}

}