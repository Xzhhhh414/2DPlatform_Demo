namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the ModalColor of Sidebar.
	/// </summary>
	public class SidebarModalColor : Wrapper<Color, Sidebar>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SidebarModalColor"/> class.
		/// </summary>
		public SidebarModalColor()
		{
			Name = nameof(Sidebar.ModalColor);
		}

		/// <inheritdoc/>
		protected override Color Get(Sidebar widget)
		{
			return widget.ModalColor;
		}

		/// <inheritdoc/>
		protected override void Set(Sidebar widget, Color value)
		{
			widget.ModalColor = value;
		}
	}
}