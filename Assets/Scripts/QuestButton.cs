using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class QuestButton : MonoBehaviour
{
    public string abrev;
    public int bestScore;
    int id; 
    public Text abreviationText;
    int stars = 0;
    public Image star1,star2,star3;

    public Sprite emptyStar, halfStar, fullStar;

    public GameObject manager;
    public AudioManager am;

    void Start()
    {
        manager = GameObject.Find("Manager");
        am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

    }

    public void CreateButton(int _id, string _abrev, int _score)
    {
        abrev = _abrev;
        bestScore = _score;
        id = _id;
        abreviationText.text = abrev;
        CalculateStars();
        //Debug.Log("Creating button");
    }
    
    public void SelectQuestionary()
    {
        manager.GetComponent<DisciplinesMenuManager>().SelectQuestionary(id);
        am.PlayButtonSFX(0);
    }

    public void CalculateStars()
    {
        if(bestScore == 100)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = fullStar;
        }
        else if(bestScore >= 80)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = halfStar;
        }
        else if(bestScore >= 60)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = emptyStar;
        }
        else if(bestScore >= 50)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = halfStar;
            star3.sprite = emptyStar;
        }
        else if(bestScore >= 25)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = emptyStar;
            star3.sprite = emptyStar;
        }
        else if(bestScore > 0)
        {
            stars = 3;
            star1.sprite = halfStar;
            star2.sprite = emptyStar;
            star3.sprite = emptyStar;
        }
        else
        {
            stars = 0;
            star1.sprite = emptyStar;
            star2.sprite = emptyStar;
            star3.sprite = emptyStar;
        }
    }
}
