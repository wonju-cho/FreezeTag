using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CellPhone_UI : MonoBehaviour
{
    public Image[] health_images;
    public Image[] npc_images;
    public Image gem_image;

    public Color npcFreeColor;
    
    public bool is_test_controller = true;
    private int gem_total_count;

    private bool is_end_the_game = false;
    private int gem_current_count;
    private int count_released_npc;


    public GameObject winScreen;

    private void Start()
    {
        gem_total_count = GameObject.FindGameObjectsWithTag("Gem").Length;
        gem_image.fillAmount = 0f;

        is_end_the_game = false;
        gem_current_count = 0;
        count_released_npc = 0;
    }

    private void Update()
    {
        if (is_end_the_game)
        {
            winScreen.SetActive(true);
        }
    }

    public void UpdateHealthCountText(int health)
    {
        Destroy(health_images[health]);
    }    

    public void UpdateGemCountText(int gem)
    {
        float amount = 1.0f / gem_total_count;
        amount += gem_image.fillAmount;
        
        gem_image.fillAmount =  amount;

        gem_current_count++;

        DoesGameEnd();
    }


    public void UpdateNPCCountText(int npc)
    {
        Color freeColorContainer = npcFreeColor;

        freeColorContainer.a = 1f;

        npc_images[npc - 1].color = freeColorContainer;

        count_released_npc++;
        
        DoesGameEnd();
    }

    public void DoesGameEnd()
    {

        if (count_released_npc == npc_images.Length)
        {
            if (gem_current_count == gem_total_count)
            {
                is_end_the_game = true;
            }
        }
    }

}
