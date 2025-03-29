namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewSlider component.
	/// </summary>
	public class ListViewSliderComponent : ListViewItem, IViewData<ListViewSliderItem>
	{
		/// <summary>
		/// Slider.
		/// </summary>
		public Slider Slider;

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				#pragma warning disable 0618
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				#pragma warning restore
				GraphicsBackgroundVersion = 1;
			}

			base.GraphicsBackgroundInit();
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewSliderItem item)
		{
			Slider.value = item.Value;
		}

		/// <summary>
		/// Handle OnMove event.
		/// Redirect left and right movements events to slider.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
				case MoveDirection.Right:
					Slider.OnMove(eventData);
					break;
				default:
					base.OnMove(eventData);
					break;
			}
		}
	}
}