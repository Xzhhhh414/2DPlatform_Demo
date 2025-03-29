namespace UIThemes
{
	using System;

	/// <summary>
	/// Variation.
	/// </summary>
	[Serializable]
	public class Variation : Named<VariationId>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Variation"/> class.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <param name="name">Name.</param>
		public Variation(VariationId id, string name)
			: base(id, name)
		{
		}
	}
}