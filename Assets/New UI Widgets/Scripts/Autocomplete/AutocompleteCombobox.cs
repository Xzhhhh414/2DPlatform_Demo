namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Autocomplete as Combobox.
	/// </summary>
	[System.Obsolete("Replaced with AutocompleteStringCombobox.")]
	public class AutocompleteCombobox : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public Autocomplete Autocomplete;

		/// <summary>
		/// Button to show all options.
		/// </summary>
		[SerializeField]
		public Button AutocompleteToggle;

		/// <summary>
		/// Return focus to InputField if input not found.
		/// </summary>
		[SerializeField]
		public bool FocusIfInvalid = false;

		/// <summary>
		/// Index of the selected option.
		/// </summary>
		public int Index => Autocomplete.DataSource.IndexOf(Autocomplete.Value);

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Autocomplete.InputFieldAdapter.onEndEdit.AddListener(Validate);
			AutocompleteToggle.onClick.AddListener(Autocomplete.ShowAllOptions);
		}

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Autocomplete.InputFieldAdapter.onEndEdit.RemoveListener(Validate);
			AutocompleteToggle.onClick.RemoveListener(Autocomplete.ShowAllOptions);
		}

		/// <summary>
		/// Validate input.
		/// </summary>
		/// <param name="value">Value.</param>
		protected void Validate(string value)
		{
			if (Autocomplete.DataSource.Contains(value))
			{
				return;
			}

			if (FocusIfInvalid)
			{
				Autocomplete.InputFieldAdapter.Focus();
			}
			else
			{
				Autocomplete.Value = string.Empty;
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (Autocomplete != null)
			{
				Autocomplete.SetStyle(style);
			}

			if ((AutocompleteToggle != null) && AutocompleteToggle.TryGetComponent<Image>(out var img))
			{
				style.Combobox.ToggleButton.ApplyTo(img);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (Autocomplete != null)
			{
				Autocomplete.GetStyle(style);
			}

			if ((AutocompleteToggle != null) && AutocompleteToggle.TryGetComponent<Image>(out var img))
			{
				style.Combobox.ToggleButton.GetFrom(img);
			}

			return true;
		}
		#endregion
	}
}