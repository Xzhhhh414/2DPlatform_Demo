namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeViewInput: editable color.
	/// </summary>
	public class TreeViewInputColor : TreeViewComponent
	{
		/// <summary>
		/// Input.
		/// </summary>
		[SerializeField]
		public ColorPickerDialog InputColor;

		/// <summary>
		/// View color.
		/// </summary>
		[SerializeField]
		public Image ViewColor;

		/// <summary>
		/// View color transparency.
		/// </summary>
		[SerializeField]
		public Image ViewColorTransparency;

		/// <summary>
		/// Button to open dialog.
		/// </summary>
		[SerializeField]
		public Button BtnOpenDialog;

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			BtnOpenDialog.onClick.AddListener(SelectColor);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (BtnOpenDialog != null)
			{
				BtnOpenDialog.onClick.RemoveListener(SelectColor);
			}
		}

		async void SelectColor()
		{
			var result = await InputColor.Clone().ShowAsync((Color)Item.Tag);
			if (result.Success)
			{
				Item.Tag = result.Value;
			}
		}

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			base.UpdateView();

			var color = (Color)Item.Tag;

			if (ViewColorTransparency != null)
			{
				ViewColor.color = new Color(color.r, color.g, color.b);
				ViewColorTransparency.fillAmount = color.a;
			}
			else
			{
				ViewColor.color = color;
			}
		}
	}
}