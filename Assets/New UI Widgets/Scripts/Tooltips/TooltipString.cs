namespace UIWidgets
{
	/// <summary>
	/// Tooltip string.
	/// </summary>
	public class TooltipString : Tooltip<string, TooltipString>
	{
		/// <summary>
		/// Text.
		/// </summary>
		public TextAdapter Text;

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			Text.text = CurrentData;
		}
	}
}