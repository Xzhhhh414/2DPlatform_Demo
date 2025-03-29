namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Scale widget.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/scale.html")]
	[DataBindSupport]
	public class Scale : MonoBehaviourConditional, IObservable, INotifyPropertyChanged, IStylable
	{
		/// <summary>
		/// Mark data.
		/// </summary>
		public readonly struct MarkData : IEquatable<MarkData>
		{
			/// <summary>
			/// Position.
			/// </summary>
			public readonly Vector2 Position;

			/// <summary>
			/// Rotation.
			/// </summary>
			public readonly float Rotation;

			/// <summary>
			/// Anchors min.
			/// </summary>
			public readonly Vector2 AnchorMin;

			/// <summary>
			/// Anchors max.
			/// </summary>
			public readonly Vector2 AnchorMax;

			/// <summary>
			/// Pivot.
			/// </summary>
			public readonly Vector2 Pivot;

			/// <summary>
			/// Value.
			/// </summary>
			public readonly float Value;

			/// <summary>
			/// Convert value to label.
			/// </summary>
			public readonly Func<float, string> Value2Label;

			/// <summary>
			/// Label.
			/// </summary>
			public string Label => Value2Label != null ? Value2Label(Value) : Value.ToString();

			/// <summary>
			/// Initializes a new instance of the <see cref="MarkData"/> struct.
			/// </summary>
			/// <param name="value">Value.</param>
			/// <param name="position">Position.</param>
			/// <param name="rotation">Rotation.</param>
			/// <param name="anchorMin">Anchors min.</param>
			/// <param name="anchorMax">Anchors max.</param>
			/// <param name="pivot">Pivot.</param>
			/// <param name="value2label">Convert value to label.</param>
			public MarkData(
				float value,
				Vector2 position = default,
				float rotation = 0f,
				Vector2 anchorMin = default,
				Vector2 anchorMax = default,
				Vector2? pivot = null,
				Func<float, string> value2label = null)
			{
				Value = value;

				Position = position;
				Rotation = rotation;

				AnchorMin = anchorMin;
				AnchorMax = anchorMax;
				Pivot = pivot ?? new Vector2(0.5f, 0.5f);

				Value2Label = value2label;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj) => (obj is MarkData data) && Equals(data);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(MarkData other)
			{
				return (Position == other.Position)
					&& (Rotation == other.Rotation)
					&& (AnchorMin == other.AnchorMin)
					&& (AnchorMax == other.AnchorMax)
					&& (Pivot == other.Pivot)
					&& (Value == other.Value)
					&& (Value2Label == other.Value2Label);
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return Position.GetHashCode() ^ Rotation.GetHashCode() ^ AnchorMin.GetHashCode() ^ AnchorMax.GetHashCode() ^ Pivot.GetHashCode() ^ Value.GetHashCode() ^ Value2Label.GetHashCode();
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(MarkData a, MarkData b) => a.Equals(b);

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances not equal; otherwise, false.</returns>
			public static bool operator !=(MarkData a, MarkData b) => !a.Equals(b);
		}

		[SerializeField]
		RectTransform container;

		/// <summary>
		/// Container.
		/// </summary>
		public RectTransform Container
		{
			get => container;

			set => Change(ref container, value, nameof(Container));
		}

		[SerializeField]
		Image mainLine;

		/// <summary>
		/// Main line.
		/// </summary>
		public Image MainLine
		{
			get => mainLine;

			set => Change(ref mainLine, value, nameof(MainLine));
		}

		[SerializeField]
		bool showCurrentValue = true;

		/// <summary>
		/// Show current values.
		/// </summary>
		[DataBindField]
		public bool ShowCurrentValue
		{
			get => showCurrentValue;

			set => Change(ref showCurrentValue, value, nameof(ShowCurrentValue));
		}

		[SerializeField]
		[EditorConditionBool(nameof(showCurrentValue))]
		ScaleMarkTemplate currentMarkTemplate;

		/// <summary>
		/// Current mark template.
		/// </summary>
		public ScaleMarkTemplate CurrentMarkTemplate
		{
			get => currentMarkTemplate;

			set => Change(ref currentMarkTemplate, value, nameof(CurrentMarkTemplate));
		}

		[SerializeField]
		bool showMinValue = true;

		/// <summary>
		/// Show minimal value.
		/// </summary>
		[DataBindField]
		public bool ShowMinValue
		{
			get => showMinValue;

			set => Change(ref showMinValue, value, nameof(ShowMinValue));
		}

		[SerializeField]
		[EditorConditionBool(nameof(showMinValue))]
		ScaleMarkTemplate minMark;

		/// <summary>
		/// Min mark.
		/// </summary>
		public ScaleMarkTemplate MinMark
		{
			get => minMark;

			set => Change(ref minMark, value, nameof(MinMark));
		}

		[SerializeField]
		bool showMaxValue = true;

		/// <summary>
		/// Show max value.
		/// </summary>
		[DataBindField]
		public bool ShowMaxValue
		{
			get => showMaxValue;

			set => Change(ref showMaxValue, value, nameof(ShowMaxValue));
		}

		[SerializeField]
		[EditorConditionBool(nameof(showMaxValue))]
		ScaleMarkTemplate maxMark;

		/// <summary>
		/// Max mark.
		/// </summary>
		public ScaleMarkTemplate MaxMark
		{
			get => maxMark;

			set => Change(ref maxMark, value, nameof(MaxMark));
		}

		[SerializeField]
		List<ScaleMark> scaleMarks = new List<ScaleMark>();

		ObservableList<ScaleMark> marks;

		/// <summary>
		/// Marks.
		/// </summary>
		[DataBindField]
		public ObservableList<ScaleMark> Marks
		{
			get
			{
				marks ??= new ObservableList<ScaleMark>(scaleMarks)
				{
					Comparison = MarkComparison,
				};

				return marks;
			}

			set
			{
				marks?.OnChangeMono.RemoveListener(MarksChanged);

				marks = value;

				if (marks != null)
				{
					marks.Comparison = MarkComparison;
					marks.OnChangeMono.AddListener(MarksChanged);
				}

				MarksChanged();
			}
		}

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Items comparison.
		/// </summary>
		[DomainReloadExclude]
		public static readonly Comparison<ScaleMark> MarkComparison = (x, y) => -x.Step.CompareTo(y.Step);

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (container == null)
			{
				container = transform as RectTransform;
			}

			if (currentMarkTemplate != null)
			{
				currentMarkTemplate.gameObject.SetActive(false);
			}

			if (minMark != null)
			{
				minMark.gameObject.SetActive(false);
			}

			if (maxMark != null)
			{
				maxMark.gameObject.SetActive(false);
			}

			if (mainLine != null)
			{
				mainLine.gameObject.SetActive(false);
			}

			for (int i = 0; i < Marks.Count; i++)
			{
				var mark = Marks[i];
				if (mark.Step <= 0)
				{
					Debug.LogWarning("ScaleMark.Step cannot be negative or zero.", this);
					continue;
				}

				mark.Template.gameObject.SetActive(false);
			}

			foreach (var instance in MarksInstances)
			{
				if (instance != null)
				{
					instance.Return();
				}
			}

			MarksInstances.Clear();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Clear();

			if (marks != null)
			{
				marks.OnChangeMono.RemoveListener(MarksChanged);
				marks = null;
			}
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Mark values generator.
		/// </summary>
		public Action<float, float, float, List<float>> MarkValuesGenerator = DefaultGenerator;

		/// <summary>
		/// Used values.
		/// </summary>
		protected HashSet<float> UsedValues = new HashSet<float>();

		/// <summary>
		/// Marks instances.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ScaleMarkTemplate> MarksInstances = new List<ScaleMarkTemplate>();

		/// <summary>
		/// Properties tracker.
		/// </summary>
		protected DrivenRectTransformTracker PropertiesTracker;

		/// <summary>
		/// Driven properties.
		/// </summary>
		protected DrivenTransformProperties DrivenProperties = DrivenTransformProperties.AnchoredPosition
			| DrivenTransformProperties.Anchors
			| DrivenTransformProperties.Pivot
			| DrivenTransformProperties.Rotation;

		/// <summary>
		/// Change value.
		/// </summary>
		/// <typeparam name="T">Type of field.</typeparam>
		/// <param name="field">Field value.</param>
		/// <param name="value">New value.</param>
		/// <param name="propertyName">Property name.</param>
		protected void Change<T>(ref T field, T value, string propertyName)
		{
			if (!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				NotifyPropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// Raise PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			OnChange?.Invoke();

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Process marks changed events.
		/// </summary>
		protected virtual void MarksChanged()
		{
			Clear();
			NotifyPropertyChanged("Marks");
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public virtual void Clear()
		{
			PropertiesTracker.Clear();

			UsedValues.Clear();

			if (CurrentMarkTemplate != null)
			{
				CurrentMarkTemplate.gameObject.SetActive(false);
			}

			if (MinMark != null)
			{
				MinMark.gameObject.SetActive(false);
			}

			if (MaxMark != null)
			{
				MaxMark.gameObject.SetActive(false);
			}

			foreach (var instance in MarksInstances)
			{
				if (instance != null)
				{
					instance.Return();
				}
			}

			MarksInstances.Clear();
		}

		/// <summary>
		/// Set min mark.
		/// </summary>
		/// <param name="mark">Mark data.</param>
		protected virtual void SetMin(MarkData mark)
		{
			if (MinMark == null)
			{
				return;
			}

			if (ShowMinValue)
			{
				MinMark.gameObject.SetActive(true);
				Set(MinMark, mark);
			}
			else
			{
				MinMark.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Set max mark.
		/// </summary>
		/// <param name="mark">Mark data.</param>
		protected virtual void SetMax(MarkData mark)
		{
			if (MaxMark == null)
			{
				return;
			}

			if (ShowMaxValue)
			{
				MaxMark.gameObject.SetActive(true);
				Set(MaxMark, mark);
			}
			else
			{
				MaxMark.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Set mark.
		/// </summary>
		/// <param name="instance">Instance.</param>
		/// <param name="mark">Mark data.</param>
		protected virtual void Set(ScaleMarkTemplate instance, MarkData mark)
		{
			var rt = instance.RectTransform;

			PropertiesTracker.Add(this, rt, DrivenProperties);

			rt.anchorMin = mark.AnchorMin;
			rt.anchorMax = mark.AnchorMax;
			rt.pivot = mark.Pivot;

			rt.anchoredPosition = mark.Position;
			rt.localRotation = Quaternion.Euler(0f, 0f, mark.Rotation);

			if (instance.Label != null)
			{
				instance.Label.text = mark.Label;

				var label_rt = instance.Label.transform as RectTransform;
				label_rt.localRotation = Quaternion.Euler(0f, 0f, -mark.Rotation);
			}
		}

		/// <summary>
		/// Default marks values generator.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="step">Step.</param>
		/// <param name="output">Result.</param>
		public static void DefaultGenerator(float min, float max, float step, List<float> output)
		{
			for (var value = min; value < max; value += step)
			{
				output.Add(value);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current">Current values.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, params float[] current)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current);
			}
		}

		/// <summary>
		/// Set base scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max)
		{
			Init();
			Clear();

			MarkValuesGenerator ??= DefaultGenerator;

			if (MainLine != null)
			{
				MainLine.gameObject.SetActive(true);
			}

			SetMin(value2mark(min));
			if (ShowMinValue)
			{
				UsedValues.Add(min);
			}

			SetMax(value2mark(max));
			if (ShowMinValue)
			{
				UsedValues.Add(max);
			}

			for (int i = 0; i < Marks.Count; i++)
			{
				var mark = Marks[i];
				if (mark.Step <= 0)
				{
					continue;
				}

				using var _ = ListPool<float>.Get(out var values);
				MarkValuesGenerator(min, max, mark.Step, values);

				foreach (var value in values)
				{
					if (UsedValues.Contains(value))
					{
						continue;
					}

					var instance = mark.Template.GetInstance(Container);
					Set(instance, value2mark(value));

					MarksInstances.Add(instance);
					UsedValues.Add(value);
				}
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current">Current value 0.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current0">Current value 0.</param>
		/// <param name="current1">Current value 1.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current0, float current1)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current0);
				SetCurrent(value2mark, current1);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current0">Current value 0.</param>
		/// <param name="current1">Current value 1.</param>
		/// <param name="current2">Current value 2.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current0, float current1, float current2)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current0);
				SetCurrent(value2mark, current1);
				SetCurrent(value2mark, current2);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current0">Current value 0.</param>
		/// <param name="current1">Current value 1.</param>
		/// <param name="current2">Current value 2.</param>
		/// <param name="current3">Current value 3.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current0, float current1, float current2, float current3)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current0);
				SetCurrent(value2mark, current1);
				SetCurrent(value2mark, current2);
				SetCurrent(value2mark, current3);
			}
		}

		/// <summary>
		/// Set scale data.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="current0">Current value 0.</param>
		/// <param name="current1">Current value 1.</param>
		/// <param name="current2">Current value 2.</param>
		/// <param name="current3">Current value 3.</param>
		/// <param name="current4">Current value 4.</param>
		public virtual void Set(Func<float, MarkData> value2mark, float min, float max, float current0, float current1, float current2, float current3, float current4)
		{
			Set(value2mark, min, max);

			if (ShowCurrentValue)
			{
				SetCurrent(value2mark, current0);
				SetCurrent(value2mark, current1);
				SetCurrent(value2mark, current2);
				SetCurrent(value2mark, current3);
				SetCurrent(value2mark, current4);
			}
		}

		/// <summary>
		/// Set current marks.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="current">Current values.</param>
		protected virtual void SetCurrent(Func<float, MarkData> value2mark, params float[] current)
		{
			foreach (var value in current)
			{
				SetCurrent(value2mark, value);
			}
		}

		/// <summary>
		/// Set current mark.
		/// </summary>
		/// <param name="value2mark">Convert value to mark data.</param>
		/// <param name="current">Current value.</param>
		protected virtual void SetCurrent(Func<float, MarkData> value2mark, float current)
		{
			var instance = CurrentMarkTemplate.GetInstance(Container);
			Set(instance, value2mark(current));

			MarksInstances.Add(instance);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Scale.MainLine.ApplyTo(MainLine);

			foreach (var instance in MarksInstances)
			{
				if (instance != null)
				{
					instance.SetStyle(style);
				}
			}

			if (currentMarkTemplate != null)
			{
				currentMarkTemplate.SetStyle(style);
			}

			if (minMark != null)
			{
				minMark.SetStyle(style);
			}

			if (maxMark != null)
			{
				maxMark.SetStyle(style);
			}

			if (IsInited)
			{
				for (int i = 0; i < Marks.Count; i++)
				{
					Marks[i].Template.SetStyle(style);
				}
			}
			else
			{
				for (int i = 0; i < scaleMarks.Count; i++)
				{
					scaleMarks[i].Template.SetStyle(style);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Scale.MainLine.GetFrom(MainLine);

			if (currentMarkTemplate != null)
			{
				currentMarkTemplate.GetStyle(style);
			}
			else if (minMark != null)
			{
				minMark.GetStyle(style);
			}
			else if (maxMark != null)
			{
				maxMark.GetStyle(style);
			}
			else
			{
				for (int i = 0; i < scaleMarks.Count; i++)
				{
					scaleMarks[i].Template.GetStyle(style);
				}
			}

			return true;
		}
		#endregion
	}
}