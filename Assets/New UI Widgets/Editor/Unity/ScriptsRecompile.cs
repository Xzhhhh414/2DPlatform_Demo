#if UNITY_EDITOR
namespace UIWidgets
{
	using System.IO;
	using UnityEngine;

	/// <summary>
	/// Forced recompilation if compilation was not done after Scripting Define Symbols was changed.
	/// </summary>
	public static class ScriptsRecompile
	{
		/// <summary>
		/// Text label for initial state.
		/// </summary>
		public const string StatusInitial = "initial";

		/// <summary>
		/// Text label for state after symbols added.
		/// </summary>
		public const string StatusSymbolsAdded = "symbols added";

		/// <summary>
		/// Text label for recompilation started state.
		/// </summary>
		public const string StatusRecompiledAdded = "recompiled label added";

		/// <summary>
		/// Text label for recompilation labels removed state.
		/// </summary>
		public const string StatusRecompileRemoved = "recompiled label removed";

		/// <summary>
		/// Check if forced recompilation required.
		/// </summary>
		[UnityEditor.Callbacks.DidReloadScripts]
		public static void Run()
		{
#if UIWIDGETS_TMPRO_SUPPORT
			Check(ReferenceGUID.TMProStatus, ReferenceGUID.TMProSupport);
#endif

#if UIWIDGETS_DATABIND_SUPPORT
			Check(ReferenceGUID.DataBindStatus, ReferenceGUID.DataBindSupport);
#endif

#if UIWIDGETS_I2LOCALIZATION_SUPPORT
			Check(ReferenceGUID.I2LocalizationStatus, ReferenceGUID.I2LocalizationSupport);
#endif
		}

		static void Check(string path, string[] guids)
		{
			var status = GetStatus(path);

			switch (status)
			{
				case StatusInitial:
					break;
				case StatusSymbolsAdded:
					Compatibility.ForceRecompileByGUID(guids);

					SetStatus(path, StatusRecompiledAdded);
					break;
				case StatusRecompiledAdded:
					Compatibility.RemoveForceRecompileByGUID(guids);

					SetStatus(path, StatusRecompileRemoved);
					break;
				case StatusRecompileRemoved:
					SetStatus(path, StatusInitial);
					break;
				default:
					Debug.LogWarning("Unknown recompile status: " + status);
					break;
			}
		}

		/// <summary>
		/// Get forced recompilation status from file with label.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <returns>Status.</returns>
		public static string GetStatus(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return StatusInitial;
			}

			return File.ReadAllText(path);
		}

		/// <summary>
		/// Set forced recompilation status to file with label.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="status">Status.</param>
		public static void SetStatus(string path, string status)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			File.WriteAllText(path, status);
		}
	}
}
#endif