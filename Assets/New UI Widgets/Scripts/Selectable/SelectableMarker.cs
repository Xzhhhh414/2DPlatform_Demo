namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Shows the specified marker for the currently selected game object.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/selectable-marker.html")]
	public class SelectableMarker : MonoBehaviourInitiable, IUpdatable
	{
		/// <summary>
		/// Target data.
		/// </summary>
		protected readonly struct TargetData : IEquatable<TargetData>
		{
			/// <summary>
			/// Selected object.
			/// </summary>
			public readonly GameObject Target;

			/// <summary>
			/// Target is not null.
			/// </summary>
			public readonly bool Valid;

			/// <summary>
			/// Target ID.
			/// </summary>
			public readonly int TargetId;

			/// <summary>
			/// Widget.
			/// </summary>
			public readonly GameObject Widget;

			/// <summary>
			/// Widget ID.
			/// </summary>
			public readonly int WidgetId;

			/// <summary>
			/// RectTransform.
			/// </summary>
			public readonly RectTransform RectTransform;

			/// <summary>
			/// Initializes a new instance of the <see cref="TargetData"/> struct.
			/// </summary>
			/// <param name="target">Target.</param>
			public TargetData(GameObject target)
			{
				Target = target;
				Valid = target != null;
				TargetId = Valid ? target.GetInstanceID() : 0;
				Widget = target;

				if (Valid)
				{
					var t = target.GetComponentInParent<ISelectableMarkerTarget>() as MonoBehaviour;
					if (t != null)
					{
						Widget = t.gameObject;
					}
				}

				WidgetId = Valid ? Widget.GetInstanceID() : 0;
				RectTransform = Valid ? Widget.transform as RectTransform : null;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj) => (obj is TargetData id) && Equals(id);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(TargetData other) => (Valid == other.Valid) && (TargetId == other.TargetId) && (WidgetId == other.WidgetId);

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode() => TargetId ^ WidgetId;

			/// <summary>
			/// Compare specified ids.
			/// </summary>
			/// <param name="a">First id.</param>
			/// <param name="b">Second id.</param>
			/// <returns>true if the ids are equal; otherwise, false.</returns>
			public static bool operator ==(TargetData a, TargetData b) => (a.Valid == b.Valid) && (a.TargetId == b.TargetId) && (a.WidgetId == b.WidgetId);

			/// <summary>
			/// Compare specified ids.
			/// </summary>
			/// <param name="a">First id.</param>
			/// <param name="b">Second id.</param>
			/// <returns>true if the ids not equal; otherwise, false.</returns>
			public static bool operator !=(TargetData a, TargetData b) => (a.Valid != b.Valid) || (a.TargetId != b.TargetId) || (a.WidgetId != b.WidgetId);

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			public override string ToString() => Valid ? "null" : Target.ToString();
		}

		/// <summary>
		/// Marker.
		/// </summary>
		[SerializeField]
		public RectTransform Marker;

		[NonSerialized]
		RectTransform markerInstance;

		/// <summary>
		/// Marker instance.
		/// Because of selected object can be destroyed together with marker.
		/// </summary>
		protected RectTransform MarkerInstance
		{
			get
			{
				if (markerInstance == null)
				{
					markerInstance = Instantiate(Marker, Marker.parent);
					Utilities.FixInstantiated(Marker, markerInstance);

					var le = Utilities.RequireComponent<LayoutElement>(markerInstance);
					le.ignoreLayout = true;
				}

				return markerInstance;
			}
		}

		/// <summary>
		/// Target data.
		/// </summary>
		protected TargetData Target;

		/// <summary>
		/// Show marker only for the nested game objects.
		/// </summary>
		[SerializeField]
		[Tooltip("Show marker only for the nested game objects.")]
		public bool ChildrenOnly = false;

		/// <summary>
		/// Check if a marker should be displayed for the specified object. It has more priority than the ChildrenOnly option.
		/// </summary>
		public Predicate<GameObject> RequireMarker;

		/// <summary>
		/// Default check is marker should be displayed for the specified object.
		/// </summary>
		public Predicate<GameObject> DefaultRequireMarker
		{
			get;
			protected set;
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			Marker.gameObject.SetActive(false);
			DefaultRequireMarker = x => !ChildrenOnly || x.transform.IsChildOf(transform);
			RequireMarker = DefaultRequireMarker;
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable() => Updater.Add(this);

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable() => Updater.Remove(this);

		/// <summary>
		/// Update.
		/// </summary>
		public virtual void RunUpdate()
		{
			var go = EventSystem.current.currentSelectedGameObject;
			var data = new TargetData(go);
			if (Target != data)
			{
				Target = data;
				UpdateMarker();
			}
		}

		/// <summary>
		/// Show or hide marker for the selected object.
		/// </summary>
		protected virtual void UpdateMarker()
		{
			if (!Target.Valid || !RequireMarker(Target.Widget))
			{
				MarkerInstance.gameObject.SetActive(false);
				MarkerInstance.SetParent(Marker.transform.parent, false);
				return;
			}

			MarkerInstance.SetParent(Target.RectTransform, false);
			MarkerInstance.SetAsLastSibling();

			MarkerInstance.anchorMin = Vector2.zero;
			MarkerInstance.anchorMax = Vector2.one;
			MarkerInstance.anchoredPosition = Vector3.zero;
			MarkerInstance.sizeDelta = Vector2.zero;

			MarkerInstance.gameObject.SetActive(true);
		}
	}
}