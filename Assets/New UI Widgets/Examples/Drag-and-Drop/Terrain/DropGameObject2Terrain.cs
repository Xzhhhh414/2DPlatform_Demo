namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drop gameobject to terrain.
	/// Required PhysicsRaycaster on camera.
	/// </summary>
	public class DropGameObject2Terrain : MonoBehaviour, IDropSupport<GameObject>
	{
		class HitComparer : IComparer<RaycastHit>
		{
			public int Compare(RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance);
		}

		struct DropData
		{
			public bool Allow;
			public Vector3 Position;
		}

		/// <summary>
		/// Drop event.
		/// </summary>
		public event Action<GameObject> OnDrop;

		/// <summary>
		/// Pivot fix (for the demo purpose).
		/// </summary>
		[SerializeField]
		protected Vector3 PivotFix = new Vector3(0, 2f, 0f);

		readonly RaycastHit[] hits = new RaycastHit[10];

		readonly HitComparer comparer = new HitComparer();

		/// <inheritdoc/>
		public bool CanReceiveDrop(GameObject data, PointerEventData eventData)
		{
			var drop = ProcessEvent(data, eventData);

			data.transform.position = drop.Position + PivotFix;
			data.SetActive(drop.Allow);

			return drop.Allow;
		}

		/// <inheritdoc/>
		public void Drop(GameObject data, PointerEventData eventData)
		{
			var drop = ProcessEvent(data, eventData);

			if (drop.Allow)
			{
				var instance = Instantiate(data);
				instance.transform.position = drop.Position + PivotFix;

				OnDrop?.Invoke(instance);
			}

			data.SetActive(false);
		}

		/// <inheritdoc/>
		public void DropCanceled(GameObject data, PointerEventData eventData)
		{
			if (data != null)
			{
				data.SetActive(false);
			}
		}

		DropData ProcessEvent(GameObject target, PointerEventData eventData)
		{
			var result = default(DropData);

			var instance_id = gameObject.GetInstanceID();
			var target_id = target.GetInstanceID();

			var ray = Camera.main.ScreenPointToRay(eventData.position);
			var hits_count = Physics.RaycastNonAlloc(ray, hits);

			// sort by hit distance
			Array.Sort(hits, 0, hits_count, comparer);

			var terrain_id = 0;
			for (int i = 0; i < hits_count; i++)
			{
				var hit = hits[i];
				var hit_id = hit.transform.gameObject.GetInstanceID();

				// skip hit on dragged data
				if (hit_id == target_id)
				{
					terrain_id += 1;
					continue;
				}

				// skip hits on other objects
				if (hit_id != instance_id)
				{
					continue;
				}

				// value equals if direct hit (nothing between cursor and terrain except dragged object)
				result.Allow = i == terrain_id;
				result.Position = hit.point;
				break;
			}

			return result;
		}
	}
}