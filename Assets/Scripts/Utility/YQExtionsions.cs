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
        cts.Cancel(); // ȡ��ǰһ������
        cts = new CancellationTokenSource(); // �����µ� CancellationTokenSource

        animator.SetTrigger(triggerName);
        try
        {
            await Task.Delay((int)(time * 1000), cts.Token);
        }
        catch (TaskCanceledException)
        {
            // �������ȡ�����Ͳ���Ҫ���ô�����
            return;
        }
        animator.ResetTrigger(triggerName);
    }

    public static void TriggerEvent(this object sender, CustomEventType type)
    {
        EventManager.Instance.TriggerEvent(type);
    }
}
