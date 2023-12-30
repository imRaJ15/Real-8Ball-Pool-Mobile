using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Classes

    public void StartGame()
    {  SceneManager.LoadScene("Game");  }

    public void QuitGame()
    {  Application.Quit(); }

    #endregion
}
