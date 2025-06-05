using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMenuManager : MonoBehaviour
{
    
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
}
