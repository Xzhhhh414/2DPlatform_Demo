namespace UIThemes
{
	using System;

	/// <summary>
	/// Property group attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	[Obsolete("Renamed to the [PropertyGroup].")]
	public class PropertyAttribute : PropertyGroupAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
		/// </summary>
		/// <param name="fieldView">Type of the field view.</param>
		/// <param name="undoName">Undo name.</param>
		public PropertyAttribute(Type fieldView, string undoName)
			: base(fieldView, undoName)
		{
		}
	}
}