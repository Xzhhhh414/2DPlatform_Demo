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
    public float TimeScale = 0.2f;

    public void GMTimeScale()
    {
        if(Time.timeScale != TimeScale)
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

}
