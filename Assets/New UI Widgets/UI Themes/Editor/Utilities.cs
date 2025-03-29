#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Utilities.
	/// </summary>
	public static class Utilities
	{
		/// <summary>
		/// Select assets path.
		/// </summary>
		/// <param name="path">Current path.</param>
		/// <param name="info">Information.</param>
		/// <param name="defaultName">Default folder name.</param>
		/// <returns>Assets path.</returns>
		public static string SelectAssetsPath(string path, string info, string defaultName = null)
		{
			var result = EditorUtility.OpenFolderPanel(info, path, defaultName);

			return RelativeAssetsPath(result);
		}

		/// <summary>
		/// Get relative assets path.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <returns>Relative assets path.</returns>
		public static string RelativeAssetsPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			var temp = path;
			var project_root = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);

			if (!temp.StartsWith(project_root))
			{
				return string.Empty;
			}

			var relative_path = temp.Substring(project_root.Length);
			if (relative_path.StartsWith("Assets/") || relative_path == "Assets")
			{
				return relative_path;
			}

			return string.Empty;
		}
	}
}
#endif