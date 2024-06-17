using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelIndicator : MonoBehaviour
{
    public static LevelIndicator instance;

    private TMP_Text levelText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        levelText = GetComponent<TMP_Text>();
        UpdateLevelText();
    }


    public void UpdateLevelText()
    {
        int currentLevel = 0;

        if (SceneManager.GetActiveScene().name.StartsWith("Level1"))
        {
            currentLevel = 1;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level2"))
        {
            currentLevel = 2;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level3"))
        {
            currentLevel = 3;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level4"))
        {
            currentLevel = 4;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level5"))
        {
            currentLevel = 5;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level6"))
        {
            currentLevel = 6;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level7"))
        {
            currentLevel = 7;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level8"))
        {
            currentLevel = 8;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level9"))
        {
            currentLevel = 9;
        }
        else if (SceneManager.GetActiveScene().name.StartsWith("Level10"))
        {
            currentLevel = 10;
        }

        levelText.text = "Level: " + currentLevel;
    }
}

