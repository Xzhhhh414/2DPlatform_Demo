namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test BorderEffect.
	/// </summary>
	public class TestBorderEffect : MonoBehaviour
	{
		/// <summary>
		/// BorderDrawer.
		/// </summary>
		[SerializeField]
		public BorderEffect BorderEffect;

		/// <summary>
		/// Spinner for the left border.
		/// </summary>
		[SerializeField]
		public Spinner SpinnerLeft;

		/// <summary>
		/// Spinner for the right border.
		/// </summary>
		[SerializeField]
		public Spinner SpinnerRight;

		/// <summary>
		/// Spinner for the top border.
		/// </summary>
		[SerializeField]
		public Spinner SpinnerTop;

		/// <summary>
		/// Spinner for the bottom border.
		/// </summary>
		[SerializeField]
		public Spinner SpinnerBottom;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			if (SpinnerLeft != null)
			{
				SpinnerLeft.Min = 0;
				SpinnerLeft.Max = 100;
				SpinnerLeft.Value = Mathf.RoundToInt(BorderEffect.HorizontalBorders.x);
				SpinnerLeft.onValueChangeInt.AddListener(ChangeLeftBorder);
			}

			if (SpinnerRight != null)
			{
				SpinnerRight.Min = 0;
				SpinnerRight.Max = 100;
				SpinnerRight.Value = Mathf.RoundToInt(BorderEffect.HorizontalBorders.y);
				SpinnerRight.onValueChangeInt.AddListener(ChangeRightBorder);
			}

			if (SpinnerTop != null)
			{
				SpinnerTop.Min = 0;
				SpinnerTop.Max = 100;
				SpinnerTop.Value = Mathf.RoundToInt(BorderEffect.VerticalBorders.x);
				SpinnerTop.onValueChangeInt.AddListener(ChangeTopBorder);
			}

			if (SpinnerBottom != null)
			{
				SpinnerBottom.Min = 0;
				SpinnerBottom.Max = 100;
				SpinnerBottom.Value = Mathf.RoundToInt(BorderEffect.VerticalBorders.y);
				SpinnerBottom.onValueChangeInt.AddListener(ChangeBottomBorder);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			if (SpinnerLeft != null)
			{
				SpinnerLeft.onValueChangeInt.RemoveListener(ChangeLeftBorder);
			}

			if (SpinnerRight != null)
			{
				SpinnerRight.onValueChangeInt.RemoveListener(ChangeRightBorder);
			}

			if (SpinnerTop != null)
			{
				SpinnerTop.onValueChangeInt.RemoveListener(ChangeTopBorder);
			}

			if (SpinnerBottom != null)
			{
				SpinnerBottom.onValueChangeInt.RemoveListener(ChangeBottomBorder);
			}
		}

		void ChangeLeftBorder(int border)
		{
			BorderEffect.HorizontalBorders = new Vector2(border, BorderEffect.HorizontalBorders.y);
		}

		void ChangeRightBorder(int border)
		{
			BorderEffect.HorizontalBorders = new Vector2(BorderEffect.HorizontalBorders.x, border);
		}

		void ChangeTopBorder(int border)
		{
			BorderEffect.VerticalBorders = new Vector2(border, BorderEffect.VerticalBorders.y);
		}

		void ChangeBottomBorder(int border)
		{
			BorderEffect.VerticalBorders = new Vector2(BorderEffect.VerticalBorders.x, border);
		}
	}
}