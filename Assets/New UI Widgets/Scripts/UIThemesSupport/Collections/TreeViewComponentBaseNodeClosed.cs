namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the NodeClosed of TreeViewComponentBase.
	/// </summary>
	public class TreeViewComponentBaseNodeClosed : Wrapper<Sprite, TreeViewComponentBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TreeViewComponentBaseNodeClosed"/> class.
		/// </summary>
		public TreeViewComponentBaseNodeClosed()
		{
			Name = nameof(TreeViewComponentBase.NodeClosed);
		}

		/// <inheritdoc/>
		protected override Sprite Get(TreeViewComponentBase widget)
		{
			return widget.NodeClosed;
		}

		/// <inheritdoc/>
		protected override void Set(TreeViewComponentBase widget, Sprite value)
		{
			widget.NodeClosed = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(TreeViewComponentBase widget)
		{
			return widget.NodeClosed != null;
		}
	}
}