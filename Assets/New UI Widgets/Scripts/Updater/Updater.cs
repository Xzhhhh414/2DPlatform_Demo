namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Updater.
	/// Replace Unity Update() with custom one without reflection.
	/// </summary>
	public static class Updater
	{
		static IUpdaterProxy proxy;

		/// <summary>
		/// Proxy to run Update().
		/// </summary>
		public static IUpdaterProxy Proxy
		{
			get
			{
				if (Utilities.IsNull(proxy) && !UpdaterProxy.Destroyed)
				{
					proxy = UpdaterProxy.Instance;
				}

				return proxy;
			}

			set => proxy = value;
		}

		#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(proxy))]
		static void StaticInit()
		{
			if (!Utilities.IsNull(proxy))
			{
				proxy.Clear();
				proxy = null;
			}
		}
		#endif

		/// <summary>
		/// Add target.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void Add(IUpdatable target)
		{
			var p = Proxy;
			if (!Utilities.IsNull(p))
			{
				p.Add(target);
			}
		}

		/// <summary>
		/// Remove target.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void Remove(IUpdatable target)
		{
			var p = proxy;
			if (!Utilities.IsNull(p))
			{
				p.Remove(target);
			}
		}

		/// <summary>
		/// Add target to LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void AddLateUpdate(ILateUpdatable target)
		{
			var p = Proxy;
			if (!Utilities.IsNull(p))
			{
				p.AddLateUpdate(target);
			}
		}

		/// <summary>
		/// Remove target from LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveLateUpdate(ILateUpdatable target)
		{
			var p = proxy;
			if (!Utilities.IsNull(p))
			{
				p.RemoveLateUpdate(target);
			}
		}

		/// <summary>
		/// Add target to FixedUpdate.
		/// </summary>
		/// <param name="target">target.</param>
		public static void AddFixedUpdate(IFixedUpdatable target)
		{
			var p = Proxy;
			if (!Utilities.IsNull(p))
			{
				p.AddFixedUpdate(target);
			}
		}

		/// <summary>
		/// Remove target from FixedUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveFixedUpdate(IFixedUpdatable target)
		{
			var p = proxy;
			if (!Utilities.IsNull(p))
			{
				p.RemoveFixedUpdate(target);
			}
		}

		/// <summary>
		/// Add target to run update only once.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RunOnce(IUpdatable target)
		{
			var p = Proxy;
			if (!Utilities.IsNull(p))
			{
				p.RunOnce(target);
			}
		}

		/// <summary>
		/// Add action to run only once.
		/// </summary>
		/// <param name="owner">Owner.</param>
		/// <param name="action">Action.</param>
		[Obsolete("Replaced with RunOnce(UnityEngine.Object owner, Action action).")]
		public static void RunOnce(Action action)
		{
			throw new NotSupportedException("This method replaced with RunOnce(UnityEngine.Object owner, Action action).");
		}

		/// <summary>
		/// Add action to run only once.
		/// </summary>
		/// <param name="owner">Owner.</param>
		/// <param name="action">Action.</param>
		public static void RunOnce(UnityEngine.Object owner, Action action)
		{
			var p = Proxy;
			if (!Utilities.IsNull(p))
			{
				p.RunOnce(owner, action);
			}
		}

		/// <summary>
		/// Remove target from run update only once.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveRunOnce(IUpdatable target)
		{
			var p = proxy;
			if (!Utilities.IsNull(p))
			{
				p.RemoveRunOnce(target);
			}
		}

		/// <summary>
		/// Remove action from run only once.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RemoveRunOnce(Action action)
		{
			var p = proxy;
			if (!Utilities.IsNull(p))
			{
				p.RemoveRunOnce(action);
			}
		}

		/// <summary>
		/// Add target to run update only once at next frame.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RunOnceNextFrame(IUpdatable target)
		{
			var p = Proxy;
			if (!Utilities.IsNull(p))
			{
				p.RunOnceNextFrame(target);
			}
		}

		/// <summary>
		/// Add action to run only once at next frame.
		/// </summary>
		/// <param name="owner">Owner.</param>
		/// <param name="action">Action.</param>
		[Obsolete("Replaced with RunOnceNextFrame(UnityEngine.Object owner, Action action).")]
		public static void RunOnceNextFrame(Action action)
		{
			throw new NotSupportedException("This method replaced with RunOnceNextFrame(UnityEngine.Object owner, Action action).");
		}

		/// <summary>
		/// Add action to run only once at next frame.
		/// </summary>
		/// <param name="owner">Owner.</param>
		/// <param name="action">Action.</param>
		public static void RunOnceNextFrame(UnityEngine.Object owner, Action action)
		{
			var p = Proxy;
			if (!Utilities.IsNull(p))
			{
				p.RunOnceNextFrame(owner, action);
			}
		}

		/// <summary>
		/// Remove target from run update only once at next frame.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveRunOnceNextFrame(IUpdatable target)
		{
			var p = proxy;
			if (!Utilities.IsNull(p))
			{
				p.RemoveRunOnceNextFrame(target);
			}
		}

		/// <summary>
		/// Remove action from run only once at next frame.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RemoveRunOnceNextFrame(Action action)
		{
			var p = proxy;
			if (!Utilities.IsNull(p))
			{
				p.RemoveRunOnceNextFrame(action);
			}
		}
	}
}