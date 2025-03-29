namespace UIWidgets.WidgetGeneration
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Picker generator helper.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/generator.html")]
	public class PickerGeneratorHelper : MonoBehaviour
	{
		/// <summary>
		/// Main object.
		/// </summary>
		public GameObject Main;

		/// <summary>
		/// Cancel button.
		/// </summary>
		public GameObject Title;

		/// <summary>
		/// Container.
		/// </summary>
		public Transform Content;

		/// <summary>
		/// Close button.
		/// </summary>
		public Button ButtonClose;

		/// <summary>
		/// OK button.
		/// </summary>
		public Button ButtonOK;

		/// <summary>
		/// OK button label.
		/// </summary>
		public RectTransform ButtonOKLabel;

		/// <summary>
		/// Cancel button.
		/// </summary>
		public Button ButtonCancel;

		/// <summary>
		/// Cancel button label.
		/// </summary>
		public RectTransform ButtonCancelLabel;
	}
}