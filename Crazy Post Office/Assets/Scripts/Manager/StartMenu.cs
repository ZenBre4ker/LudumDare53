using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public void OnPressStartGame()
    {
        LevelManager.StartFirstScene();
    }
}
