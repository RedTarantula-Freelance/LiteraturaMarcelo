using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip playButton;
    public AudioClip commonButton;
    public AudioClip returnButton;
    
    public AudioClip correctAnswer;
    public AudioClip wrongAnswer;
    
    public AudioClip barFilling;
    public AudioClip star1;
    public AudioClip star2;
    public AudioClip star3;

    public AudioClip music;

    public AudioMixer mixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource riserSource;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);



        if(instance == null)
        {
            instance = this;
            gameObject.tag = "AudioManager";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void PlayButtonSFX(int id)
    {
        AudioClip clip = commonButton;
        switch (id)
        {
            case 0:
                clip = commonButton;
                break;
            case 1:
                clip = playButton;
                break;
            case 2:
                clip = returnButton;
                break;
            case 3:
                clip = wrongAnswer;
                break;
            case 4:
                clip = correctAnswer;
                break;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayBarFilling()
    {
        riserSource.PlayOneShot(barFilling);
    }

    public void StopBarFilling()
    {
        riserSource.Stop();
    }

    public void PlayStar(int id)
    {
        AudioClip clip = star1;
        switch (id)
        {
            case 0:
                clip = star1;
                break;
            case 1:
                clip = star2;
                break;
            case 2:
                clip = star3;
                break;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayAnswer(bool correct)
    {
        if(correct)
        {
            sfxSource.PlayOneShot(correctAnswer);
        }
        else
        { 
            sfxSource.PlayOneShot(wrongAnswer);
        }
    }

    public void SetVolumeFloat(float sliderValue,string parameter)
    {
        mixer.SetFloat(parameter,Mathf.Log10(sliderValue) * 20);
    }
    public float GetVolumeFloat(string parameter)
    {
        mixer.GetFloat(parameter,out float f);
        f /= 20f;
        f = Mathf.Pow(10,f);
        return f;

    }
}
