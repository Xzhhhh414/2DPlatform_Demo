namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// TreeViewInput: editable string.
	/// </summary>
	public class TreeViewInputString : TreeViewComponent
	{
		/// <summary>
		/// Input.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter InputString;

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			InputString.onValueChanged.AddListener(InputChanged);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (InputString != null)
			{
				InputString.onValueChanged.RemoveListener(InputChanged);
			}
		}

		void InputChanged(string value) => Item.Tag = value;

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			base.UpdateView();

			InputString.Value = (string)Item.Tag;
		}
	}
}