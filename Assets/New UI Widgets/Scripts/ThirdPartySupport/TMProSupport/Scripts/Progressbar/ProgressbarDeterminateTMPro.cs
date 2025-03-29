#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// ProgressbarDeterminateTMPro.
	/// </summary>
	[System.Obsolete("Use ProgressbarDeterminate with TextAdapter.")]
	public class ProgressbarDeterminateTMPro : ProgressbarDeterminateBase, IUpgradeable
	{
		/// <summary>
		/// The empty bar text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with EmptyBarTextAdapter.")]
		public TextMeshProUGUI EmptyBarText;

		/// <summary>
		/// The full bar text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with FullBarTextAdapter.")]
		public TextMeshProUGUI FullBarText;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(EmptyBarText, ref EmptyBarTextAdapter);
			Utilities.RequireComponent(FullBarText, ref FullBarTextAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			Compatibility.Upgrade(this);
		}
#endif
	}
}
#endif