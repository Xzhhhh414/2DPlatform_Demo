namespace UIThemes
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Registry of properties created with reflection.
	/// </summary>
	public static class ReflectionWrappersRegistry
	{
		[DomainReloadExclude]
		static readonly Dictionary<Type, HashSet<string>> Types = new Dictionary<Type, HashSet<string>>();

		/// <summary>
		/// Action on registry data changed.
		/// </summary>
		[DomainReloadExclude]
		public static event Action OnChanged;

		/// <summary>
		/// Add.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="property">Property or field name.</param>
		public static void Add(Type type, string property)
		{
			if (!Types.TryGetValue(type, out var properties))
			{
				properties = new HashSet<string>();
				Types[type] = properties;
			}

			properties.Add(property);

			OnChanged?.Invoke();
		}

		/// <summary>
		/// Get all registered properties.
		/// </summary>
		/// <returns>Registered properties.</returns>
		public static IReadOnlyDictionary<Type, IReadOnlyCollection<string>> All()
		{
			var result = new Dictionary<Type, IReadOnlyCollection<string>>();
			foreach (var item in Types)
			{
				result[item.Key] = item.Value;
			}

			return result;
		}
	}
}