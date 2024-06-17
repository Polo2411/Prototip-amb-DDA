using System.Collections;
using System.IO; // Necesario para trabajar con archivos
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;

    private int totalEnemies;
    private int enemiesKilled;
    private float totaltimelevels;

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
        UpdateLevelIndicator();
    }

    public void RegisterEnemy()
    {
        totalEnemies++;
        Debug.Log($"Total Enemies: {totalEnemies}");
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Enemies Killed: {enemiesKilled} / {totalEnemies}");
        CheckIfAllEnemiesKilled();
    }

    private void CheckIfAllEnemiesKilled()
    {
        Debug.Log($"Checking if all enemies are killed: {enemiesKilled} / {totalEnemies}");
        if (enemiesKilled >= totalEnemies)
        {
            DDAscript.instance.DetermineNextSceneOnLive(SceneManager.GetActiveScene().name);
            string nextSceneName = DDAscript.instance.nextSceneName;
            Debug.Log($"Scene to load: {nextSceneName}");
            LoadNextLevel(nextSceneName);
        }
    }

    public void LoadNextLevel(string nextSceneName)
    {
        Debug.Log($"Loading next level: {nextSceneName}");
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            PlayerPrefs.SetInt("PlayerLives", player.GetCurrentLives());
            PlayerPrefs.SetInt("Deaths", player.deathCounter);
            PlayerPrefs.SetInt("Hits", player.hitsCounter);
            PlayerPrefs.SetInt("Dash", player.dashCounter);
            PlayerPrefs.SetInt("Charged", player.chargedCounter);
            PlayerPrefs.Save();
            totaltimelevels += player.timeLevel;
            Debug.Log($"Total time: {totaltimelevels}");
        }
        if(nextSceneName == "Menú")
        {
            ReturnToMenu();
        }
        else if (SceneManager.GetSceneByName(nextSceneName) != null)
        {
            StartCoroutine(LoadNextLevelWithDelay(nextSceneName));
        }
        else
        {
            Debug.LogError($"Scene '{nextSceneName}' not found!");
        }
    }

    private IEnumerator LoadNextLevelWithDelay(string nextSceneName)
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene(nextSceneName);
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        UpdateLevelIndicator();
    }

    private void UpdateLevelIndicator()
    {
        if (LevelIndicator.instance != null)
        {
            LevelIndicator.instance.UpdateLevelText();
        }
    }

    public void ResetGame()
    {
        StartCoroutine(ResetGameCoroutine());
    }

    private IEnumerator ResetGameCoroutine()
    {
        DDAscript.instance.DetermineNextSceneOnDeath(SceneManager.GetActiveScene().name);
        string currentSceneName = DDAscript.instance.nextSceneName;
        Debug.Log($"Scene to load: {currentSceneName}");
        Player player = FindObjectOfType<Player>();
        PlayerPrefs.SetInt("Deaths", player.deathCounter);
        PlayerPrefs.SetInt("Hits", player.hitsCounter);
        PlayerPrefs.SetInt("Dash", player.dashCounter);
        PlayerPrefs.SetInt("Charged", player.chargedCounter);
        PlayerPrefs.Save();
        totaltimelevels += player.timeLevel;
        Time.timeScale = 0f;
        SceneManager.LoadScene(currentSceneName);
        yield return new WaitForSecondsRealtime(3f); // Esperar un momento para asegurarse de que la escena se haya cargado
        ResetPlayerLives();
        Time.timeScale = 1f;
        ResetEnemyCounters();
        RegisterAllEnemies();
    }

    private void ResetPlayerLives()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetCurrentLives(3); // Establecer vidas a 3
            PlayerPrefs.SetInt("PlayerLives", 3);
            player.UpdateHealthUI();
            PlayerPrefs.Save();
        }
    }

    private void ResetEnemyCounters()
    {
        totalEnemies = 0;
        enemiesKilled = 0;
    }

    private void RegisterAllEnemies()
    {
        Ghost[] ghost = FindObjectsOfType<Ghost>();
        foreach (var enemy in ghost)
        {
            RegisterEnemy();
        }

        EyeBall[] eyeball = FindObjectsOfType<EyeBall>();
        foreach (var enemy in eyeball)
        {
            RegisterEnemy();
        }

        FireMonster[] firemonster = FindObjectsOfType<FireMonster>();
        foreach (var enemy in firemonster)
        {
            RegisterEnemy();
        }
    }

    public void ReturnToMenu()
    {
        SavePlayerStatistics();
        SceneManager.LoadScene("Menú"); 
    }

    private void SavePlayerStatistics()
    {
        Player player = FindObjectOfType<Player>();
        if (player == null) return;

        // Obtener la ruta de la carpeta de la build
        string buildFolder = Directory.GetParent(Application.dataPath).FullName;
        string filePath = Path.Combine(buildFolder, "PlayerStatisticsDDA.txt");

        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("Player Statistics DDA:");
            writer.WriteLine($"Last level: {SceneManager.GetActiveScene().name}");
            writer.WriteLine($"Deaths: {player.deathCounter}");
            writer.WriteLine($"Hits: {player.hitsCounter}");
            writer.WriteLine($"Dashes: {player.dashCounter}");
            writer.WriteLine($"Charged Projectiles: {player.chargedCounter}");
            writer.WriteLine($"Total time: {totaltimelevels} seconds");
        }

        Debug.Log($"Player statistics saved to {filePath}");
    }
}
