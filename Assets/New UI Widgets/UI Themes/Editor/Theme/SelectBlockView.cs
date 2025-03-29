#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using UnityEditor;
	using UnityEditor.UIElements;
	using UnityEngine.UIElements;

	/// <summary>
	/// Theme editor window.
	/// </summary>
	public partial class ThemeEditor : EditorWindow
	{
		/// <summary>
		/// Theme select view.
		/// </summary>
		protected class SelectBlockView : BlockView
		{
			/// <summary>
			/// Action on theme changed.
			/// </summary>
			public event Action<Theme> OnThemeChanged;

			/// <summary>
			/// Initializes a new instance of the <see cref="SelectBlockView"/> class.
			/// </summary>
			public SelectBlockView()
				: base("theme-select")
			{
				var label = new Label("Select Theme")
				{
					name = "theme-header",
				};
				Block.Add(label);

				var theme_field = new ObjectField("Theme")
				{
					name = "theme-field",
					objectType = typeof(Theme),
				};
				theme_field.RegisterValueChangedCallback(ThemeChanged);
				Block.Add(theme_field);
			}

			void ThemeChanged(ChangeEvent<UnityEngine.Object> ev)
			{
				OnThemeChanged?.Invoke(ev.newValue as Theme);
			}
		}
	}
}
#endif