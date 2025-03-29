namespace UIThemes.Samples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Display current version number.
	/// </summary>
	public class AssetVersion : MonoBehaviour
	{
		/// <summary>
		/// File with version number.
		/// </summary>
		[SerializeField]
		public TextAsset VersionFile;

		/// <summary>
		/// Label to display version number.
		/// </summary>
		[SerializeField]
		public Text Label;

		#if UIWIDGETS_TMPRO_SUPPORT
		/// <summary>
		/// TMPro Label to display version number.
		/// </summary>
		[SerializeField]
		public TMPro.TextMeshProUGUI LabelTMPro;
		#endif

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			DisplayVersion();
		}

		/// <summary>
		/// Display current version number.
		/// </summary>
		public void DisplayVersion()
		{
			if (VersionFile == null)
			{
				return;
			}

			var version = string.Format("v{0}", VersionFile.text);
			if ((Label != null) && (Label.text != version))
			{
				Label.text = version;
			}

			#if UIWIDGETS_TMPRO_SUPPORT
			if ((LabelTMPro != null) && (LabelTMPro.text != version))
			{
				LabelTMPro.text = version;
			}
			#endif
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate() => DisplayVersion();
		#endif
	}
}