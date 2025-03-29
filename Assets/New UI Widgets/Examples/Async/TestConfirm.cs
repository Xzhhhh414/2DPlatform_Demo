namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test ConfirmExample.
	/// </summary>
	public class TestConfirm : MonoBehaviour
	{
		/// <summary>
		/// Confirm.
		/// </summary>
		[SerializeField]
		public ConfirmExample Confirm;

		/// <summary>
		/// Test.
		/// </summary>
		public async void Test()
		{
			var quit = await Confirm.Open("Quit?");
			if (quit)
			{
				Debug.Log("quit");
				Application.Quit();
			}
			else
			{
				Debug.Log("cancel");
			}
		}
	}
}