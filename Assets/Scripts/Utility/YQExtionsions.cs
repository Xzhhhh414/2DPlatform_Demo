using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class YQExtionsions
{
    //public static async void SetTriggerByTime(this Animator animator, string triggerName, float time)
    //{

    //    animator.SetTrigger(triggerName);
    //    await Task.Delay((int)(time * 1000));
    //    animator.ResetTrigger(triggerName);
    //}
    private static CancellationTokenSource cts = new CancellationTokenSource();

    public static async void SetTriggerByTime(this Animator animator, string triggerName, float time)
    {
        cts.Cancel(); // 取消前一个操作
        cts = new CancellationTokenSource(); // 创建新的 CancellationTokenSource

        animator.SetTrigger(triggerName);
        try
        {
            await Task.Delay((int)(time * 1000), cts.Token);
        }
        catch (TaskCanceledException)
        {
            // 如果任务被取消，就不需要重置触发器
            return;
        }
        animator.ResetTrigger(triggerName);
    }

    public static void TriggerEvent(this object sender, CustomEventType type)
    {
        EventManager.Instance.TriggerEvent(type);
    }
}
