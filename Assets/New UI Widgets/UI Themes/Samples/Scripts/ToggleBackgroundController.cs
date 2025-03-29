namespace UIThemes.Samples
{
	using UIThemes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Toggle.Background controller.
	/// Change background color when the toggle value is changed.
	/// </summary>
	public class ToggleBackgroundController : MonoBehaviour, ITargetOwner
	{
		/// <summary>
		/// Toggle.
		/// </summary>
		public Toggle Toggle;

		/// <summary>
		/// Toggle background.
		/// </summary>
		public Image ToggleBackground;

		[SerializeField]
		Color colorOn = Color.white;

		/// <summary>
		/// Background color when Toggle is on.
		/// </summary>
		public Color ColorOn
		{
			get => colorOn;
			set
			{
				colorOn = value;
				UpdateColor(Toggle.isOn);
			}
		}

		[SerializeField]
		Color colorOff = Color.white;

		/// <summary>
		/// Background color when Toggle is off.
		/// </summary>
		public Color ColorOff
		{
			get => colorOff;
			set
			{
				colorOff = value;
				UpdateColor(Toggle.isOn);
			}
		}

		/// <summary>
		/// Set theme target owner.
		/// </summary>
		public virtual void SetTargetOwner() => Utilities.SetTargetOwner<Graphic>(typeof(Color), ToggleBackground, nameof(Image.color), this);

		void Start()
		{
			SetTargetOwner();
			Toggle.onValueChanged.AddListener(UpdateColor);
			UpdateColor(Toggle.isOn);
		}

		void OnDestroy() => Toggle.onValueChanged.RemoveListener(UpdateColor);

		void UpdateColor(bool isOn) => ToggleBackground.color = isOn ? colorOn : colorOff;
	}
}