
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool gameHasEnabled = false;
    public float restartDelay = 2f;
    public GameOverScreen gameoverScreenUI;
    
    public void EndGame()
    {
        if(gameHasEnabled == false)
        {
            Debug.Log("GameOver");
            gameHasEnabled = true;

            gameoverScreenUI.PopUpScreenUI(true);
            //Invoke("RestartGame", restartDelay);
        }
    }

}
