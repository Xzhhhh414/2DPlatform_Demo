namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Base class for the SnapGrid.
	/// </summary>
	public abstract partial class SnapGridBase : UIBehaviourConditional
	{
		/// <summary>
		/// Line at Y axis.
		/// </summary>
		[Serializable]
		public struct LineY
		{
			[SerializeField]
			float y;

			/// <summary>
			/// Y.
			/// </summary>
			public readonly float Y => y;

			[SerializeField]
			bool snapBottom;

			/// <summary>
			/// Snap by target bottom side.
			/// </summary>
			public readonly bool SnapBottom => snapBottom;

			[SerializeField]
			bool snapTop;

			/// <summary>
			/// Snap by target top side.
			/// </summary>
			public readonly bool SnapTop => snapTop;

			/// <summary>
			/// Create instance.
			/// </summary>
			[DomainReloadExclude]
			public static readonly Func<float, bool, bool, LineY> Create = (y, bottom, top) => new LineY(y, bottom, top);

			/// <summary>
			/// Initializes a new instance of the <see cref="LineY"/> struct.
			/// </summary>
			/// <param name="y">Y.</param>
			/// <param name="snapBottom">Snap by target bottom side.</param>
			/// <param name="snapTop">Snap by target top side.</param>
			public LineY(float y, bool snapBottom = true, bool snapTop = true)
			{
				this.y = y;
				this.snapBottom = snapBottom;
				this.snapTop = snapTop;
			}
		}
	}
}