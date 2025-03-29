namespace UIThemes
{
	using System;

	/// <summary>
	/// Theme property attribute.
	/// Mark editable fields.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ThemePropertyAttribute : Attribute
	{
		/// <summary>
		/// Theme property name.
		/// </summary>
		public readonly string ThemeProperty;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThemePropertyAttribute"/> class.
		/// </summary>
		/// <param name="themeProperty">Theme property name.</param>
		public ThemePropertyAttribute(string themeProperty)
		{
			ThemeProperty = themeProperty;
		}
	}
}