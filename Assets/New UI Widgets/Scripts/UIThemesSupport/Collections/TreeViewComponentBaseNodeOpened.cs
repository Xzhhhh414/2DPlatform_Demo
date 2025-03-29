namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the NodeOpened of TreeViewComponentBase.
	/// </summary>
	public class TreeViewComponentBaseNodeOpened : Wrapper<Sprite, TreeViewComponentBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TreeViewComponentBaseNodeOpened"/> class.
		/// </summary>
		public TreeViewComponentBaseNodeOpened()
		{
			Name = nameof(TreeViewComponentBase.NodeOpened);
		}

		/// <inheritdoc/>
		protected override Sprite Get(TreeViewComponentBase widget)
		{
			return widget.NodeOpened;
		}

		/// <inheritdoc/>
		protected override void Set(TreeViewComponentBase widget, Sprite value)
		{
			widget.NodeOpened = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(TreeViewComponentBase widget)
		{
			return widget.NodeOpened != null;
		}
	}
}