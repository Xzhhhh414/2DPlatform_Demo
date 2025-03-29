#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;

	/// <summary>
	/// Combobox with TMPro support.
	/// </summary>
	[System.Obsolete("Replaced with ComboboxString.")]
	public class ComboboxTMPro : Combobox
	{
		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
			if (TryGetComponent<TMP_InputField>(out var input))
			{
				Utilities.RequireComponent(input, ref InputAdapter);
			}
		}
	}
}
#endif