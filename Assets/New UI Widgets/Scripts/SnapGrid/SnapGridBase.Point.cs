namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Base class for the SnapGrid.
	/// </summary>
	public abstract partial class SnapGridBase : UIBehaviourConditional
	{
		/// <summary>
		/// Point.
		/// </summary>
		public struct Point
		{
			/// <summary>
			/// X.
			/// </summary>
			public float X;

			/// <summary>
			/// Y.
			/// </summary>
			public float Y;

			/// <summary>
			/// Point on the left side.
			/// </summary>
			public bool Left;

			/// <summary>
			/// Point on the right side.
			/// </summary>
			public bool Right;

			/// <summary>
			/// Point on the top side.
			/// </summary>
			public bool Top;

			/// <summary>
			/// Point on the bottom side.
			/// </summary>
			public bool Bottom;

			/// <summary>
			/// Initializes a new instance of the <see cref="Point"/> struct.
			/// </summary>
			/// <param name="x">X.</param>
			/// <param name="y">Y.</param>
			/// <param name="left">Point on the left side.</param>
			/// <param name="top">Point on the top side.</param>
			public Point(float x, float y, bool left, bool top)
			{
				X = x;
				Y = y;

				Left = left;
				Top = top;
				Right = !left;
				Bottom = !top;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Point"/> struct.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="snapGrid">Snap grid.</param>
			/// <param name="left">Point on the left side.</param>
			/// <param name="top">Point on the top side.</param>
			public Point(RectTransform target, SnapGridBase snapGrid, bool left, bool top)
				: this(target, snapGrid.RectTransform, left, top)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Point"/> struct.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="snapTarget">Snap target.</param>
			/// <param name="left">Point on the left side.</param>
			/// <param name="top">Point on the top side.</param>
			public Point(RectTransform target, RectTransform snapTarget, bool left, bool top)
			{
				var angle = 360f - (target.rotation.eulerAngles.z - snapTarget.rotation.eulerAngles.z);
				if (angle >= 360f)
				{
					angle -= 360f;
				}
				else if (angle <= -360f)
				{
					angle += 360f;
				}

				var angle_rad = angle * Mathf.Deg2Rad;

				var size = target.rect.size;
				var target_scale = target.lossyScale;
				var grid_scale = snapTarget.lossyScale;
				size.x *= target_scale.x / grid_scale.x;
				size.y *= target_scale.y / grid_scale.y;

				var sin = Mathf.Sin(angle_rad);
				var cos = Mathf.Cos(angle_rad);

				var pivot = new Vector2(left ? 0f : 1f, top ? 1f : 0f) - target.pivot;
				var dx = (size.x * pivot.x * cos) + (size.y * pivot.y * sin);
				var dy = (size.y * pivot.y * cos) - (size.x * pivot.x * sin);

				var position = target.position / UtilitiesUI.GetCanvasScale(target);

				X = position.x + dx;
				Y = position.y + dy;

				Left = left;
				Top = top;
				Right = !left;
				Bottom = !top;
			}

			/// <summary>
			/// Converts this instance to the string.
			/// </summary>
			/// <returns>String.</returns>
			public override string ToString()
			{
				var args = new object[] { X.ToString(), Y.ToString(), Left.ToString(), Top.ToString(), };
				return string.Format("SnapGridBase.Point(X: {0}; Y: {1}; left: {2}; top: {3})", args);
			}
		}
	}
}