using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVLselectButton : MonoBehaviour
{
    [Tooltip("the stars attached to the game object")]
    public GameObject[] stars;
    [Tooltip("represent the number of this lvl ")]
    public int levelNumber;

    /// <summary>
    /// loads the amount of stars the player got on that lvl
    /// </summary>
    private void Start()
    {
        if(PlayerPrefs.HasKey("LVL" + levelNumber))
        {
            for (int i = 0; i < PlayerPrefs.GetInt("LVL" + levelNumber); i++)
            {
                stars[i].SetActive(true);
            }
        } 
    }
}
