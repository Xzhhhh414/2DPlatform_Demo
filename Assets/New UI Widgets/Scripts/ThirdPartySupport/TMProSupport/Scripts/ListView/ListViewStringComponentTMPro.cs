#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// List view item component.
	/// </summary>
	[System.Obsolete("Use ListViewStringComponent with TextAdapter.")]
	public class ListViewStringComponentTMPro : ListViewStringComponent
	{
		/// <summary>
		/// The Text component.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapter.")]
		public TextMeshProUGUI TextTMPro;

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

#pragma warning disable 0618
			Utilities.RequireComponent(TextTMPro, ref TextAdapter);
#pragma warning restore 0618
		}
	}
}
#endif