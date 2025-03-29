namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Safe area.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/mobile/safe-area.html")]
	public class SafeArea : UIBehaviour, IUpdatable
	{
		readonly struct ScreenState
		{
			public readonly Rect SafeArea;

			public readonly Resolution Resolution;

			public readonly ScreenOrientation Orientation;

			private ScreenState(Rect safeArea, Resolution resolution, ScreenOrientation orientation)
			{
				SafeArea = safeArea;

				Resolution = resolution;
				Orientation = orientation;
			}

			public static ScreenState Current => new ScreenState(Screen.safeArea, Screen.currentResolution, Screen.orientation);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj) => (obj is ScreenState state) && Equals(state);

			bool SameResolution(Resolution resolution) => Resolution.width == resolution.width && Resolution.height == resolution.height;

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(ScreenState other) => SafeArea == other.SafeArea && SameResolution(other.Resolution) && Orientation == other.Orientation;

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode() => SafeArea.GetHashCode() ^ Resolution.width ^ Resolution.height ^ (int)Orientation;

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">Left instance.</param>
			/// <param name="b">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(ScreenState a, ScreenState b) => a.Equals(b);

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">Left instance.</param>
			/// <param name="b">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(ScreenState a, ScreenState b) => !a.Equals(b);
		}

		RectTransform rectTransform;

		Canvas canvas;

		ScreenState prevState;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			TryGetComponent(out rectTransform);
			canvas = GetComponentInParent<Canvas>();

			prevState = ScreenState.Current;
			ChangeScreenState();
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			Updater.Add(this);
			base.OnEnable();
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			Updater.Remove(this);
			base.OnDisable();
		}

		/// <summary>
		/// Run update.
		/// </summary>
		public void RunUpdate()
		{
			CheckScreenState();
		}

		/// <summary>
		/// Process the resize event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			CheckScreenState();
		}

		void CheckScreenState()
		{
			var current = ScreenState.Current;
			if (current != prevState && canvas != null)
			{
				prevState = current;
				Updater.RunOnceNextFrame(this, ChangeScreenState);
			}
		}

		void ChangeScreenState()
		{
			var scale = canvas.scaleFactor;
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Mathf.Round(prevState.SafeArea.xMin / scale), Mathf.Round(prevState.SafeArea.width / scale));
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, Mathf.Round(prevState.SafeArea.yMin / scale), Mathf.Round(prevState.SafeArea.height / scale));
		}
	}
}