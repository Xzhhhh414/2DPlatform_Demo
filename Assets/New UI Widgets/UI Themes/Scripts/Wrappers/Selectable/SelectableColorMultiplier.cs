namespace UIThemes.Wrappers
{
	using UnityEngine.UI;

	/// <summary>
	/// Theme property for the colorMultiplier of Selectable.
	/// </summary>
	public class SelectableColorMultiplier : Wrapper<ColorMultiplierValue, Selectable>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SelectableColorMultiplier"/> class.
		/// </summary>
		public SelectableColorMultiplier() => Name = nameof(Selectable.colors.colorMultiplier);

		/// <inheritdoc/>
		protected override ColorMultiplierValue Get(Selectable widget) => widget.colors.colorMultiplier;

		/// <inheritdoc/>
		protected override void Set(Selectable widget, ColorMultiplierValue value)
		{
			var colors = widget.colors;
			colors.colorMultiplier = value;
			widget.colors = colors;
		}

		/// <inheritdoc/>
		protected override bool Active(Selectable widget) => widget.transition == Selectable.Transition.ColorTint;

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(Selectable widget) => widget.colors != ColorBlock.defaultColorBlock;
	}
}