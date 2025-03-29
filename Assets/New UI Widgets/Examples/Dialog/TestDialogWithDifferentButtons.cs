namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test dialog with different buttons.
	/// </summary>
	public class TestDialogWithDifferentButtons : MonoBehaviour
	{
		/// <summary>
		/// Dialog template.
		/// </summary>
		[SerializeField]
		public Dialog DialogTemplate;

		/// <summary>
		/// Show dialog.
		/// </summary>
		public async void ShowDialog()
		{
			var actions = new DialogButton[]
			{
				new DialogButton("Cancel Button", 0),
				new DialogButton("Main Button", 1),
			};

			var dialog = DialogTemplate.Clone();
			var button_index = await dialog.ShowAsync(
				title: "Dialog With Different Buttons",
				message: "Test",
				buttons: actions,
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
			Debug.Log(string.Format("clicked: {0}", button_index.ToString()));
		}
	}
}