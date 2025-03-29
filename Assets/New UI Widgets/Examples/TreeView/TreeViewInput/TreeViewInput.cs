namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// TreeView with editable Item.Tag for the nested nodes.
	/// </summary>
	public class TreeViewInput : TreeView
	{
		/// <summary>
		/// Template selector.
		/// </summary>
		[SerializeField]
		protected TreeViewInputSelector Selector;

		/// <inheritdoc/>
		protected override IListViewTemplateSelector<TreeViewComponent, ListNode<TreeViewItem>> CreateTemplateSelector()
		{
			return (Selector != null) ? Selector : base.CreateTemplateSelector();
		}
	}
}