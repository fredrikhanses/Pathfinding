using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public void OnClick(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
