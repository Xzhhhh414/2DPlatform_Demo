namespace UIThemes
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Targets wrapper.
	/// </summary>
	public readonly struct TargetsWrapper
	{
		readonly List<Target> targets;

		readonly ExclusionList excludedProperties;

		/// <summary>
		/// Initializes a new instance of the <see cref="TargetsWrapper"/> struct.
		/// </summary>
		/// <param name="targets">Targets wrapper.</param>
		/// <param name="excludedProperties">Excluded properties.</param>
		public TargetsWrapper(List<Target> targets, ExclusionList excludedProperties)
		{
			this.targets = targets;
			this.excludedProperties = excludedProperties;
		}

		/// <summary>
		/// Has missing components.
		/// </summary>
		public readonly bool HasMissingComponents
		{
			get
			{
				foreach (var target in targets)
				{
					if (target.MissingComponent)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Add property of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="property">Property.</param>
		/// <returns>true if property was added; otherwise false.</returns>
		public readonly bool Add(Component component, string property)
		{
			var enabled = !excludedProperties.Contains(component, property);
			if (Has(component, property, out var target))
			{
				target.Enabled = enabled;
				return false;
			}

			targets.Add(new Target(component, property, enabled));

			return true;
		}

		readonly bool Has(Component component, string property, out Target result)
		{
			result = null;
			foreach (var target in targets)
			{
				if (ReferenceEquals(target.Component, component) && (target.Property == property))
				{
					result = target;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Has property of specified component?
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="property">Property.</param>
		/// <returns>true if has property of specified component; otherwise false.</returns>
		public readonly bool Has(Component component, string property) => Has(component, property, out var _);
	}
}