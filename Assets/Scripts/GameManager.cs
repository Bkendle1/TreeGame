using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Action LiveLost;

    public bool IsGamePaused => m_isGamePaused;

    private bool m_isGamePaused;
    private GameState m_gameState = GameState.Playing;
    
    
    public static GameManager Instance;
    public Vector2 lastCheckPointPos;
    private Vector2 startingPoint;
    
    private UIText m_expUI = null;
    [Tooltip("This is exposed in the Inspector for testing purposes.")]
    [SerializeField] private int m_exp;

    private int startingExp;
    [Tooltip("The rate at which the experience counter decrements.")]
    [SerializeField] private float decrementRate = .05f;
        
    public int GetCurrentExpAmount => m_exp;
    
    private UIText m_livesUI = null;
    [SerializeField] private int m_lives = 3;
    private int startingLives;

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

        startingExp = m_exp;
        startingLives = m_lives;
        startingPoint = lastCheckPointPos;
    }

    private void Start()
    {
        m_expUI = GameObject.Find("ExperienceCounter").GetComponent<UIText>();
        m_livesUI = GameObject.Find("LivesCounter").GetComponent<UIText>();
        
        m_livesUI.UpdateUI(m_lives);
        m_expUI.UpdateUI(m_exp);
    }

    private void Update()
    {
        if (m_expUI == null || m_livesUI == null)
        {
            if (SceneManager.GetActiveScene().name == "Level")
            {
                m_expUI = GameObject.Find("ExperienceCounter").GetComponent<UIText>();
                m_livesUI = GameObject.Find("LivesCounter").GetComponent<UIText>();
                
                //reset exp and lives
                m_exp = startingExp;
                m_lives = startingLives;
            
                //update ui
                m_livesUI.UpdateUI(m_lives);
                m_expUI.UpdateUI(m_exp);
            }
        }

        if (SceneManager.GetActiveScene().name != "Level")
        {
            //reset checkpoint to starting point
            lastCheckPointPos = startingPoint;
        }
    }

    public void UpdateExp(int value)
    {
        if (value < 0)
        {
            int targetNumber = m_exp + value;
            StartCoroutine(Decrement(targetNumber));
        }
        else
        {
            m_exp += value;
            m_expUI.UpdateUI(m_exp);
        }
        StopCoroutine("Decrement");
    }

    private IEnumerator Decrement(int targetNumber)
    {
        while (m_exp > targetNumber)
        {
            m_exp--;
            m_expUI.UpdateUI(m_exp);
            yield return new WaitForSeconds(decrementRate);
        }
    }
    public void UpdateLives(int value)
    {
        if (m_lives + value <= 0)
        {
            //Game Over
            m_lives = 0;

            //reset exp and lives
            m_exp = startingExp;
            m_lives = startingLives;
            
            //update ui
            m_livesUI.UpdateUI(m_lives);
            m_expUI.UpdateUI(m_exp);
            
            //reset checkpoint to starting point
            lastCheckPointPos = startingPoint;
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
