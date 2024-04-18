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

    public void GMTimeScale(float scale = 0.1f)
    {
        Time.timeScale = scale;
    }

}
