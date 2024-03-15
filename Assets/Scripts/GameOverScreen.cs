using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public void PopUpScreenUI(bool is_popUP)
    {
        gameObject.SetActive(is_popUP);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CreditButton()
    {
        SceneManager.LoadScene("CreditScene");
    }
}
