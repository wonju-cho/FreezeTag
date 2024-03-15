using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SplashScreen : MonoBehaviour
{
    public Image digipenImage;
    public TextMeshProUGUI teamText;
    public GameObject text;

    Color tempColor;
    Color teamColor;
    float alpha = 0;

    float time = 3f;
    float digipenTimer = 0f;
    float teamTimer = 0f;
    bool once = false;
    private bool digipenLogoEnd = false;


    // Start is called before the first frame update
    void Start()
    {
        tempColor = digipenImage.color;
        tempColor.a = 0f;
        digipenImage.color = tempColor;

        teamColor = teamText.color;
        teamColor.a = 0f;
        teamText.color = teamColor;
    }

    void ShowUI(Image image, Color color)
    {
        alpha += Time.deltaTime;
        color.a = alpha;
        image.color = color;
    }

    void ShowUI(TextMeshProUGUI image, Color color)
    {
        alpha += Time.deltaTime;
        color.a = alpha;
        image.color = color;
    }

    private void Update()
    {

        ShowUI(digipenImage, tempColor);
        digipenTimer += Time.deltaTime;
        if (digipenTimer > time)
        {
            digipenImage.enabled = false;
            digipenLogoEnd = true;
            text.SetActive(false);
        }

        if (digipenLogoEnd)
        {
            if (!once)
            {
                alpha = 0;
                once = true;
            }
            ShowUI(teamText, teamColor);
            teamTimer += Time.deltaTime;

            if (teamTimer > time)
            {
                SceneManager.LoadScene("SejeongScene");
            }
        }

    }

}