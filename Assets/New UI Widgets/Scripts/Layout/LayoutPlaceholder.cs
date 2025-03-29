namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Layout placeholder.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/layout/layout-placeholder.html")]
	public class LayoutPlaceholder : MonoBehaviour
	{
		/// <summary>
		/// Source.
		/// </summary>
		[NonSerialized]
		protected RectTransform Source;

		/// <summary>
		/// Layout Element.
		/// </summary>
		protected LayoutElement LayoutElement;

		/// <summary>
		/// RectTransform.
		/// </summary>
		protected RectTransform RectTransform;

		/// <summary>
		/// Create layout placeholder.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <returns>Layout placeholder.</returns>
		public static LayoutPlaceholder Create(RectTransform source)
		{
			var go = new GameObject(source.name + " Placeholder");
			var placeholder = go.AddComponent<LayoutPlaceholder>();

			placeholder.Source = source;
			placeholder.RectTransform = Utilities.RequireComponent<RectTransform>(go);
			placeholder.LayoutElement = go.AddComponent<LayoutElement>();

			placeholder.Hide();

			return placeholder;
		}

		/// <summary>
		/// Show.
		/// </summary>
		public void Show()
		{
			RectTransform.SetParent(Source.parent);
			RectTransform.SetSiblingIndex(Source.GetSiblingIndex());

			var size = Source.rect.size;
			RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			RectTransform.localScale = Source.localScale;

			LayoutElement.gameObject.SetActive(true);
			LayoutElement.ignoreLayout = false;

			var info = new LayoutElementData(Source);
			info.SetTo(LayoutElement);
		}

		/// <summary>
		/// Hide.
		/// </summary>
		public void Hide()
		{
			LayoutElement.gameObject.SetActive(false);
			LayoutElement.ignoreLayout = true;
		}
	}
}