namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// TreeViewInput: editable integer.
	/// </summary>
	public class TreeViewInputInt : TreeViewComponent
	{
		/// <summary>
		/// Input.
		/// </summary>
		[SerializeField]
		public Spinner InputInt;

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			InputInt.onValueChangeInt.AddListener(InputChanged);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (InputInt != null)
			{
				InputInt.onValueChangeInt.RemoveListener(InputChanged);
			}
		}

		void InputChanged(int value) => Item.Tag = value;

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			base.UpdateView();

			InputInt.Value = (int)Item.Tag;
		}
	}
}