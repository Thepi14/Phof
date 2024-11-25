using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CutsceneManager : MonoBehaviour
{
    public Button startGame;
    // Start is called before the first frame update
    void Start()
    {
        startGame.onClick.AddListener( delegate { StartGame(); });
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

}
