namespace UIWidgets
{
	using System.Collections;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Switch direction.
	/// </summary>
	public enum SwitchDirection
	{
		/// <summary>
		/// Left to right.
		/// </summary>
		LeftToRight = 0,

		/// <summary>
		/// Right to left.
		/// </summary>
		RightToLeft = 1,

		/// <summary>
		/// Bottom to top.
		/// </summary>
		BottomToTop = 2,

		/// <summary>
		/// Top to bottom.
		/// </summary>
		TopToBottom = 3,
	}

	/// <summary>
	/// Switch.
	/// </summary>
	[DataBindSupport]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/switch.html")]
	public class Switch : SelectableInitiable, ISubmitHandler, IPointerClickHandler, IStylable, IValidateable
	{
		/// <summary>
		/// Is on?
		/// </summary>
		[SerializeField]
		protected bool isOn;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is on.
		/// </summary>
		/// <value><c>true</c> if this instance is on; otherwise, <c>false</c>.</value>
		[DataBindField]
		public virtual bool IsOn
		{
			get => isOn;

			set
			{
				if (isOn != value)
				{
					isOn = value;
					if (Group != null)
					{
						if (isOn || (!Group.AnySwitchesOn() && !Group.AllowSwitchOff))
						{
							isOn = true;
							Group.NotifySwitchOn(this);
						}
					}

					Changed();
				}
			}
		}

		/// <summary>
		/// Switch group.
		/// </summary>
		[SerializeField]
		protected SwitchGroup Group;

		/// <summary>
		/// Gets or sets the switch group.
		/// </summary>
		/// <value>The switch group.</value>
		public virtual SwitchGroup SwitchGroup
		{
			get => Group;

			set
			{
				if (Group != null)
				{
					Group.UnregisterSwitch(this);
				}

				Group = value;

				if (Group != null)
				{
					Group.RegisterSwitch(this);
					if (IsOn)
					{
						Group.NotifySwitchOn(this);
					}
				}
			}
		}

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		protected SwitchDirection direction = SwitchDirection.LeftToRight;

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public SwitchDirection Direction
		{
			get => direction;

			set
			{
				direction = value;
				SetMarkPosition(false);
			}
		}

		/// <summary>
		/// The mark.
		/// </summary>
		[SerializeField]
		public RectTransform Mark;

		/// <summary>
		/// The mark.
		/// </summary>
		[SerializeField]
		public Graphic MarkGraphic;

		/// <summary>
		/// The background.
		/// </summary>
		[SerializeField]
		public Graphic Background;

		[SerializeField]
		Color markOnColor = new Color(1f, 1f, 1f, 1f);

		/// <summary>
		/// Gets or sets the color of the mark for On state.
		/// </summary>
		/// <value>The color of the mark for On state.</value>
		public Color MarkOnColor
		{
			get => markOnColor;

			set
			{
				markOnColor = value;
				SetMarkColor();
			}
		}

		[SerializeField]
		Color markOffColor = new Color(1f, 215f / 255f, 115f / 255f, 1f);

		/// <summary>
		/// Gets or sets the color of the mark for Off State.
		/// </summary>
		/// <value>The color of the mark for Off State.</value>
		public Color MarkOffColor
		{
			get => markOffColor;

			set
			{
				markOffColor = value;
				SetMarkColor();
			}
		}

		[SerializeField]
		Color backgroundOnColor = new Color(1f, 1f, 1f, 1f);

		/// <summary>
		/// Gets or sets the color of the background for On state.
		/// </summary>
		/// <value>The color of the background on.</value>
		public Color BackgroundOnColor
		{
			get => backgroundOnColor;

			set
			{
				backgroundOnColor = value;
				SetBackgroundColor();
			}
		}

		[SerializeField]
		Color backgroundOffColor = new Color(1f, 215f / 255f, 115f / 255f, 1f);

		/// <summary>
		/// Gets or sets the color of the background for Off State.
		/// </summary>
		/// <value>The color of the background off.</value>
		public Color BackgroundOffColor
		{
			get => backgroundOffColor;

			set
			{
				backgroundOffColor = value;
				SetBackgroundColor();
			}
		}

		/// <summary>
		/// The duration of the animation.
		/// </summary>
		[SerializeField]
		public float AnimationDuration = 0.3f;

		/// <summary>
		/// Animation curve.
		/// </summary>
		[SerializeField]
		public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime;

		/// <summary>
		/// Callback executed when the IsOn of the switch is changed.
		/// </summary>
		[SerializeField]
		[DataBindEvent(nameof(IsOn))]
		public SwitchEvent OnValueChanged = new SwitchEvent();

		/// <summary>
		/// Set IsOn without OnValueChanged invoke. Not recommended for use.
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		/// <param name="animate">Change state with animation.</param>
		public void SetStatus(bool value, bool animate = true)
		{
			if (isOn == value)
			{
				return;
			}

			isOn = value;

			if (Group != null)
			{
				if (isOn || (!Group.AnySwitchesOn() && !Group.AllowSwitchOff))
				{
					isOn = true;
					Group.NotifySwitchOn(this, false, animate);
				}
			}

			SetMarkPosition(animate);

			SetBackgroundColor();
			SetMarkColor();
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			SwitchGroup = Group;
		}

		/// <summary>
		/// Changed this instance.
		/// </summary>
		protected virtual void Changed()
		{
			SetMarkPosition();

			SetBackgroundColor();
			SetMarkColor();

			OnValueChanged.Invoke(IsOn);
		}

		/// <summary>
		/// The current coroutine.
		/// </summary>
		protected IEnumerator CurrentCoroutine;

