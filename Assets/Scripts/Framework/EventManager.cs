using System.Collections.Generic;
using System;

public enum EventType
{
    ReHit,
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
    Dictionary<EventType, IEventInfo> eventDic = new();

    #region 添加事件监听
    public void AddListener(EventType type, Action action)
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
    public void AddListener<T>(EventType type, Action<T> action)
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
    #endregion

    #region 移除事件监听
    public void RemoveListener(EventType type, Action action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo).action -= action;
        }
    }
    public void RemoveListener<T>(EventType type, Action<T> action)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T>).action -= action;
        }
    }
    #endregion

    #region 触发事件
    public void TriggerEvent(EventType type)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo).action?.Invoke();
        }
    }
    public void TriggerEvent<T>(EventType type, T arg)
    {
        if (eventDic.ContainsKey(type))
        {
            (eventDic[type] as EventInfo<T>).action?.Invoke(arg);
        }
    }
    #endregion

    #region 移除事件
    public void RemoveEvent(EventType type)
    {
        if (eventDic.ContainsKey(type))
        {
            eventDic[type].Destory();
            eventDic.Remove(type);
        }
    }
    #endregion


}
