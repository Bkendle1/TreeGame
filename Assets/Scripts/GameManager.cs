using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Action GamePaused;
    public Action GameResumed;
    public Action LiveLost;
    public Action EndGame;

    public bool IsGamePaused => m_isGamePaused;

    private bool m_isGamePaused;
    private GameState m_gameState = GameState.Playing;
    
    
    public static GameManager Instance;
    public Vector2 lastCheckPointPos;

    [SerializeField] private UIText m_expUI = null;
    private int m_exp;
    
    [SerializeField] private UIText m_livesUI = null;
    [SerializeField] private int m_lives = 3;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        m_livesUI.UpdateUI(m_lives);
        m_expUI.UpdateUI(m_exp);
        Debug.Log(m_lives);
    }

    public void UpdateExp(int value)
    {
        m_exp += value;
        m_expUI.UpdateUI(m_exp);
    }
    
    public void UpdateLives(int value)
    {
        if (m_lives + value <= 0)
        {
            //Game Over
            m_lives = 0;
            SceneManager.LoadScene("GameOver");
        }

        if (value < 0)
        {
            //run methods subscribed to this action
            LiveLost?.Invoke();
        }
        
        m_lives += value;
        m_livesUI.UpdateUI(m_lives);
        
    }
}

public enum GameState
{
    Playing,
    GameOver,
    ClearLevel
}
