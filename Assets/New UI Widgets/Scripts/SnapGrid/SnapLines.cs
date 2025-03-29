namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Snap lines.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/snapgrid/snaplines.html")]
	public class SnapLines : SnapGridBase
	{
		[SerializeField]
		List<LineX> linesX = new List<LineX>();

		ObservableList<LineX> xAxisLines;

		/// <summary>
		/// Lines at X axis.
		/// </summary>
		[DataBindField]
		public ObservableList<LineX> XAxisLines
		{
			get
			{
				if (xAxisLines == null)
				{
					xAxisLines = new ObservableList<LineX>(linesX);
					xAxisLines.OnChangeMono.AddListener(UpdateLines);
				}

				return xAxisLines;
			}

			set
			{
				xAxisLines?.OnChangeMono.RemoveListener(UpdateLines);

				xAxisLines = value;

				if (xAxisLines != null)
				{
					xAxisLines.OnChangeMono.AddListener(UpdateLines);

					UpdateLines();
				}
			}
		}

		[SerializeField]
		List<LineY> linesY = new List<LineY>();

		ObservableList<LineY> yAxisLines;

		/// <summary>
		/// Lines at Y axis.
		/// </summary>
		[DataBindField]
		public ObservableList<LineY> YAxisLines
		{
			get
			{
				if (yAxisLines == null)
				{
					yAxisLines = new ObservableList<LineY>(linesY);
					yAxisLines.OnChangeMono.AddListener(UpdateLines);
				}

				return yAxisLines;
			}

			set
			{
				yAxisLines?.OnChangeMono.RemoveListener(UpdateLines);

				yAxisLines = value;

				if (yAxisLines != null)
				{
					yAxisLines.OnChangeMono.AddListener(UpdateLines);

					UpdateLines();
				}
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			XAxisLines = null;
			YAxisLines = null;
		}

		/// <inheritdoc/>
		protected override void UpdateLines()
		{
			LinesX.Clear();
			LinesX.AddRange(XAxisLines);

			LinesY.Clear();
			LinesY.AddRange(YAxisLines);

			OnLinesChanged.Invoke();
		}

		#if UNITY_EDITOR

		/// <inheritdoc/>
		protected override void OnValidate()
		{
			XAxisLines = null;
			YAxisLines = null;

			base.OnValidate();
		}

		#endif
	}
}