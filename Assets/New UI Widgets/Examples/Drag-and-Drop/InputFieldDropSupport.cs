namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drop support for the InputField.
	/// </summary>
	[RequireComponent(typeof(InputFieldAdapter))]
	public class InputFieldDropSupport : MonoBehaviour, IDropSupport<TreeNode<TreeViewItem>>, IDropSupport<string>
	{
		/// <summary>
		/// InputField.text value before drop.
		/// Can be used to swap content with drag source.
		/// </summary>
		public string OriginalData;

		string GetData()
		{
			return TryGetComponent<InputFieldAdapter>(out var adapter) ? adapter.text : string.Empty;
		}

		void SetData(string data)
		{
			if (TryGetComponent<InputFieldAdapter>(out var adapter))
			{
				adapter.text = data;
			}
		}

		#region IDropSupport<string>

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(string data, PointerEventData eventData)
		{
			OriginalData = GetData();
			SetData(data);
		}

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(string data, PointerEventData eventData) => true;

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(string data, PointerEventData eventData)
		{
		}

		#endregion

		#region IDropSupport<TreeNode<TreeViewItem>>

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			OriginalData = GetData();
			SetData(data.Item.Name);
		}

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			return data.Item.Name.EndsWith("1", System.StringComparison.InvariantCulture);
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
		}

		#endregion
	}
}