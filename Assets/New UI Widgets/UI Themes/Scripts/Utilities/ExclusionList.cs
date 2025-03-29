namespace UIThemes
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Exclusion list.
	/// </summary>
	public class ExclusionList
	{
		[DomainReloadExclude]
		static readonly Stack<ExclusionList> Cache = new Stack<ExclusionList>();

		readonly Dictionary<int, HashSet<string>> exclude = new Dictionary<int, HashSet<string>>();

		private ExclusionList()
		{
		}

		/// <summary>
		/// Get ExclusionList instance.
		/// </summary>
		/// <param name="components">Components.</param>
		/// <returns>ExclusionList instance.</returns>
		public static ExclusionList Get(IReadOnlyList<Component> components)
		{
			var list = Cache.Count > 0 ? Cache.Pop() : new ExclusionList();
			Utilities.InvokeStaticMethods<ExclusionRegistryAttribute, IReadOnlyList<Component>, ExclusionList>(components, list);

			return list;
		}

		/// <summary>
		/// Return instance to the cache.
		/// </summary>
		public void Return()
		{
			exclude.Clear();
			Cache.Push(this);
		}

		/// <summary>
		/// Get properties.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Properties.</returns>
		public IReadOnlyCollection<string> Properties(Component component)
		{
			var id = component.GetInstanceID();
			if (!exclude.TryGetValue(id, out var properties))
			{
				return null;
			}

			return properties;
		}

		/// <summary>
		/// Add exclusion.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="property">Property.</param>
		public void Add(Component component, string property)
		{
			var id = component.GetInstanceID();
			if (exclude.TryGetValue(id, out var properties))
			{
				properties.Add(property);
			}
			else
			{
				exclude[id] = new HashSet<string>() { property, };
			}
		}

		/// <summary>
		/// Contains property for the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="property">Property.</param>
		/// <returns>true if contains property for the specified component; otherwise false.</returns>
		public bool Contains(Component component, string property)
		{
			var id = component.GetInstanceID();
			if (!exclude.TryGetValue(id, out var properties))
			{
				return false;
			}

			return properties.Contains(property);
		}
	}
}