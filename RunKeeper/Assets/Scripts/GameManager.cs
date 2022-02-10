using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager gmInstance;

    public float generalSpeed = 1.5f;
    public bool isGameOver = false;
    [SerializeField] Slider progressDistance;
    [SerializeField] TMP_Text cupsText;
    [SerializeField] TMP_Text adrenalineText;
    [SerializeField] Slider hp;
    [SerializeField] Slider stamina;
    [SerializeField] Slider ammo;
    [SerializeField] Slider magazines;
    [SerializeField] Texture2D cursorPoint;
    [SerializeField] GameObject statisticsPanel;
    [SerializeField] PlayerController pc;
    [SerializeField] TMP_Text statCups;
    [SerializeField] TMP_Text statDistance;
    [SerializeField] TMP_Text statTime;
    private float endDistance = 5000;
    private float currDistance = 0;
    private double startTimerValue = 300.00;
    public TMP_Text timer;

    public static GameManager Instance
    {
        get
        {
            if (gmInstance == null)
            {
                gmInstance = GameObject.FindObjectOfType<GameManager>();
            }

            return gmInstance;
        }
    }

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        //Cursor.SetCursor(cursorPoint, new Vector2(0.5f,0.5f), CursorMode.ForceSoftware);
    }

    public void GameOver()
    {
        generalSpeed = 0;
        isGameOver = true;
        ShowStatistics();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        Timer();
        ProgressDistance();

    }

    void ProgressDistance()
    {
        if (!isGameOver)
        {
            if (endDistance > currDistance)
            {
                if (pc.isSpeedUp)
                {
                    currDistance += Time.deltaTime * 10 * pc.speedUp;
                }
                else
                {
                    currDistance += Time.deltaTime * 10;
                }

                progressDistance.value = currDistance;
            }
        }
    }

    void Timer()
    {
        if (!isGameOver)
        {
            if (startTimerValue > 0)
            {
                startTimerValue -= Time.deltaTime;
                var ts = TimeSpan.FromSeconds(startTimerValue);
                timer.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            }
        }
    }

    private void ShowStatistics()
    {
        statisticsPanel.SetActive(true);
        statisticsPanel.GetComponent<Animator>().SetTrigger("isStatistics");
        statDistance.text = Mathf.CeilToInt(currDistance).ToString() + " �.";
        var ts = TimeSpan.FromSeconds(startTimerValue);
        statTime.text = string.Format("{0:00}:{1:00} ���.", ts.Minutes, ts.Seconds);
        statCups.text = pc.cupsValue.ToString();
    }

    public void RestartLevel()
    {
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }


    public void SetCups(int value)
    {
        cupsText.text = value.ToString();
    }

    public void SetStamina(float value)
    {
        stamina.value = value;
    }

    public void SetAmmo(int value)
    {
        ammo.value = value;
    }

    public void SetMagazines(int value)
    {
        magazines.value = value;
    }

    public void SetAdrenaline(int value)
    {
        adrenalineText.text = value.ToString();
    }

    public void SetHP(int value)
    {
        hp.value = value;
    }
}