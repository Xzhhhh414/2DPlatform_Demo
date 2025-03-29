#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using UnityEditor;
	using UnityEngine.UIElements;

	/// <summary>
	/// Theme inspector.
	/// </summary>
	[CustomEditor(typeof(Theme), true)]
	public class ThemeInspector : Editor
	{
		/// <summary>
		/// Theme.
		/// </summary>
		protected Theme Theme;

		/// <summary>
		/// Create GUI.
		/// </summary>
		/// <returns>Root element.</returns>
		public override VisualElement CreateInspectorGUI()
		{
			Theme = target as Theme;

			var root = new VisualElement();
			foreach (var style in ReferencesGUIDs.GetStyleSheets())
			{
				root.styleSheets.Add(style);
			}

			root.AddToClassList("theme-inspector");

			var button = new Button(() => ThemeEditor.Open(Theme))
			{
				text = "Open Editor",
			};
			button.AddToClassList("theme-inspector-button");
			root.Add(button);

			return root;
		}
	}
}
#endif