using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    private static GameManager gmInstance;

    public float generalSpeed = 1.5f;
    public float currGeneralSpeed;
    public bool isGameOver = false;
    public bool isPause = false;
    [SerializeField] Slider progressDistance;
    [SerializeField] TMP_Text cupsText;
    [SerializeField] TMP_Text adrenalineText;
    [SerializeField] Slider hp;
    [SerializeField] Slider stamina;
    [SerializeField] Slider ammo;
    [SerializeField] Slider magazines;
    [SerializeField] GameObject statisticsPanel;
    [SerializeField] PlayerController pc;
    [SerializeField] TMP_Text statCups;
    [SerializeField] TMP_Text statDistance;
    [SerializeField] TMP_Text statTime;
    [SerializeField] SpawnManager spManager;
    [SerializeField] Color nightLightColor;
    private List<Light2D> envLights;
    private float endDistance = 3000;
    private float currDistance = 0;
    private double startTimerValue = 300.00;
    private bool isNight = false;
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
        envLights = new List<Light2D>();
        currGeneralSpeed = generalSpeed;
        FindAllLights();
        CursorVisible(false);
    }

    private void FindAllLights()
    {
        GameObject[] lights = GameObject.FindGameObjectsWithTag("EnvironmentLights");
        for (int i = 0; i < lights.Length; i++)
        {
            envLights.Add(lights[i].GetComponent<Light2D>());
        }
    }

    public void GameOver()
    {
        generalSpeed = 0;
        isGameOver = true;
        ShowStatistics();
        CursorVisible(true);
    }

    private void Update()
    {
        if (!isPause && !isGameOver)
        {
            Timer();
            ProgressDistance();
            GeneralSpeedUp();
            ChangeDayNight();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    private void Pause()
    {
        if (!isPause)
        {
            isPause = true;
            generalSpeed = 0;
            CursorVisible(true);
            ShowStatistics();
            spManager.StartSpawns(false);
        }
        else
        {
            isPause = false;
            generalSpeed = currGeneralSpeed;
            CursorVisible(false);
            HideStatistics();
            spManager.StartSpawns(true);
        }
    }

    void CursorVisible(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void ProgressDistance()
    {
        if (!isGameOver)
        {
            if (endDistance > currDistance)
            {
                currDistance += (endDistance / (float)startTimerValue) * Time.deltaTime;
                progressDistance.value = currDistance;
            }
        }
    }

    void GeneralSpeedUp()
    {
        if (generalSpeed < 2.7f)
        {
            generalSpeed += Time.deltaTime / 300;
            currGeneralSpeed += Time.deltaTime / 300;
        }
    }

    private void ChangeDayNight()
    {
        if (!isNight && startTimerValue < 150)
        {
            for (int i = 0; i < envLights.Count; i++)
            {
                if ((envLights[i].intensity > 0 && envLights[i].lightType == Light2D.LightType.Freeform) || (envLights[i].intensity > 0.75f && envLights[i].lightType == Light2D.LightType.Global))
                {
                    envLights[i].intensity -= Time.deltaTime;
                    envLights[i].color = Color.Lerp(envLights[i].color, nightLightColor, Time.time / 1000);
                }
                /*else
                {
                    isNight = true;
                }*/
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
        statDistance.text = Mathf.CeilToInt(currDistance).ToString() + " м.";
        var ts = TimeSpan.FromSeconds(startTimerValue);
        statTime.text = string.Format("{0:00}:{1:00} мин.", ts.Minutes, ts.Seconds);
        statCups.text = pc.cupsValue.ToString();
    }

    private void HideStatistics()
    {
        statisticsPanel.SetActive(false);
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
