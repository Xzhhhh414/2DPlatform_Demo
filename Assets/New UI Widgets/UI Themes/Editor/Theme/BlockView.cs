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
		/// Block view.
		/// </summary>
		public class BlockView
		{
			/// <summary>
			/// Block.
			/// </summary>
			public VisualElement Block
			{
				get;
				private set;
			}

			/// <summary>
			/// Visible.
			/// </summary>
			public bool Visible
			{
				get
				{
					return Block.parent != null;
				}

				set
				{
					Block.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="BlockView"/> class.
			/// </summary>
			/// <param name="name">Name.</param>
			public BlockView(string name)
			{
				Block = new VisualElement
				{
					name = name,
				};
			}
		}
	}
}
#endif