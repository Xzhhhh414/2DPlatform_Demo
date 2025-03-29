namespace UIWidgets.Examples
{
	using UIWidgets.Examples;
	using UnityEngine;

	/// <summary>
	/// LVInputFields drag support.
	/// </summary>
	[RequireComponent(typeof(LVInputFieldsComponent))]
	public class LVInputFieldsDragSupport : ListViewCustomDragSupport<LVInputFields, LVInputFieldsComponent, LVInputFieldsItem>
	{
		/// <inheritdoc/>
		protected override LVInputFieldsItem GetData(LVInputFieldsComponent component) => component.Item;

		/// <inheritdoc/>
		protected override void SetDragInfoData(LVInputFieldsItem data) => DragInfo.SetData(data);
	}
}