		/// <summary>
		/// Sets the mark position.
		/// </summary>
		/// <param name="animate">If set to <c>true</c> animate.</param>
		protected virtual void SetMarkPosition(bool animate = true)
		{
			if (CurrentCoroutine != null)
			{
				StopCoroutine(CurrentCoroutine);
				CurrentCoroutine = null;
			}

			if (animate && gameObject.activeInHierarchy)
			{
				CurrentCoroutine = AnimateSwitch(IsOn, AnimationDuration);
				StartCoroutine(CurrentCoroutine);
			}
			else
			{
				SetMarkPosition(GetPosition(IsOn));
			}
		}

		/// <summary>
		/// Animates the switch.
		/// </summary>
		/// <returns>The switch.</returns>
		/// <param name="state">IsOn.</param>
		/// <param name="duration">Time.</param>
		protected virtual IEnumerator AnimateSwitch(bool state, float duration)
		{
			var prev_position = GetPosition(!IsOn);
			var next_position = GetPosition(IsOn);
			var time = 0f;
			var rate = AnimationCurve[AnimationCurve.length - 1].time / duration;

			SetMarkPosition(prev_position);

			do
			{
				var pos = Mathf.Lerp(prev_position, next_position, AnimationCurve.Evaluate(time * rate));

				SetMarkPosition(pos);

				yield return null;

				time += UtilitiesTime.GetDeltaTime(UnscaledTime);
			}
			while (time < duration);

			SetMarkPosition(next_position);
		}

		/// <summary>
		/// Get time.
		/// </summary>
		/// <returns>Time.</returns>
		[System.Obsolete("Use Utilities.GetTime(UnscaledTime).")]
		protected float GetTime()
		{
			return Utilities.GetTime(UnscaledTime);
		}

		/// <summary>
		/// Sets the mark position.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void SetMarkPosition(float position)
		{
			if (Mark == null)
			{
				return;
			}

			if (IsHorizontal())
			{
				Mark.anchorMin = new Vector2(position, Mark.anchorMin.y);
				Mark.anchorMax = new Vector2(position, Mark.anchorMax.y);
				Mark.pivot = new Vector2(position, Mark.pivot.y);
			}
			else
			{
				Mark.anchorMin = new Vector2(Mark.anchorMin.x, position);
				Mark.anchorMax = new Vector2(Mark.anchorMax.x, position);
				Mark.pivot = new Vector2(Mark.pivot.x, position);
			}
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		protected bool IsHorizontal()
		{
			return Direction == SwitchDirection.LeftToRight || Direction == SwitchDirection.RightToLeft;
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="state">State.</param>
		protected float GetPosition(bool state)
		{
			switch (Direction)
			{
				case SwitchDirection.LeftToRight:
				case SwitchDirection.BottomToTop:
					return state ? 1f : 0f;
				case SwitchDirection.RightToLeft:
				case SwitchDirection.TopToBottom:
					return state ? 0f : 1f;
			}

			return 0f;
		}

		/// <summary>
		/// Sets the color of the background.
		/// </summary>
		protected virtual void SetBackgroundColor()
		{
			if (Background == null)
			{
				return;
			}

			Background.color = IsOn ? BackgroundOnColor : BackgroundOffColor;
		}

		/// <summary>
		/// Sets the color of the mark.
		/// </summary>
		protected virtual void SetMarkColor()
		{
			if (MarkGraphic == null)
			{
				return;
			}

			MarkGraphic.color = IsOn ? MarkOnColor : MarkOffColor;
		}

		/// <summary>
		/// Calls the changed.
		/// </summary>
		protected virtual void CallChanged()
		{
			if (!IsActive() || !IsInteractable())
			{
				return;
			}

			IsOn = !IsOn;
		}

		/// <summary>
		/// Reverse the IsOn value.
		/// </summary>
		public virtual void Reverse()
		{
			IsOn = !IsOn;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnSubmit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			CallChanged();
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerClick event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			CallChanged();
		}

		/// <summary>
		/// Process the dimensions changed event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			if (!IsActive())
			{
				return;
			}

			Init();
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			SetTargetOwner();
			SetMarkPosition(false);
			SetBackgroundColor();
			SetMarkColor();
		}

		/// <summary>
		/// Set target owner.
		/// </summary>
		public virtual void SetTargetOwner()
		{
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Background, nameof(Background.color), this);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Mark, nameof(Background.color), this);
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			Init();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Handle validate event.
		/// </summary>
		public virtual void Validate()
		{
			Init();
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Switch.Border.ApplyTo(bg);
			}

			style.Switch.BackgroundDefault.ApplyTo(Background as Image);

			if ((Mark != null) && Mark.TryGetComponent<Image>(out var mark))
			{
				style.Switch.MarkDefault.ApplyTo(mark);
			}

			backgroundOnColor = style.Switch.BackgroundOnColor;
			backgroundOffColor = style.Switch.BackgroundOffColor;

			markOnColor = style.Switch.MarkOnColor;
			markOffColor = style.Switch.MarkOffColor;

			SetBackgroundColor();
			SetMarkColor();

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Switch.Border.GetFrom(bg);
			}

			style.Switch.BackgroundDefault.GetFrom(Background as Image);

			if ((Mark != null) && Mark.TryGetComponent<Image>(out var mark))
			{
				style.Switch.MarkDefault.GetFrom(mark);
			}

			style.Switch.BackgroundOnColor = backgroundOnColor;
			style.Switch.BackgroundOffColor = backgroundOffColor;

			style.Switch.MarkOnColor = markOnColor;
			style.Switch.MarkOffColor = markOffColor;

			return true;
		}
		#endregion
	}
}