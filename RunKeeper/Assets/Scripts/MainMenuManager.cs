using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] AudioSource audio;
    [SerializeField] List<AudioClip> menuAudioFx;

    public void StartSingleGame()
    {
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BtnMouseOver()
    {
        audio.PlayOneShot(menuAudioFx[0]);
    }

    public void BtnMouseClick()
    {
        audio.PlayOneShot(menuAudioFx[1]);
    }
}
