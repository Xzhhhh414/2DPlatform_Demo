namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Maximum size for the target of Splitter.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Splitter MaxSize")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/splitter.html")]
	public class SplitterMaxSize : MonoBehaviour
	{
		/// <summary>
		/// Maximum size.
		/// </summary>
		[SerializeField]
		public float Value;
	}
}