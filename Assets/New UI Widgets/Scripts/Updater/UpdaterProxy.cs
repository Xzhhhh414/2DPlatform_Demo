namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	/// <summary>
	/// Updater proxy.
	/// Replace Unity Update() with custom one without reflection.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/integration/updater.html")]
	public class UpdaterProxy : MonoBehaviourInitiable, IUpdaterProxy
	{
		/// <summary>
		/// Targets to run on update.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		protected class TargetsList<T>
		{
			readonly LinkedHashSet<T> targets = new LinkedHashSet<T>();

			readonly List<T> temp = new List<T>();

			/// <summary>
			/// Add.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Add(T target)
			{
				targets.Add(target);
			}

			/// <summary>
			/// Remove.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Remove(T target)
			{
				targets.Remove(target);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="TargetsOnceList{T}" />.
			/// </summary>
			/// <returns>A <see cref="List{T}.Enumerator" /> for the <see cref="TargetsOnceList{T}" />.</returns>
			public List<T>.Enumerator GetEnumerator()
			{
				temp.AddRange(targets);

				return temp.GetEnumerator();
			}

			/// <summary>
			/// Cleanup.
			/// </summary>
			public void Cleanup()
			{
				temp.Clear();
			}

			/// <summary>
			/// Clear.
			/// </summary>
			public void Clear()
			{
				temp.Clear();
				targets.Clear();
			}
		}

		/// <summary>
		/// Targets to run once on nearest update.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		protected class TargetsOnceList<T>
		{
			readonly List<T> targets = new List<T>();

			readonly List<T> temp = new List<T>();

			/// <summary>
			/// Add.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Add(T target)
			{
				targets.Add(target);
			}

			/// <summary>
			/// Remove.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Remove(T target)
			{
				targets.Remove(target);
			}

			/// <summary>
			/// Add range.
			/// </summary>
			/// <param name="list">List.</param>
			public void AddRange(List<T> list)
			{
				targets.AddRange(list);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="TargetsOnceList{T}" />.
			/// </summary>
			/// <returns>A <see cref="List{T}.Enumerator" /> for the <see cref="TargetsOnceList{T}" />.</returns>
			public List<T>.Enumerator GetEnumerator()
			{
				temp.AddRange(targets);
				targets.Clear();

				return temp.GetEnumerator();
			}

			/// <summary>
			/// Cleanup.
			/// </summary>
			public void Cleanup()
			{
				temp.Clear();
			}

			/// <summary>
			/// Clear.
			/// </summary>
			public void Clear()
			{
				temp.Clear();
				targets.Clear();
			}
		}

		/// <summary>
		/// Targets to run on next frame.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		protected class TargetsNextList<T>
		{
			/// <summary>
			/// Next frame number.
			/// </summary>
			public int Frame;

			readonly List<T> targets = new List<T>();

			readonly TargetsOnceList<T> once;

			/// <summary>
			/// Initializes a new instance of the <see cref="TargetsNextList{T}"/> class.
			/// </summary>
			/// <param name="once">Targets.</param>
			public TargetsNextList(TargetsOnceList<T> once)
			{
				this.once = once;
				Frame = UtilitiesTime.GetFrameCount() + 1;
			}

			/// <summary>
			/// Add.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Add(T target)
			{
				Check();

				targets.Add(target);
			}

			/// <summary>
			/// Remove.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Remove(T target)
			{
				once.Remove(target);
				targets.Remove(target);
			}

			/// <summary>
			/// Check.
			/// </summary>
			public void Check()
			{
				var current_frame = UtilitiesTime.GetFrameCount();
				if ((Frame <= current_frame) && (targets.Count > 0))
				{
					once.AddRange(targets);
					targets.Clear();
					Frame = current_frame + 1;
				}
			}

			/// <summary>
			/// Cleanup.
			/// </summary>
			public void Cleanup()
			{
				targets.Clear();
			}

			/// <summary>
			/// Clear.
			/// </summary>
			public void Clear()
			{
				Frame = 0;
				once.Clear();
				targets.Clear();
			}
		}

		/// <summary>
		/// Actions with owner.
		/// </summary>
		protected struct OwnerAction : IEquatable<OwnerAction>
		{
			/// <summary>
			/// Owner.
			/// </summary>
			public readonly UnityEngine.Object Owner;

			/// <summary>
			/// Action.
			/// </summary>
			public readonly Action Action;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="action">Action.</param>
			public OwnerAction(UnityEngine.Object owner, Action action)
			{
				Owner = owner;
				Action = action;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="action">Action.</param>
			public OwnerAction(Action action)
			{
				Owner = null;
				Action = action;
			}

			/// <summary>
			/// Invoke action.
			/// </summary>
			public void Invoke()
			{
				if (!Utilities.IsNull(Owner))
				{
					Action();
				}
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly override bool Equals(object obj) => (obj is OwnerAction color) && Equals(color);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public readonly bool Equals(OwnerAction other) => Action == other.Action;

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode() => Action.GetHashCode();
		}

		/// <summary>
		/// Targets to run on update.
		/// </summary>
		protected TargetsList<IUpdatable> TargetsBase = new TargetsList<IUpdatable>();

		/// <summary>
		/// Targets to run on late update.
		/// </summary>
		protected TargetsList<ILateUpdatable> TargetsLate = new TargetsList<ILateUpdatable>();

		/// <summary>
		/// Targets to run on fixed update.
		/// </summary>
		protected TargetsList<IFixedUpdatable> TargetsFixed = new TargetsList<IFixedUpdatable>();

		/// <summary>
		/// Targets to run on nearest update.
		/// </summary>
		protected TargetsOnceList<IUpdatable> TargetsOnce = new TargetsOnceList<IUpdatable>();

		/// <summary>
		/// Actions to run on nearest update.
		/// </summary>
		protected TargetsOnceList<OwnerAction> ActionsOnce = new TargetsOnceList<OwnerAction>();

		/// <summary>
		/// Targets to run on next frame.
		/// </summary>
		protected TargetsNextList<IUpdatable> TargetsOnceNext;

		/// <summary>
		/// Actions to run on next frame.
		/// </summary>
		protected TargetsNextList<OwnerAction> ActionsOnceNext;

		static UpdaterProxy instance;

		[SerializeField]
		[HideInInspector]
		bool destroyWithGameObject;

		static UpdaterProxy GetInstance()
		{
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (!scene.IsValid() || !scene.isLoaded)
				{
					continue;
				}

				foreach (var go in scene.GetRootGameObjects())
				{
					if (go.TryGetComponent<UpdaterProxy>(out var updater))
					{
						updater.Init();
						return updater;
					}
				}
			}

			var updater_go = new GameObject("New UI Widgets Updater Proxy");
			var instance = updater_go.AddComponent<UpdaterProxy>();
			instance.destroyWithGameObject = true;

			return instance;
		}

		/// <summary>
		/// Proxy destroyed.
		/// </summary>
		[field: DomainReloadExclude]
		public static bool Destroyed
		{
			get;
			protected set;
		}

		/// <summary>
		/// Proxy instance.
		/// </summary>
		public static UpdaterProxy Instance
		{
			get
			{
				if (instance == null)
				{
					instance = GetInstance();
				}

				return instance;
			}

			protected set
			{
				instance = value;

				if (instance != null)
				{
					instance.Init();

					if (!Application.isEditor)
					{
						DontDestroyOnLoad(instance);
					}
				}
			}
		}

		#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(instance))]
		static void StaticInit()
		{
			Destroyed = false;

			if (instance != null)
			{
				instance.Clear();
				instance = null;
			}
		}
		#endif

		/// <summary>
		/// Process the awake event.
		/// </summary>
		protected virtual void Awake()
		{
			if ((instance != null) && (instance.GetInstanceID() != GetInstanceID()))
			{
				if (destroyWithGameObject)
				{
					Destroy(gameObject);
				}
				else
				{
					Destroy(this);
				}
			}
			else
			{
				Instance = this;
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			TargetsOnceNext ??= new TargetsNextList<IUpdatable>(TargetsOnce);
			ActionsOnceNext ??= new TargetsNextList<OwnerAction>(ActionsOnce);
		}

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Clear();

			if ((instance != null) && (instance.GetInstanceID() == GetInstanceID()))
			{
				Destroyed = true;
				Updater.Proxy = null;
				instance.Clear();
				instance = null;
			}
		}

		/// <inheritdoc/>
		public void Clear()
		{
			TargetsBase.Clear();
			TargetsLate.Clear();
			TargetsFixed.Clear();

			TargetsOnce.Clear();
			TargetsOnceNext?.Clear();
			TargetsOnceNext = null;

			ActionsOnce.Clear();
			ActionsOnceNext?.Clear();
			ActionsOnceNext = null;
		}

		/// <inheritdoc/>
		public void Add(IUpdatable target)
		{
			TargetsBase.Add(target);
		}

		/// <inheritdoc/>
		public void Remove(IUpdatable target)
		{
			TargetsBase.Remove(target);
		}

		/// <inheritdoc/>
		public void AddLateUpdate(ILateUpdatable target)
		{
			TargetsLate.Add(target);
		}

		/// <inheritdoc/>
		public void RemoveLateUpdate(ILateUpdatable target)
		{
			TargetsLate.Remove(target);
		}

		/// <inheritdoc/>
		[Obsolete("Renamed to AddLateUpdate()")]
		public void LateUpdateAdd(ILateUpdatable target)
		{
			TargetsLate.Add(target);
		}

		/// <inheritdoc/>
		[Obsolete("Renamed to RemoveLateUpdate()")]
		public void LateUpdateRemove(ILateUpdatable target)
		{
			TargetsLate.Remove(target);
		}

		/// <inheritdoc/>
		public void AddFixedUpdate(IFixedUpdatable target)
		{
			TargetsFixed.Add(target);
		}

		/// <inheritdoc/>
		public void RemoveFixedUpdate(IFixedUpdatable target)
		{
			TargetsFixed.Remove(target);
		}

		/// <inheritdoc/>
		public void RunOnce(IUpdatable target)
		{
			TargetsOnce.Add(target);
		}

		/// <inheritdoc/>
		public void RunOnce(UnityEngine.Object owner, Action action)
		{
			ActionsOnce.Add(new OwnerAction(owner, action));
		}

		/// <inheritdoc/>
		public void RemoveRunOnce(IUpdatable target)
		{
			TargetsOnce.Remove(target);
			TargetsOnceNext.Remove(target);
		}

		/// <inheritdoc/>
		public void RemoveRunOnce(Action action)
		{
			ActionsOnce.Remove(new OwnerAction(action));
			ActionsOnceNext.Remove(new OwnerAction(action));
		}

		/// <inheritdoc/>
		public void RunOnceNextFrame(IUpdatable target)
		{
			TargetsOnceNext.Add(target);
		}

		/// <inheritdoc/>
		public void RunOnceNextFrame(UnityEngine.Object owner, Action action)
		{
			ActionsOnceNext.Add(new OwnerAction(owner, action));
		}

		/// <inheritdoc/>
		public void RemoveRunOnceNextFrame(IUpdatable target)
		{
			TargetsOnceNext.Remove(target);
		}

		/// <inheritdoc/>
		public void RemoveRunOnceNextFrame(Action action)
		{
			ActionsOnceNext.Remove(new OwnerAction(action));
		}

		/// <summary>
		/// Process update.
		/// </summary>
		protected virtual void Update()
		{
			RunOnceTargets();
			RunOnceActions();
			RunTargetsBase();
		}

		/// <summary>
		/// Process late update.
		/// </summary>
		protected virtual void LateUpdate()
		{
			RunTargetsLate();
		}

		/// <summary>
		/// Process fixed update.
		/// </summary>
		protected virtual void FixedUpdate()
		{
			RunTargetsFixed();
		}

		/// <summary>
		/// Run onceTargets update.
		/// </summary>
		protected virtual void RunOnceTargets()
		{
			TargetsOnceNext.Check();

			foreach (var target in TargetsOnce)
			{
				if (!Utilities.IsNull(target))
				{
					target.RunUpdate();
				}
			}

			TargetsOnce.Cleanup();
		}

		/// <summary>
		/// Run onceActions.
		/// </summary>
		protected virtual void RunOnceActions()
		{
			ActionsOnceNext.Check();

			foreach (var action in ActionsOnce)
			{
				action.Invoke();
			}

			ActionsOnce.Cleanup();
		}

		/// <summary>
		/// Run targets update.
		/// </summary>
		protected virtual void RunTargetsBase()
		{
			foreach (var target in TargetsBase)
			{
				target.RunUpdate();
			}

			TargetsBase.Cleanup();
		}

		/// <summary>
		/// Run targets LateUpdate.
		/// </summary>
		protected virtual void RunTargetsLate()
		{
			foreach (var target in TargetsLate)
			{
				target.RunLateUpdate();
			}

			TargetsLate.Cleanup();
		}

		/// <summary>
		/// Run targets FixedUpdate.
		/// </summary>
		protected virtual void RunTargetsFixed()
		{
			foreach (var target in TargetsFixed)
			{
				target.RunFixedUpdate();
			}

			TargetsFixed.Cleanup();
		}
	}
}