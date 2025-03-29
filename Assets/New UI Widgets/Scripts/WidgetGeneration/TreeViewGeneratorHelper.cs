namespace UIWidgets.WidgetGeneration
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeView generator helper.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/generator.html")]
	public class TreeViewGeneratorHelper : ListViewGeneratorHelper
	{
		/// <summary>
		/// Viewport.
		/// </summary>
		public LayoutElement Indentation;

		/// <summary>
		/// Toggle.
		/// </summary>
		public TreeNodeToggle Toggle;
	}
}