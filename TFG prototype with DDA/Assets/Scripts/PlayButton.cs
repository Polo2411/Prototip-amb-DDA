using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void PlayGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Level1n");
    }
}
