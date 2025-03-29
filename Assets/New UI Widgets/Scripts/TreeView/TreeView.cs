namespace UIWidgets
{
	/// <summary>
	/// TreeView.
	/// </summary>
	public class TreeView : TreeViewCustom<TreeViewComponent, TreeViewItem>
	{
		/// <summary>
		/// NodeToggleProxy event.
		/// </summary>
		public TreeViewNodeEvent NodeToggleProxy = new TreeViewNodeEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			NodeToggle.AddListener(NodeToggleProxy.Invoke);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			NodeToggle.RemoveListener(NodeToggleProxy.Invoke);

			base.OnDestroy();
		}
	}
}