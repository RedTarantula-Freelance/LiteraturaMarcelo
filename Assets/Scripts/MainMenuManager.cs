using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public AudioManager am;


    private void Start()
    {
        am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetFloat("MusicVolume"));
    }

    public void PlayButtonSFX(int id)
    {
        am.PlayButtonSFX(id);
    }
    
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume",volume);
        if(am != null)
        am.SetVolumeFloat(volume,"Music");
    }
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume",volume);
        if(am != null)
        am.SetVolumeFloat(volume,"SFX");
        //am.PlayAnswer(true);
    }

    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene,LoadSceneMode.Single);
    }

    public void ChooseDiscipline(int disciplineID)
    {
        MenuVariables.selectedDiscipline = disciplineID;
        FilePaths.selectedDisciplinePath = FilePaths.questionariesPath + PathHelper.DisciplineIdToPath(disciplineID) + "/";
    }

    public void ClearProgress()
    {
        MenuVariables.RemoveAllProgress();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    }

    public void MailSupport()
    {
        Application.OpenURL("mailto:lunyx@lunyxstudios.com" + "?subject=Aplicativo EM Quiz");
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}