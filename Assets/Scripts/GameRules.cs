using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameRulesIntEvent : UnityEvent<int>
{

}

public class GameRules : MonoBehaviour
{
    public static GameRules instance;
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
}
