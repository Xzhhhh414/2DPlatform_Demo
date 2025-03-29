#if UNITY_EDITOR && UITHEMES_ADDRESSABLE_SUPPORT
namespace UIThemes.Editor
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor.Build.Reporting;
	using UnityEditor.Build;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.AddressableAssets.Build;
	using UnityEditor.AddressableAssets.Settings;
	using UnityEditor.AddressableAssets;

	/// <summary>
	/// Theme build processor to ensure addressable assets and load those assets back after build.
	/// </summary>
	public class ThemeBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
	{
		/// <summary>
		/// Callback order.
		/// </summary>
		public int callbackOrder => 0;

		/// <summary>
		/// Get themes.
		/// </summary>
		/// <returns>Themes.</returns>
		protected List<Theme> GetThemes()
		{
			var list = new List<Theme>();
			var guids = AssetDatabase.FindAssets("t:" + typeof(Theme).FullName);
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var theme = AssetDatabase.LoadAssetAtPath<Theme>(path);
				if (theme.AddressableSupport)
				{
					list.Add(theme);
				}
			}

			return list;
		}

		/// <summary>
		/// Preprocess build event.
		/// </summary>
		/// <param name="report">Report.</param>
		public void OnPreprocessBuild(BuildReport report)
		{
			foreach (var theme in GetThemes())
			{
				theme.EnsureAddressable(Object2Address, true);
			}
		}

		/// <summary>
		/// Postprocess build event.
		/// </summary>
		/// <param name="report">Report.</param>
		public void OnPostprocessBuild(BuildReport report)
		{
			foreach (var theme in GetThemes())
			{
				theme.PreloadAddressable();
			}
		}

		AddressableAsset Object2Address(UnityEngine.Object obj)
		{
			var path = AssetDatabase.GetAssetPath(obj);
			if (string.IsNullOrEmpty(path))
			{
				return AddressableAsset.Fail;
			}

			var settings = AddressableAssetSettingsDefaultObject.Settings;
			var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(path));
			if (entry == null)
			{
				return AddressableAsset.Fail;
			}

			return new AddressableAsset(entry.address, entry.guid);
		}
	}
}
#endif