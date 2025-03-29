namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test RoundedCorners.
	/// </summary>
	public class TestRoundedCornersX4 : MonoBehaviour
	{
		/// <summary>
		/// Rounded corners.
		/// </summary>
		[SerializeField]
		public RoundedCornersX4 RoundedCorners;

		/// <summary>
		/// Spinner for the top left corner.
		/// </summary>
		[SerializeField]
		public Spinner TopLeft;

		/// <summary>
		/// Spinner for the top right corner.
		/// </summary>
		[SerializeField]
		public Spinner TopRight;

		/// <summary>
		/// Spinner for the the bottom top corner.
		/// </summary>
		[SerializeField]
		public Spinner BottomLeft;

		/// <summary>
		/// Spinner for the bottom right corner.
		/// </summary>
		[SerializeField]
		public Spinner BottomRight;

		/// <summary>
		/// Spinner for the border.
		/// </summary>
		[SerializeField]
		public Spinner Border;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			if (TopLeft != null)
			{
				TopLeft.Min = 0;
				TopLeft.Max = Mathf.RoundToInt(RoundedCorners.MaxRadius());
				TopLeft.Value = Mathf.RoundToInt(RoundedCorners.Radius.TopLeft);
				TopLeft.onValueChangeInt.AddListener(UpdateRadius);
			}

			if (TopRight != null)
			{
				TopRight.Min = 0;
				TopRight.Max = Mathf.RoundToInt(RoundedCorners.MaxRadius());
				TopRight.Value = Mathf.RoundToInt(RoundedCorners.Radius.TopRight);
				TopRight.onValueChangeInt.AddListener(UpdateRadius);
			}

			if (BottomLeft != null)
			{
				BottomLeft.Min = 0;
				BottomLeft.Max = Mathf.RoundToInt(RoundedCorners.MaxRadius());
				BottomLeft.Value = Mathf.RoundToInt(RoundedCorners.Radius.BottomLeft);
				BottomLeft.onValueChangeInt.AddListener(UpdateRadius);
			}

			if (BottomRight != null)
			{
				BottomRight.Min = 0;
				BottomRight.Max = Mathf.RoundToInt(RoundedCorners.MaxRadius());
				BottomRight.Value = Mathf.RoundToInt(RoundedCorners.Radius.BottomRight);
				BottomRight.onValueChangeInt.AddListener(UpdateRadius);
			}

			if (Border != null)
			{
				Border.Min = 0;
				Border.Max = Mathf.FloorToInt(RoundedCorners.Radius.Max);
				Border.Value = Mathf.RoundToInt(RoundedCorners.BorderWidth);
				Border.onValueChangeInt.AddListener(UpdateBorder);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			if (TopLeft != null)
			{
				TopLeft.onValueChangeInt.RemoveListener(UpdateRadius);
			}

			if (TopRight != null)
			{
				TopRight.onValueChangeInt.RemoveListener(UpdateRadius);
			}

			if (BottomLeft != null)
			{
				BottomLeft.onValueChangeInt.RemoveListener(UpdateRadius);
			}

			if (BottomRight != null)
			{
				BottomRight.onValueChangeInt.RemoveListener(UpdateRadius);
			}

			if (Border != null)
			{
				Border.onValueChangeInt.RemoveListener(UpdateBorder);
			}
		}

		void UpdateRadius(int ignore)
		{
			RoundedCorners.Radius = new RoundedCornersX4.BorderRadius(
				TopLeft.Value,
				TopRight.Value,
				BottomLeft.Value,
				BottomRight.Value);
			Border.Max = Mathf.FloorToInt(RoundedCorners.Radius.Max);
		}

		void UpdateBorder(int border) => RoundedCorners.BorderWidth = border;
	}
}