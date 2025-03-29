namespace UIWidgets
{
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// UI utilities.
	/// </summary>
	public static class UtilitiesUI
	{
		/// <summary>
		/// Select first child object with Selectable component.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="ignore">Object that should not be selected.</param>
		/// <returns>true if child object was selected; otherwise false.</returns>
		public static bool SelectChild(MonoBehaviour obj, Selectable ignore = null)
		{
			using var _ = ListPool<Selectable>.Get(out var selectable);

			obj.GetComponentsInChildren(selectable);
			if (selectable.Count == 1)
			{
				Select(selectable[0].gameObject);
				return true;
			}

			selectable.Remove(ignore);
			if (selectable.Count == 0)
			{
				return false;
			}

			Select(selectable[selectable.Count - 1].gameObject);
			return true;
		}

		/// <summary>
		/// Set EventSystem selected game object.
		/// </summary>
		/// <param name="go">Game object.</param>
		public static void Select(GameObject go)
		{
			var ev = EventSystem.current;
			if (ev == null)
			{
				return;
			}

			if (ev.alreadySelecting)
			{
				Updater.RunOnceNextFrame(go, () => ev.SetSelectedGameObject(go));
				return;
			}

			ev.SetSelectedGameObject(go);
		}

		/// <summary>
		/// Finds the canvas.
		/// </summary>
		/// <returns>The canvas.</returns>
		/// <param name="currentObject">Current object.</param>
		public static RectTransform FindCanvas(Transform currentObject)
		{
			var canvas = currentObject.GetComponentInParent<Canvas>();
			if (canvas == null)
			{
				return null;
			}

			return canvas.transform as RectTransform;
		}

		/// <summary>
		/// Finds the topmost canvas.
		/// </summary>
		/// <returns>The canvas.</returns>
		/// <param name="currentObject">Current object.</param>
		public static RectTransform FindTopmostCanvas(Transform currentObject)
		{
			using var _ = ListPool<Canvas>.Get(out var temp);

			currentObject.GetComponentsInParent(true, temp);

			return temp.Count > 0 ? temp[temp.Count - 1].transform as RectTransform : null;
		}

		/// <summary>
		/// Get canvas scale.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Scale.</returns>
		public static float GetCanvasScale(Transform target)
		{
			var canvas = target.GetComponentInParent<Canvas>();
			if (canvas == null)
			{
				return 1f;
			}

			if (!canvas.isRootCanvas)
			{
				canvas = canvas.rootCanvas;
			}

			if (canvas.renderMode == RenderMode.WorldSpace)
			{
				return 1f;
			}

			return canvas.scaleFactor;
		}

		/// <summary>
		/// Calculates the drag position.
		/// </summary>
		/// <returns>The drag position.</returns>
		/// <param name="screenPosition">Screen position.</param>
		/// <param name="canvas">Canvas.</param>
		/// <param name="canvasRect">Canvas RectTransform.</param>
		public static Vector3 CalculateDragPosition(Vector3 screenPosition, Canvas canvas, RectTransform canvasRect)
		{
			Vector3 result;
			var canvasSize = canvasRect.sizeDelta;
			Vector2 min = Vector2.zero;
			Vector2 max = canvasSize;

			var isOverlay = canvas.renderMode == RenderMode.ScreenSpaceOverlay;
			var noCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null;
			if (isOverlay || noCamera)
			{
				result = screenPosition;
			}
			else
			{
				var ray = canvas.worldCamera.ScreenPointToRay(screenPosition);
				var plane = new Plane(canvasRect.forward, canvasRect.position);

				plane.Raycast(ray, out var distance);

				result = canvasRect.InverseTransformPoint(ray.origin + (ray.direction * distance));

				min = -Vector2.Scale(max, canvasRect.pivot);
				max = canvasSize - min;
			}

			result.x = Mathf.Clamp(result.x, min.x, max.y);
			result.y = Mathf.Clamp(result.y, min.x, max.y);

			return result;
		}

		/// <summary>
		/// Determines if slider is horizontal.
		/// </summary>
		/// <returns><c>true</c> if slider is horizontal; otherwise, <c>false</c>.</returns>
		/// <param name="slider">Slider.</param>
		public static bool IsHorizontal(Slider slider)
		{
			return slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft;
		}

		/// <summary>
		/// Determines if scrollbar is horizontal.
		/// </summary>
		/// <returns><c>true</c> if scrollbar is horizontal; otherwise, <c>false</c>.</returns>
		/// <param name="scrollbar">Scrollbar.</param>
		public static bool IsHorizontal(Scrollbar scrollbar)
		{
			return scrollbar.direction == Scrollbar.Direction.LeftToRight || scrollbar.direction == Scrollbar.Direction.RightToLeft;
		}

		/// <summary>
		/// Get graphic component from TextAdapter.
		/// </summary>
		/// <param name="adapter">Adapter.</param>
		/// <returns>Graphic component.</returns>
		public static Graphic GetGraphic(TextAdapter adapter)
		{
			return (adapter != null) ? adapter.Graphic : null;
		}
	}
}