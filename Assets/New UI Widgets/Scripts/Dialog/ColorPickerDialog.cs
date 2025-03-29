namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// ColorPicker dialog.
	/// </summary>
	public class ColorPickerDialog : PickerOptionalOK<Color, ColorPickerDialog>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ColorPicker ColorPicker;

		/// <summary>
		/// Subscription on color changed.
		/// </summary>
		Subscription<Color32> colorChanged;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			colorChanged = new Subscription<Color32>(ColorPicker.OnChange, ValueChanged);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			colorChanged.Clear();

			base.OnDestroy();
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(Color defaultValue)
		{
			base.BeforeOpen(defaultValue);

			ColorPicker.Color =	defaultValue;
		}

		void ValueChanged(Color32 color)
		{
			if (Value == (Color)color)
			{
				return;
			}

			Value = color;

			if (Mode == PickerMode.CloseOnSelect)
			{
				Selected(Value);
			}
		}
	}
}