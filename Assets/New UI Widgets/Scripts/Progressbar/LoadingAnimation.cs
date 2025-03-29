namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Loading animation.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/misc/loading-animation.html")]
	[DataBindSupport]
	public class LoadingAnimation : MonoBehaviourInitiable, IUpdatable
	{
		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		public ProgressbarDeterminate Progressbar;

		/// <summary>
		/// Value min.
		/// </summary>
		[SerializeField]
		[Range(0, 360)]
		[DataBindField]
		public int ValueMin = 30;

		/// <summary>
		/// Value max.
		/// </summary>
		[SerializeField]
		[Range(1, 360)]
		[DataBindField]
		public int ValueMax = 330;

		/// <summary>
		/// Value speed.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public int ValueSpeed = 45;

		/// <summary>
		/// Rotate speed.
		/// </summary>
		[SerializeField]
		[DataBindField]
		public float RotateSpeed = -45;

		/// <summary>
		/// Value change rate.
		/// </summary>
		[NonSerialized]
		[DataBindField]
		protected int ValueRate = 100;

		float progress;

		int direction;

		RectTransform progressbarRect;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			progressbarRect = Progressbar.transform as RectTransform;

			Progressbar.Max = 360 * ValueRate;
			Progressbar.Value = ValueMin * ValueRate;

			progress = ValueMin * ValueRate;
			direction = 1;
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Init();
			Updater.Add(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Updater.Remove(this);
		}

		/// <inheritdoc/>
		public virtual void RunUpdate()
		{
			Rotate(Time.deltaTime);
			Progress(Time.deltaTime);
		}

		void Progress(float dt)
		{
			progress += ValueSpeed * ValueRate * dt * direction;
			if (progress >= (ValueMax * ValueRate))
			{
				direction = -1;
				progress = ValueMax * ValueRate;
			}
			else if (progress <= (ValueMin * ValueRate))
			{
				direction = 1;
				progress = ValueMin * ValueRate;
			}

			Progressbar.Value = Mathf.RoundToInt(progress);
		}

		void Rotate(float dt)
		{
			var angles = progressbarRect.localEulerAngles;
			angles.z += RotateSpeed * dt;

			progressbarRect.localEulerAngles = angles;
		}
	}
}