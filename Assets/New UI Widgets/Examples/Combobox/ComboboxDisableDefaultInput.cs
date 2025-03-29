namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Disable Combobox input.
	/// </summary>
	[RequireComponent(typeof(Combobox))]
	[System.Obsolete]
	public class ComboboxDisableDefaultInput : MonoBehaviour
	{
		/// <summary>
		/// Disable combobox input.
		/// </summary>
		protected virtual void Start()
		{
			// disable default combobox input, this also disable input field
			if (TryGetComponent<Combobox>(out var combobox))
			{
				combobox.Init();
				combobox.Editable = false;
			}

			// enable input field back
			if (TryGetComponent<InputFieldAdapter>(out var adapter))
			{
				adapter.interactable = true;
			}
			else if (TryGetComponent<InputField>(out var input))
			{
				input.interactable = true;
			}
		}
	}
}