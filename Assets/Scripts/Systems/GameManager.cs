using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject gameWonPanel;
    private bool gameIsOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if(gameWonPanel != null)
        {
            gameWonPanel.SetActive(false);
        }
        
    }

    public void PlayerDied()
    {
        if (gameIsOver) return;
        gameIsOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void BossDied()
    {
        if (gameIsOver) return;
        gameIsOver = true;

        if (gameWonPanel != null)
        {
            gameWonPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}
