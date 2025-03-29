namespace UIWidgets
{
	using System;
	using System.Collections;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Animations.
	/// </summary>
	public static class Animations
	{
		/// <summary>
		/// Run animation with animation curve value.
		/// </summary>
		/// <param name="curve">Animation curve.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, bool unscaledTime, Action<float> setValue, Action onEnd = null)
		{
			var duration = curve[curve.length - 1].time;
			var time = 0f;

			do
			{
				setValue(curve.Evaluate(time));
				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			setValue(curve.Evaluate(duration));

			onEnd?.Invoke();
		}

		/// <summary>
		/// Run animation with float value.
		/// </summary>
		/// <param name="curve">Animation curve with value in range [0..1].</param>
		/// <param name="startValue">Start value.</param>
		/// <param name="endValue">End value.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, float startValue, float endValue, bool unscaledTime, Action<float> setValue, Action onEnd = null)
		{
			var duration = curve[curve.length - 1].time;
			var time = 0f;

			do
			{
				var value = Mathf.Lerp(startValue, endValue, curve.Evaluate(time));
				setValue(value);
				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time <= duration);

			setValue(endValue);

			onEnd?.Invoke();
		}

		/// <summary>
		/// Run animation with Vector2 values.
		/// </summary>
		/// <param name="curve">Animation curve with value in range [0..1].</param>
		/// <param name="startValue">Start value.</param>
		/// <param name="endValue">End value.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, Vector2 startValue, Vector2 endValue, bool unscaledTime, Action<Vector2> setValue, Action onEnd = null)
		{
			var duration = curve[curve.length - 1].time;
			var time = 0f;

			do
			{
				var value = Vector2.Lerp(startValue, endValue, curve.Evaluate(time));
				setValue(value);
				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			setValue(endValue);

			onEnd?.Invoke();
		}

		/// <summary>
		/// Run animation with Vector3 values.
		/// </summary>
		/// <param name="curve">Animation curve with value in range [0..1].</param>
		/// <param name="startValue">Start value.</param>
		/// <param name="endValue">End value.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, Vector3 startValue, Vector3 endValue, bool unscaledTime, Action<Vector3> setValue, Action onEnd = null)
		{
			var duration = curve[curve.length - 1].time;
			var time = 0f;

			do
			{
				var value = Vector3.Lerp(startValue, endValue, curve.Evaluate(time));

				setValue(value);
				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			setValue(endValue);

			onEnd?.Invoke();
		}

		/// <summary>
		/// Run animation Quaternion values.
		/// </summary>
		/// <param name="curve">Animation curve with value in range [0..1].</param>
		/// <param name="startValue">Start value.</param>
		/// <param name="endValue">End value.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="setValue">Function to set value.</param>
		/// <param name="onEnd">Action on animation end.</param>
		/// <returns>Animation coroutine.</returns>
		public static IEnumerator Run(AnimationCurve curve, Quaternion startValue, Quaternion endValue, bool unscaledTime, Action<Quaternion> setValue, Action onEnd = null)
		{
			var duration = curve[curve.length - 1].time;
			var time = 0f;

			do
			{
				var value = Quaternion.Lerp(startValue, endValue, curve.Evaluate(time));
				setValue(value);
				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			setValue(endValue);

			onEnd?.Invoke();
		}

		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="duration">Time.</param>
		/// <param name="startAngle">Start rotation angle.</param>
		/// <param name="endAngle">End rotation angle.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Rotate(RectTransform rectTransform, float duration = 0.5f, float startAngle = 0, float endAngle = 90, bool unscaledTime = false, Action actionAfter = null)
		{
			var start_rotarion = rectTransform.localRotation.eulerAngles;
			var time = 0f;

			do
			{
				var rotation_x = Mathf.Lerp(startAngle, endAngle, time / duration);

				rectTransform.localRotation = Quaternion.Euler(rotation_x, start_rotarion.y, start_rotarion.z);
				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			// return rotation back for future use
			rectTransform.localRotation = Quaternion.Euler(start_rotarion);

			actionAfter?.Invoke();
		}

		/// <summary>
		/// Rotate animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="duration">Time.</param>
		/// <param name="startAngle">Start rotation angle.</param>
		/// <param name="endAngle">End rotation angle.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator RotateZ(RectTransform rectTransform, float duration = 0.5f, float startAngle = 0, float endAngle = 90, bool unscaledTime = false, Action actionAfter = null)
		{
			var start_rotarion = rectTransform.localRotation.eulerAngles;
			var time = 0f;

			do
			{
				var rotation_z = Mathf.Lerp(startAngle, endAngle, time / duration);

				rectTransform.localRotation = Quaternion.Euler(start_rotarion.x, start_rotarion.y, rotation_z);
				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time <= duration);

			// return rotation back for future use
			rectTransform.localRotation = Quaternion.Euler(start_rotarion);

			actionAfter?.Invoke();
		}

		[DomainReloadExclude]
		static readonly AnimationCurve LinearCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Collapse(RectTransform rectTransform, float time = 0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			return Collapse(rectTransform, time, LinearCurve, isHorizontal, unscaledTime, actionAfter);
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="curve">Animation curve.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Collapse(RectTransform rectTransform, float duration, AnimationCurve curve, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var layout_element = Utilities.RequireComponent<LayoutElement>(rectTransform);
			var layout_size = new LayoutElementData(layout_element);
			var size = isHorizontal
				? (LayoutUtilities.IsWidthControlled(rectTransform) ? layout_size.PreferredWidth : rectTransform.rect.width)
				: (LayoutUtilities.IsHeightControlled(rectTransform) ? layout_size.PreferredHeight : rectTransform.rect.height);

			var rate = curve[curve.length - 1].time / duration;
			var time = 0f;

			do
			{
				var new_size = size * (1f - curve.Evaluate(time * rate));
				if (isHorizontal)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, new_size);
					layout_element.preferredWidth = new_size;
				}
				else
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, new_size);
					layout_element.preferredHeight = new_size;
				}

				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time <= duration);

			// return size back for future use
			if (isHorizontal)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				layout_element.preferredWidth = size;
			}
			else
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
				layout_element.preferredHeight = size;
			}

			actionAfter?.Invoke();
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator CollapseFlexible(RectTransform rectTransform, float time = 0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			return CollapseFlexible(rectTransform, time, LinearCurve, isHorizontal, unscaledTime, actionAfter);
		}

		/// <summary>
		/// Collapse animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="rate">Speed rate.</param>
		/// <param name="curve">Animation curve.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator CollapseFlexible(RectTransform rectTransform, float rate, AnimationCurve curve, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var layout_element = Utilities.RequireComponent<LayoutElement>(rectTransform);
			if (isHorizontal)
			{
				layout_element.preferredWidth = -1f;
			}
			else
			{
				layout_element.preferredHeight = -1f;
			}

			var duration = curve[curve.length - 1].time * rate;
			var time = 0f;

			do
			{
				var size = curve.Evaluate(time / duration);
				if (isHorizontal)
				{
					layout_element.flexibleWidth = size;
				}
				else
				{
					layout_element.flexibleHeight = size;
				}

				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time <= duration);

			if (isHorizontal)
			{
				layout_element.flexibleWidth = 0f;
			}
			else
			{
				layout_element.flexibleHeight = 0f;
			}

			actionAfter?.Invoke();
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Open(RectTransform rectTransform, float time = 0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			return Open(rectTransform, time, LinearCurve, isHorizontal, unscaledTime, actionAfter);
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="curve">Animation curve.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator Open(RectTransform rectTransform, float duration, AnimationCurve curve, bool isHorizontal, bool unscaledTime, Action actionAfter = null)
		{
			var layout_element = Utilities.RequireComponent<LayoutElement>(rectTransform);
			var layout_size = new LayoutElementData(layout_element);
			var size = isHorizontal
				? (LayoutUtilities.IsWidthControlled(rectTransform) ? layout_size.PreferredWidth : rectTransform.rect.width)
				: (LayoutUtilities.IsHeightControlled(rectTransform) ? layout_size.PreferredHeight : rectTransform.rect.height);

			var rate = curve[curve.length - 1].time / duration;
			var time = 0f;

			do
			{
				var new_size = size * curve.Evaluate(time * rate);
				if (isHorizontal)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, new_size);
					layout_element.preferredWidth = new_size;
				}
				else
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, new_size);
					layout_element.preferredHeight = new_size;
				}

				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			// return size back for future use
			if (isHorizontal)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				layout_element.preferredWidth = size;
			}
			else
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
				layout_element.preferredHeight = size;
			}

			actionAfter?.Invoke();
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="time">Time.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator OpenFlexible(RectTransform rectTransform, float time = 0.5f, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			return OpenFlexible(rectTransform, time, LinearCurve, isHorizontal, unscaledTime, actionAfter);
		}

		/// <summary>
		/// Open animation.
		/// </summary>
		/// <returns>Animation.</returns>
		/// <param name="rectTransform">RectTransform.</param>
		/// <param name="rate">Speed rate.</param>
		/// <param name="curve">Animation curve.</param>
		/// <param name="isHorizontal">Is Horizontal animated width changes; otherwise height.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="actionAfter">Action to run after animation.</param>
		public static IEnumerator OpenFlexible(RectTransform rectTransform, float rate, AnimationCurve curve, bool isHorizontal = false, bool unscaledTime = false, Action actionAfter = null)
		{
			var layoutElement = Utilities.RequireComponent<LayoutElement>(rectTransform);
			if (isHorizontal)
			{
				layoutElement.preferredWidth = -1f;
			}
			else
			{
				layoutElement.preferredHeight = -1f;
			}

			var duration = curve[curve.length - 1].time * rate;
			var time = 0f;

			do
			{
				var size = curve.Evaluate(time / duration);
				if (isHorizontal)
				{
					layoutElement.flexibleWidth = size;
				}
				else
				{
					layoutElement.flexibleHeight = size;
				}

				yield return null;

				time += UtilitiesTime.GetDeltaTime(unscaledTime);
			}
			while (time < duration);

			// return size back for future use
			if (isHorizontal)
			{
				layoutElement.flexibleWidth = 1f;
			}
			else
			{
				layoutElement.flexibleHeight = 1f;
			}

			actionAfter?.Invoke();
		}
	}
}