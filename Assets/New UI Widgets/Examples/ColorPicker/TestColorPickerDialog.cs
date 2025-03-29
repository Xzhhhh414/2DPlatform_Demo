namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test ColorPickerDialog.
	/// </summary>
	public class TestColorPickerDialog : MonoBehaviour
	{
		[SerializeField]
		ColorPickerDialog Dialog;

		[SerializeField]
		Image Image;

		/// <summary>
		/// Test.
		/// </summary>
		public async void Test()
		{
			var result = await Dialog.Clone().ShowAsync(Image.color);
			if (result.Success)
			{
				Image.color = result.Value;
				Debug.Log("value: " + result.Value);
			}
			else
			{
				Debug.Log("canceled");
			}
		}
	}
}