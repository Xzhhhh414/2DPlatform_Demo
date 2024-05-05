using System.Threading.Tasks;
using UnityEngine;

public static class YQExtionsions
{
    public static async void SetTriggerByTime(this Animator animator, string triggerName, float time)
    {

        animator.SetTrigger(triggerName);
        await Task.Delay((int)(time * 1000));
        animator.ResetTrigger(triggerName);
    }
}
