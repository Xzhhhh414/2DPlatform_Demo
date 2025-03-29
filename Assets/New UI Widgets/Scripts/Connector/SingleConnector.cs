namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Single connector.
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("UI/New UI Widgets/Connectors/Single Connector")]
	public class SingleConnector : ConnectorBase
	{
		/// <summary>
		/// The line.
		/// </summary>
		[SerializeField]
		protected ConnectorLine line;

		/// <summary>
		/// Gets or sets the line.
		/// </summary>
		/// <value>The line.</value>
		[DataBindField]
		public ConnectorLine Line
		{
			get
			{
				return line;
			}

			set
			{
				if (line != null)
				{
					line.OnChange -= LinesChanged;
				}

				line = value;

				if (line != null)
				{
					line.OnChange += LinesChanged;
				}

				LinesChanged();
			}
		}

		/// <summary>
		/// The listener.
		/// </summary>
		protected TransformListener listener;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Line = line;
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			LinesChanged();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			RemoveListener();

			base.OnDestroy();
		}

		/// <summary>
		/// Process lines changes.
		/// </summary>
		protected virtual void LinesChanged()
		{
			RemoveListener();
			AddListener();
			SetVerticesDirty();
		}

		/// <summary>
		/// Removes the listener.
		/// </summary>
		protected virtual void RemoveListener()
		{
			if (listener != null)
			{
				listener.OnTransformChanged.RemoveListener(SetVerticesDirty);
			}
		}

		/// <summary>
		/// Adds the listener.
		/// </summary>
		protected virtual void AddListener()
		{
			if ((Line != null) && (Line.Target != null))
			{
				listener = Utilities.RequireComponent<TransformListener>(Line.Target);
				listener.OnTransformChanged.AddListener(SetVerticesDirty);
			}
		}

		/// <summary>
		/// Fill the vertex buffer data.
		/// </summary>
		/// <param name="vh">VertexHelper.</param>
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			AddLine(rectTransform, Line, vh, 0);
		}
	}
}