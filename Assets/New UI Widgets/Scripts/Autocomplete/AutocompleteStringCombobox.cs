namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Autocomplete as Combobox.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/autocomplete-combobox.html")]
	public class AutocompleteStringCombobox : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Invalid mode.
		/// </summary>
		public enum InvalidMode
		{
			/// <summary>
			/// Ignore invalid value.
			/// </summary>
			Ignore = 0,

			/// <summary>
			/// Focus on InputField.
			/// </summary>
			FocusInputField = 1,

			/// <summary>
			/// Reset InputField value.
			/// </summary>
			ResetInputField = 2,
		}

		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public AutocompleteString Autocomplete;

		/// <summary>
		/// Button to show all options.
		/// </summary>
		[SerializeField]
		public Button AutocompleteToggle;

		/// <summary>
		/// Return focus to InputField if input not found.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with IfInvalid.")]
		public bool FocusIfInvalid = false;

		/// <summary>
		/// What to do when InputField focus lost and value is invalid.
		/// </summary>
		[SerializeField]
		public InvalidMode IfInvalid = InvalidMode.Ignore;

		#pragma warning disable 0414
		[SerializeField]
		[HideInInspector]
		private int version = 0;
		#pragma warning restore

		/// <summary>
		/// Index of the selected option.
		/// </summary>
		public int Index => Autocomplete.DataSource.IndexOf(Autocomplete.InputFieldAdapter.text);

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Autocomplete.ResetListViewSelection = false;
			Autocomplete.Init();
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

			if (IfInvalid == InvalidMode.FocusInputField)
			{
				Autocomplete.InputFieldAdapter.Focus();
			}
			else if (IfInvalid == InvalidMode.ResetInputField)
			{
				Autocomplete.InputFieldAdapter.text = string.Empty;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected virtual void OnValidate()
		{
			if (version == 0)
			{
				#pragma warning disable 0618
				IfInvalid = FocusIfInvalid ? InvalidMode.FocusInputField : InvalidMode.Ignore;
				#pragma warning restore 0618

				version = 1;
			}
		}
#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (Autocomplete != null)
			{
				Autocomplete.SetStyle(style);
			}

			if (AutocompleteToggle != null)
			{
				style.Combobox.ToggleButton.ApplyTo(AutocompleteToggle.targetGraphic);
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

			if (AutocompleteToggle != null)
			{
				style.Combobox.ToggleButton.GetFrom(AutocompleteToggle.targetGraphic);
			}

			return true;
		}
		#endregion
	}
}