#if UNITY_EDITOR
using UnityEngine;

public class BattleTestManager
{
    private static BattleTestManager _instance;
    public static BattleTestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BattleTestManager();
            }
            return _instance;
        }
    }
    #region 游戏时间控制
    public float TimeScale = 0.2f;

    public void GMTimeScale()
    {
        if (Time.timeScale != TimeScale)
        {
            Time.timeScale = TimeScale;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }
    #endregion

    #region 最小伤害控制
    public bool isMinDamage;
    #endregion

}
#endif
