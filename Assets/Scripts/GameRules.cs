using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameRulesIntEvent : UnityEvent<int>
{

}

public class GameRules : MonoBehaviour
{
    public static GameRules instance;
    public int winSceneIndex = 1;
    public int loseSceneIndex = 2;
    public int ammoRemaining;
    public GameRulesIntEvent onAmmoChanged;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnAmmoAdded()
    {
        ammoRemaining++;
        onAmmoChanged.Invoke(ammoRemaining);
    }

    public void OnAmmoRemoved()
    {
        ammoRemaining--;
        onAmmoChanged.Invoke(ammoRemaining);
    }

    public void GameWin()
    {
        SceneManager.LoadScene(winSceneIndex);
    }

    public void GameLose()
    {
        SceneManager.LoadScene(loseSceneIndex);
    }
}
