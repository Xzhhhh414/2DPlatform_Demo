namespace UIThemes
{
	using System;

	/// <summary>
	/// Option.
	/// </summary>
	[Serializable]
	public class Option : Named<OptionId>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Option"/> class.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <param name="name">Name.</param>
		public Option(OptionId id, string name)
			: base(id, name)
		{
		}
	}
}