namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Distance line.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/distance-lines.html")]
	public class DistanceLine : MonoBehaviour
	{
		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform => transform as RectTransform;

		/// <summary>
		/// Label to display distance.
		/// </summary>
		[SerializeField]
		public TextAdapter Label;

		/// <summary>
		/// Format.
		/// </summary>
		[SerializeField]
		public string Format = "0.0";

		/// <summary>
		/// Show distance.
		/// </summary>
		/// <param name="distance">Distance.</param>
		public virtual void ShowDistance(float distance)
		{
			if (Label != null)
			{
				Label.text = distance.ToString(Format);
			}
		}
	}
}