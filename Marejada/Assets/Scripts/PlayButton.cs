using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene("BlockoutOasis_Replaced");
    }
}