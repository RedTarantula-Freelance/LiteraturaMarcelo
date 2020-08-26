using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class DisciplinesMenuManager : MonoBehaviour
{
    public GameObject questionaryButtonPrefab;
    public GameObject questionaryButtonsPanel;
    Questionary selectedQuestionary;
    string jsonString;

    public Text questionaryTitleText;
    public Text questionaryDescriptionText;
    public Text timesAnsweredText;
    public GameObject inProgressText;
    public GameObject startFrom0Button;

    int stars = 0;
    public Image star1,star2,star3;
    public Sprite emptyStar, halfStar, fullStar;

    void Start()
    {
        if (GetQuestionaries())
        {
            foreach (Questionary q in MenuVariables.availableQuestionaries)
            {
                GameObject qbp = Instantiate(questionaryButtonPrefab);
                qbp.transform.SetParent(questionaryButtonsPanel.transform);
                qbp.transform.localScale = new Vector3(1f,1f);
                //Debug.Log("Transformed new button");
                QuestButton button = qbp.GetComponent<QuestButton>();
                button.CreateButton(q.id,q.abrev,q.bestscore);
                //Debug.Log("Finished with the componetn");
            }
        }
        else
        {
            Debug.LogError("Couldn't load questionaries");
        }
        SelectQuestionary(0);
    }

    bool GetQuestionaries()
    {
        string filepath = FilePaths.selectedDisciplinePath + PathHelper.DisciplineIdToPath(MenuVariables.selectedDiscipline) + "_questionaries.json";

        if (File.Exists(filepath))
        {
            jsonString = File.ReadAllText(filepath);
            MenuVariables.availableQuestionaries = JsonHelper.FromJson<Questionary>(jsonString);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GoToQuestionary(bool startFromBeginning)
    {
        FilePaths.selectedQuestionaryPath = FilePaths.selectedDisciplinePath + selectedQuestionary.file + ".json";
        MenuVariables.selectedQuestionaryObj = selectedQuestionary;

        if (startFromBeginning)
        { MenuVariables.selectedQuestionaryObj.lastAnsweredQuestion = 0; MenuVariables.selectedQuestionaryObj.correctAnswers = 0; }

        //Debug.Log("Trying to load file: " + FilePaths.selectedQuestionaryPath);
        if (File.Exists(FilePaths.selectedQuestionaryPath))
        {
            SceneManager.LoadScene("Questions",LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Couldn't load questionary file");
        }
    }

    public void SelectQuestionary(int id)
    {
        selectedQuestionary = MenuVariables.availableQuestionaries[id];
        UpdateInfoTexts();
    }

    void UpdateInfoTexts()
    {
        questionaryTitleText.text = selectedQuestionary.title;
        questionaryDescriptionText.text = selectedQuestionary.description;
        if (selectedQuestionary.lastAnsweredQuestion > 0)
        {
            inProgressText.SetActive(true);
            startFrom0Button.SetActive(true);
        }
        else
        {
            inProgressText.SetActive(false);
            startFrom0Button.SetActive(false);
        }
        timesAnsweredText.text = "Respondido " + selectedQuestionary.timesAnswered.ToString() + " vezes";
        GetStarsScore();
    }

    void GetStarsScore()
    {
        int bestScore = selectedQuestionary.bestscore;
        if (bestScore == 100)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = fullStar;
        }
        else if (bestScore >= 80)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = halfStar;
        }
        else if (bestScore >= 60)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = fullStar;
            star3.sprite = emptyStar;
        }
        else if (bestScore >= 50)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = halfStar;
            star3.sprite = emptyStar;
        }
        else if (bestScore >= 25)
        {
            stars = 3;
            star1.sprite = fullStar;
            star2.sprite = emptyStar;
            star3.sprite = emptyStar;
        }
        else if (bestScore > 0)
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