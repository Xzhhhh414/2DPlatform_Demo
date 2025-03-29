namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Connector base class.
	/// </summary>
	[RequireComponent(typeof(TransformListener))]
	[RequireComponent(typeof(CanvasRenderer))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/connectors.html")]
	[DataBindSupport]
	public abstract class ConnectorBase : MaskableGraphicInitiable, IStylable
	{
		[SerializeField]
		Sprite sprite;

		/// <summary>
		/// Gets or sets the sprite.
		/// </summary>
		/// <value>The sprite.</value>
		[DataBindField]
		public Sprite Sprite
		{
			get => sprite;

			set
			{
				sprite = value;
				SetAllDirty();
			}
		}

		/// <summary>
		/// Image's texture comes from the UnityEngine.Image.
		/// </summary>
		public override Texture mainTexture => sprite == null ? s_WhiteTexture : sprite.texture;

		TransformListener currentTransformListener;

		/// <summary>
		/// Root canvas.
		/// </summary>
		protected Canvas RootCanvas
		{
			get
			{
				if (canvas == null)
				{
					return null;
				}

				return canvas.isRootCanvas ? canvas : canvas.rootCanvas;
			}
		}

		ILineBuilder builder = new LineBuilder();

		/// <summary>
		/// Line builder.
		/// </summary>
		[DataBindField]
		public ILineBuilder Builder
		{
			get => builder;

			set
			{
				builder = value;
				SetAllDirty();
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			AddSelfListener();
		}

		/// <summary>
		/// Adds the self listener.
		/// </summary>
		protected void AddSelfListener()
		{
			if (currentTransformListener == null)
			{
				TryGetComponent(out currentTransformListener);
			}

			if (currentTransformListener != null)
			{
				currentTransformListener.OnTransformChanged.AddListener(SetVerticesDirty);
			}
		}

		/// <summary>
		/// Removes the self listener.
		/// </summary>
		protected void RemoveSelfListener()
		{
			if (currentTransformListener != null)
			{
				currentTransformListener.OnTransformChanged.RemoveListener(SetVerticesDirty);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
		/// </summary>
		protected override void OnValidate()
		{
			RemoveSelfListener();
			AddSelfListener();
			SetVerticesDirty();

			base.OnValidate();
		}
#endif

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			RemoveSelfListener();
			base.OnDestroy();
		}

		/// <summary>
		/// Get target position relative to the canvas.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Position.</returns>
		protected virtual Vector3 CanvasRelativePosition(Transform target)
		{
			var pos = target.localPosition;
			if (canvas == null)
			{
				return pos;
			}

			var canvas_id = canvas.transform.GetInstanceID();

			while (true)
			{
				target = target.parent;

				if ((target == null) || (target.GetInstanceID() == canvas_id))
				{
					break;
				}

				pos += target.localPosition;
			}

			return pos;
		}

		/// <summary>
		/// Gets the point.
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="targetRectTransform">Target RectTransform.</param>
		/// <param name="position">Position.</param>
		public Vector3 GetPoint(RectTransform targetRectTransform, ConnectorPosition position)
		{
			var rect = GetPixelAdjustedRect();

			Vector3 delta;

			if (RootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				var camera = RootCanvas.worldCamera;
				delta = camera.WorldToScreenPoint(rectTransform.position) - camera.WorldToScreenPoint(targetRectTransform.position);

				if (canvas.scaleFactor != 0f)
				{
					delta /= canvas.scaleFactor;
				}
			}
			else if (RootCanvas.renderMode == RenderMode.WorldSpace)
			{
				delta = CanvasRelativePosition(rectTransform) - CanvasRelativePosition(targetRectTransform);
			}
			else
			{
				delta = rectTransform.position - targetRectTransform.position;

				if (canvas.scaleFactor != 0f)
				{
					delta /= canvas.scaleFactor;
				}
			}

			rect.x -= delta.x;
			rect.y -= delta.y;

			switch (position)
			{
				case ConnectorPosition.Left:
					rect.y += rect.height / 2f;
					break;
				case ConnectorPosition.Right:
					rect.x += rect.width;
					rect.y += rect.height / 2f;
					break;
				case ConnectorPosition.Top:
					rect.x += rect.width / 2f;
					rect.y += rect.height;
					break;
				case ConnectorPosition.Bottom:
					rect.x += rect.width / 2f;
					break;
				case ConnectorPosition.Center:
					rect.x += rect.width / 2f;
					rect.y += rect.height / 2f;
					break;
			}

			return new Vector3(rect.x, rect.y, 0);
		}

		/// <summary>
		/// Adds the line.
		/// </summary>
		/// <returns>The line.</returns>
		/// <param name="source">Source.</param>
		/// <param name="line">Line.</param>
		/// <param name="vh">Vertex helper.</param>
		/// <param name="index">Index.</param>
		public int AddLine(RectTransform source, ConnectorLine line, VertexHelper vh, int index)
		{
			if (RootCanvas == null)
			{
				return 0;
			}

			return Builder.Build(this, source, line, vh, index);
		}

		/// <summary>
		/// Rotates the point.
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="point">Point.</param>
		/// <param name="pivot">Pivot.</param>
		/// <param name="angle">Angle.</param>
		public static Vector3 RotatePoint(Vector3 point, Vector3 pivot, Quaternion angle)
		{
			var direction = angle * (point - pivot);

			return direction + pivot;
		}

		#region IStylable

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			color = style.Connector.Color;
			material = style.Connector.Material;
			Sprite = style.Connector.Sprite;

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.Connector.Color = color;
			style.Connector.Sprite = Sprite;
			Style.SetValue(material, ref style.Connector.Material);

			return true;
		}
		#endregion
	}
}