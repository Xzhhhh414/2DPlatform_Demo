namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// TreeViewInput: editable bool.
	/// </summary>
	public class TreeViewInputBool : TreeViewComponent
	{
		/// <summary>
		/// Input.
		/// </summary>
		[SerializeField]
		public Switch InputBool;

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			InputBool.OnValueChanged.AddListener(InputChanged);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (InputBool != null)
			{
				InputBool.OnValueChanged.RemoveListener(InputChanged);
			}
		}

		void InputChanged(bool value) => Item.Tag = value;

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			base.UpdateView();

			InputBool.IsOn = (bool)Item.Tag;
		}
	}
}