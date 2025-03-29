#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using UnityEditor;
	using UnityEngine.UIElements;

	/// <summary>
	/// Theme editor window.
	/// </summary>
	public partial class ThemeEditor : EditorWindow
	{
		/// <summary>
		/// Theme warning view.
		/// </summary>
		protected class ThemeWarningBlockView : BlockView
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ThemeWarningBlockView"/> class.
			/// </summary>
			public ThemeWarningBlockView()
				: base("theme-select")
			{
				var label = new Label("The default Theme cannot be edited to avoid losing change when updating the package.\nUse the context menu \"Create / UI Themes / Theme\" to create a clone of the default Theme.")
				{
					name = "default-theme-warning",
				};
				Block.Add(label);
			}
		}
	}
}
#endif