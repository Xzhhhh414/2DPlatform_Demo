namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// TreeView: editable Vector3.
	/// </summary>
	public class TreeViewInputVector3 : TreeViewComponent
	{
		/// <summary>
		/// Input.
		/// </summary>
		[SerializeField]
		public SpinnerVector3 InputVector3;

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			InputVector3.OnValueChanged.AddListener(InputChanged);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (InputVector3 != null)
			{
				InputVector3.OnValueChanged.RemoveListener(InputChanged);
			}
		}

		void InputChanged(Vector3 value) => Item.Tag = value;

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			base.UpdateView();

			InputVector3.Value = (Vector3)Item.Tag;
		}
	}
}