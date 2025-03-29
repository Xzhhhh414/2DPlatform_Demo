#if UNITY_EDITOR
namespace UIWidgets
{
	using System.IO;
	using UnityEditor;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class CursorsMenuOptions
	{
		/// <summary>
		/// Creates the style.
		/// </summary>
		[MenuItem("Assets/Create/New UI Widgets/Cursors", false)]
		public static void CreateStyle()
		{
			var path = UtilitiesEditor.SelectedAssetPath() + Path.DirectorySeparatorChar + "New UI Widgets Cursors.asset";
			var cursors = UtilitiesEditor.CreateScriptableObjectAsset<Cursors>(path);

			Selection.activeObject = cursors;
		}
	}
}
#endif