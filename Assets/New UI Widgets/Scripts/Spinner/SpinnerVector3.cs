namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Three Spinners combined to represent Vector3.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/spinner-vector3.html")]
	[DataBindSupport]
	public class SpinnerVector3 : MonoBehaviourConditional
	{
		/// <summary>
		/// Event.
		/// </summary>
		[Serializable]
		public class Vector3Event : UnityEvent<Vector3>
		{
		}

		[SerializeField]
		bool interactable = true;

		/// <summary>
		/// Interactable.
		/// </summary>
		[DataBindField]
		public bool Interactable
		{
			get => interactable;

			set
			{
				interactable = value;

				SpinnerX.Interactable = interactable;
				SpinnerY.Interactable = Interactable;
				SpinnerZ.Interactable = Interactable;
			}
		}

		/// <summary>
		/// Current value.
		/// </summary>
		[SerializeField]
		protected Vector3 CurrentValue;

		/// <summary>
		/// Value.
		/// </summary>
		[DataBindField]
		public Vector3 Value
		{
			get => new Vector3(SpinnerX.Value, SpinnerY.Value, SpinnerZ.Value);

			set
			{
				if (CurrentValue != value)
				{
					CurrentValue = value;
					SpinnerX.Value = value.x;
					SpinnerY.Value = value.y;
					SpinnerZ.Value = value.z;

					OnValueChanged.Invoke(CurrentValue);
				}
			}
		}

		/// <summary>
		/// Spinner for the Vector3.x;
		/// </summary>
		[SerializeField]
		protected SpinnerFloat SpinnerX;

		/// <summary>
		/// Spinner for the Vector3.y;
		/// </summary>
		[SerializeField]
		protected SpinnerFloat SpinnerY;

		/// <summary>
		/// Spinner for the Vector3.z;
		/// </summary>
		[SerializeField]
		protected SpinnerFloat SpinnerZ;

		/// <summary>
		/// Value changed event.
		/// </summary>
		[SerializeField]
		[DataBindEvent(nameof(Value))]
		public Vector3Event OnValueChanged = new Vector3Event();

		/// <summary>
		/// Subscription on spinner value changed.
		/// </summary>
		protected Subscriptions<Subscription<float>> SpinnersValueChanged;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			UnityAction<float> callback = ValueChanged;

			SpinnersValueChanged = new Subscriptions<Subscription<float>>(
				new Subscription<float>(SpinnerX.onValueChangeFloat, callback),
				new Subscription<float>(SpinnerY.onValueChangeFloat, callback),
				new Subscription<float>(SpinnerZ.onValueChangeFloat, callback));

			Interactable = interactable;
		}

		/// <summary>
		/// Process value changed event.
		/// </summary>
		/// <param name="ignore">Argument is ignored.</param>
		protected virtual void ValueChanged(float ignore = 0f)
		{
			if (CurrentValue == Value)
			{
				return;
			}

			CurrentValue = Value;
			OnValueChanged.Invoke(CurrentValue);
		}
	}
}