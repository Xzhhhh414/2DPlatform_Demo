using System.Collections.Generic;
using System;

public enum CustomEventType
{
    ReHit,
    SpellSkill01,
    SpellSkill02,
    SpellSkill03,
    Skill03ClearCDSuccess,
    Skill03ClearCooldown,
    InteractObjectIn, //进入场景物件交互范围
    InteractObjectOut, //离开场景物件交互范围
    AttemptInteractObject, //玩家按下场景交互输入
    ResourceMoneyAdd, //玩家获得金币

}
public class EventManager : Singleton<EventManager>
{
    interface IEventInfo { void Destory(); }
    private class EventInfo : IEventInfo
    {
        public Action action;
        public void Init(Action action)
        {
            this.action = action;
        }
        public void Destory()
        {
            action = null;
        }
    }
    private class EventInfo<T> : IEventInfo
    {
        public Action<T> action;
        public void Init(Action<T> action)
        {
            this.action = action;
        }
        public void Destory()
        {
            this.action -= action;
        }
    }
    private class EventInfo<T, K> : IEventInfo
    {
        public Action<T, K> action;
        public void Init(Action<T, K> action)
        {
            this.action = action;
        }
        public void Destory()
        {
            this.action -= action;
        }
    }
    private class EventInfo<T, K, L> : IEventInfo
    {
        public Action<T, K, L> action;
        public void Init(Action<T, K, L> action)
        {
            this.action = action;
        }
        public void Destory()
        {
            this.action -= action;
        }
    }
    Dictionary<CustomEventType, IEventInfo> eventDic = new();

    #region 添加事件监听
    public void AddListener(CustomEventType type, Action action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo).action += action;
        }
        else
        {
            var eventInfo = new EventInfo();
            eventInfo.Init(action);
            eventDic.Add(type, eventInfo);
        }
    }
    public void AddListener<T>(CustomEventType type, Action<T> action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T>).action += action;
        }
        else
        {
            var eventInfo = new EventInfo<T>();
            eventInfo.Init(action);
            eventDic.Add(type, eventInfo);
        }
    }
    public void AddListener<T, K>(CustomEventType type, Action<T, K> action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T, K>).action += action;
        }
        else
        {
            var eventInfo = new EventInfo<T, K>();
            eventInfo.Init(action);
            eventDic.Add(type, eventInfo);
        }
    }
    public void AddListener<T, K, L>(CustomEventType type, Action<T, K, L> action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T, K, L>).action += action;
        }
        else
        {
            var eventInfo = new EventInfo<T, K, L>();
            eventInfo.Init(action);
            eventDic.Add(type, eventInfo);
        }
    }
    #endregion

    #region 移除事件监听
    public void RemoveListener(CustomEventType type, Action action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo).action -= action;
        }
    }
    public void RemoveListener<T>(CustomEventType type, Action<T> action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T>).action -= action;
        }
    }
    public void RemoveListener<T, K>(CustomEventType type, Action<T, K> action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T, K>).action -= action;
        }
    }
    public void RemoveListener<T, K, L>(CustomEventType type, Action<T, K, L> action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T, K, L>).action -= action;
        }
    }
    #endregion

    #region 触发事件
    public void TriggerEvent(CustomEventType type)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo).action?.Invoke();
        }
    }
    public void TriggerEvent<T>(CustomEventType type, T arg)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T>).action?.Invoke(arg);
        }
    }
    public void TriggerEvent<T, K>(CustomEventType type, T arg1, K arg2)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T, K>).action?.Invoke(arg1, arg2);
        }
    }
    public void TriggerEvent<T, K, L>(CustomEventType type, T arg1, K arg2, L arg3)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T, K, L>).action?.Invoke(arg1, arg2, arg3);
        }
    }
    #endregion

    #region 移除事件
    public void RemoveEvent(CustomEventType type)
    {
        if (eventDic.ContainsKey(type))
        {
            eventDic[type].Destory();
            eventDic.Remove(type);
        }
    }
    #endregion
}
