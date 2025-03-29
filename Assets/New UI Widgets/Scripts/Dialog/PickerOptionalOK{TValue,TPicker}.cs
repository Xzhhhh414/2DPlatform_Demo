namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base custom class for Pickers with optional OK button.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	/// <typeparam name="TPicker">Type of picker.</typeparam>
	public class PickerOptionalOK<TValue, TPicker> : Picker<TValue, TPicker>
		where TPicker : Picker<TValue, TPicker>
	{
		/// <summary>
		/// OK button.
		/// </summary>
		[SerializeField]
		public PickerMode Mode = PickerMode.CloseOnSelect;

		/// <summary>
		/// OK button.
		/// </summary>
		[SerializeField]
		[EditorConditionEnum(nameof(Mode), (int)PickerMode.CloseOnOK)]
		public Button OkButton;

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			if (OkButton != null)
			{
				OkButton.onClick.AddListener(OkCallback);
			}
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			if (OkButton != null)
			{
				OkButton.onClick.RemoveListener(OkCallback);
			}
		}

		/// <inheritdoc/>
		public override void BeforeOpen(TValue defaultValue)
		{
			base.BeforeOpen(defaultValue);

			if (OkButton != null)
			{
				OkButton.gameObject.SetActive(Mode == PickerMode.CloseOnOK);
			}
		}

		/// <summary>
		/// OK button callback.
		/// </summary>
		protected virtual void OkCallback()
		{
			Selected(Value);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Ok"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			style.Dialog.Button.GetFrom(transform.Find("Buttons/Ok"));
			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}