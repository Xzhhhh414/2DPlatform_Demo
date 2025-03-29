#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Progressbar with TextMeshPro support.
	/// </summary>
	[System.Obsolete("Use Progressbar with TextAdapter.")]
	public class ProgressbarTMPro : Progressbar
	{
		/// <summary>
		/// The empty bar text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with EmptyBarTextAdapter.")]
		public TextMeshProUGUI EmptyBarTextTMPro;

		/// <summary>
		/// The full bar text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with FullBarTextAdapter.")]
		public TextMeshProUGUI FullBarTextTMPro;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(EmptyBarTextTMPro, ref EmptyBarTextAdapter);
			Utilities.RequireComponent(FullBarTextTMPro, ref FullBarTextAdapter);
#pragma warning restore 0612, 0618
		}
	}
}
#endif