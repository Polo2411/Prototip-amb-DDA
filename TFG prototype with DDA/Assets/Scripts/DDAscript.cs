using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDAscript : MonoBehaviour
{
    public static DDAscript instance;
    public string nextSceneName; // Variable global que almacenará el nombre de la siguiente escena a cargar

    private int deathCounter;
    private int hitsCounter;
    private float timeLevel;
    private int dashCounter;
    private int chargedCounter;
    private Dictionary<string, float> levelHeuristics = new Dictionary<string, float>();
    private Dictionary<string, int> LevelDash = new Dictionary<string, int>();
    private Dictionary<string, int> LevelCharged = new Dictionary<string, int>();
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
        Thresholds();
    }

    private float CalculateHeuristic(int deathCounter, int hitsCounter, float timeLevel)
    {
        float heuristic = 0f;
        heuristic = (0.6f * deathCounter) + (0.3f * hitsCounter) + (0.1f * timeLevel/10);
        Debug.Log($"deathCounter: {deathCounter}");
        Debug.Log($"hitsCounter: {hitsCounter}");
        Debug.Log($"timeLevel: {timeLevel}");
        Debug.Log($"heuristic: {heuristic}");
        return heuristic;
    }

    public void DetermineNextSceneOnLive(string currentSceneName)
    {
        UpdatePlayerMetrics();

        // Calcular el heurístico del jugador
        float heuristic = CalculateHeuristic(deathCounter, hitsCounter, timeLevel);

        // Obtener el número de nivel actual
        int currentLevelNumber = GetLevelNumber(currentSceneName);

        // Verificar si es el último nivel
        if (currentLevelNumber == 10)
        {
            nextSceneName = "Menú";
            return;
        }

        // Determinar el nombre de la siguiente escena
        string heuristicSuffix = GetHeuristicSuffix(currentLevelNumber, heuristic);
        string performanceSuffix = GetPerformanceSuffix(currentLevelNumber, dashCounter, chargedCounter);
        nextSceneName = $"Level{currentLevelNumber + 1}{heuristicSuffix}{performanceSuffix}";
    }

    public void DetermineNextSceneOnDeath(string currentSceneName)
    {
        UpdatePlayerMetrics();

        // Calcular el heurístico del jugador
        float heuristic = CalculateHeuristic(deathCounter, hitsCounter, timeLevel);

        // Determinar el nombre de la siguiente escena
        string levelKey = GetLevelKey(currentSceneName);
        if (levelKey != null)
        {
            string heuristicSuffix = GetHeuristicSuffix(GetLevelNumber(levelKey), heuristic);
            string performanceSuffix = GetPerformanceSuffix(GetLevelNumber(levelKey), dashCounter, chargedCounter);
            nextSceneName = $"{levelKey}{heuristicSuffix}{performanceSuffix}";
        }
        Debug.Log($"Scene name death: {nextSceneName}");
    }

    private void UpdatePlayerMetrics()
    {
        Player player = FindObjectOfType<Player>();
        deathCounter = player.deathCounter;
        hitsCounter = player.hitsCounter;
        timeLevel = player.timeLevel;
        dashCounter = player.dashCounter;
        chargedCounter = player.chargedCounter;
    }

    private int GetLevelNumber(string sceneName)
    {
        string levelNumberString = new string(sceneName.SkipWhile(c => !char.IsDigit(c)).TakeWhile(char.IsDigit).ToArray());
        return int.Parse(levelNumberString);
    }

    private string GetLevelKey(string sceneName)
    {
        for (int i = 1; i <= 10; i++)
        {
            if (sceneName.StartsWith($"Level{i}"))
            {
                return $"Level{i}";
            }
        }
        return null;
    }

    private string GetHeuristicSuffix(int levelNumber, float heuristic)
    {
        string heuristicKeyGood = $"Level{levelNumber}good";
        string heuristicKeyBad = $"Level{levelNumber}bad";
        Debug.Log($"Levelgood: {levelHeuristics[heuristicKeyGood]}");
        Debug.Log($"Levelbad: {levelHeuristics[heuristicKeyBad]}");
        if (heuristic <= levelHeuristics[heuristicKeyGood])
        {
            return "d";
        }
        else if (heuristic > levelHeuristics[heuristicKeyGood] && heuristic < levelHeuristics[heuristicKeyBad])
        {
            return "n";
        }
        else
        {
            return "e";
        }
    }

    private string GetPerformanceSuffix(int levelNumber, int dashCounter, int chargedCounter)
    {
        string dashKey = $"dash{levelNumber}good";
        string chargedKey = $"charged{levelNumber}good";
        Debug.Log($"dashkey: {dashKey}");
        Debug.Log($"chargedkey: {chargedKey}");
        bool dashGood = dashCounter < LevelDash[dashKey];
        bool chargedGood = chargedCounter < LevelCharged[chargedKey];
        Debug.Log($"dashgood: {dashCounter}");
        Debug.Log($"chargedgood: {chargedCounter}");
        Debug.Log($"dashlevel: {LevelDash[dashKey]}");
        Debug.Log($"chargedlevel: {LevelCharged[chargedKey]}");
        if (dashGood && chargedGood)
        {
            return "b";
        }
        else if (dashGood)
        {
            return "d";
        }
        else if (chargedGood)
        {
            return "c";
        }
        else
        {
            return string.Empty;
        }
    }

    private void Thresholds()
    {
        // Definir los umbrales para cada nivel
        var heuristicThresholds = new (int deaths, int hits, float time, int dash, int charged)[]
        {
        (0, 0, 31f, 1, 2),  // Nivel 1
        (0, 0, 38f, 2, 4),  // Nivel 2
        (0, 0, 42f, 3, 5),  // Nivel 3
        (0, 1, 30f, 5, 5),  // Nivel 4
        (0, 2, 40f, 6, 6),  // Nivel 5
        (0, 3, 50f, 9, 9),  // Nivel 6
        (0, 4, 35f, 13, 10),  // Nivel 7
        (0, 5, 33f, 15, 10),  // Nivel 8
        (0, 6, 45f, 16, 12),  // Nivel 9
        (0, 7, 52f, 20, 14)   // Nivel 10
        };

        var heuristicBadThresholds = new (int deaths, int hits, float time)[]
        {
        (0, 2, 40f),  // Nivel 1
        (0, 2, 48f),  // Nivel 2
        (0, 2, 48f),  // Nivel 3
        (1, 3, 38f),  // Nivel 4
        (1, 4, 45f),  // Nivel 5
        (1, 5, 58f),  // Nivel 6
        (1, 6, 42f),  // Nivel 7
        (1, 7, 43f),  // Nivel 8
        (2, 8, 50f),  // Nivel 9
        (2, 9, 60f)   // Nivel 10
        };

        // Actualizar los heurísticos y los contadores para cada nivel
        for (int i = 0; i < heuristicThresholds.Length; i++)
        {
            int level = i + 1;

            // Calcular y asignar los heurísticos buenos y malos
            float levelGood = CalculateHeuristic(heuristicThresholds[i].deaths, heuristicThresholds[i].hits, heuristicThresholds[i].time);
            float levelBad = CalculateHeuristic(heuristicBadThresholds[i].deaths, heuristicBadThresholds[i].hits, heuristicBadThresholds[i].time);
            levelHeuristics[$"Level{level}good"] = levelGood;
            levelHeuristics[$"Level{level}bad"] = levelBad;

            // Asignar los valores de dash y charged
            LevelDash[$"dash{level}good"] = heuristicThresholds[i].dash;
            LevelCharged[$"charged{level}good"] = heuristicThresholds[i].charged;
        }
    }

}