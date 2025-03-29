namespace UIThemes
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Target.
	/// </summary>
	[Serializable]
	public class Target
	{
		/// <summary>
		/// Active.
		/// </summary>
		public bool Active => Enabled && (Owner == null) && (Component != null) && !string.IsNullOrEmpty(Property) && OptionId != OptionId.None;

		/// <summary>
		/// Component is missing.
		/// </summary>
		public bool MissingComponent => !ReferenceEquals(Component, null) && (Component == null);

		/// <summary>
		/// Enabled.
		/// </summary>
		[SerializeField]
		public bool Enabled = true;

		/// <summary>
		/// Property owner.
		/// Used if property controlled by external component/widget, not by ThemeTarget.
		/// </summary>
		[SerializeField]
		public Component Owner = null;

		/// <summary>
		/// Component.
		/// </summary>
		[SerializeField]
		public Component Component;

		/// <summary>
		/// Property.
		/// </summary>
		[SerializeField]
		public string Property;

		/// <summary>
		/// Option ID.
		/// </summary>
		[SerializeField]
		public OptionId OptionId = OptionId.None;

		/// <summary>
		/// Initializes a new instance of the <see cref="Target"/> class.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="property">Property.</param>
		/// <param name="enabled">Enabled.</param>
		public Target(Component component, string property, bool enabled = true)
		{
			Enabled = enabled;
			Component = component;
			Property = property;
		}
	}
}