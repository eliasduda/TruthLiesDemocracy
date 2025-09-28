using UnityEngine;
using UnityEngine.InputSystem;

public class GameStatesManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject gameWonScreen;
    public GameObject pauseScreen;

    private InputAction pauseAction;

    GameManager manager;

    public static GameStatesManager instance;

    public static GameState gameState = GameState.Playing;

    private void Start()
    {
        pauseAction = InputSystem.actions.FindAction("Cancel");

        if (instance == null) instance = this;
        else Destroy(this);

        gameOverScreen.SetActive(false);
        gameWonScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    private void Update()
    {
        if (manager == null)
        {
            manager = GameManager.instance;
            manager.eventManager.OnStatChanged.AddListener(CheckConditions);
        }

        if (pauseAction.WasPressedThisFrame())
        {
            if (gameState == GameState.Playing) gameState = GameState.Paused;
            else if (gameState == GameState.Paused) gameState = GameState.Playing;
        }

        switch (gameState)
        {
            case GameState.Playing:
                Time.timeScale = 1;
                pauseScreen.SetActive(false);
                break;
            case GameState.Paused:
                PauseGame();
                break;
            case GameState.GameOver:
                GameLost();
                break;
            case GameState.Victory:
                GameWon();
                break;
        }
    }

    private void CheckConditions(InfluencableStats stat, float amount)

    {
        switch(stat)
        {
            case InfluencableStats.Signatures:
                if(amount >= GameManager.instance.gamePlaySettings.signatureGoal)
                {
                    gameState = GameState.Victory;
                }
                break;
            case InfluencableStats.DaysPassed:
                if(amount >= GameManager.instance.gamePlaySettings.daysTotal)
                {
                    gameState = GameState.GameOver;
                }
                break;
        }
    }

    public void PauseGame()
    {
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameLost()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }
    public void GameWon()
    {
        gameWonScreen.SetActive(true);
        Time.timeScale = 0;
    }
}
