using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Security.Cryptography;
using TMPro;

public class QuestionaryManagement : MonoBehaviour
{

    public struct QuestionStructure
    {
        public string[] texts;
        public string[] imgs;
        public bool startWithImg;
        public char[] order;

        public QuestionStructure(string[] _texts, string[] _imgs,char[] elementsOrder, bool startImage = false)
        {
            texts = _texts;
            imgs = _imgs;
            order = elementsOrder;
            startWithImg = startImage;
        }
    }

    Question[] questions;
    int questionsAmmount;
    public int rightAnswers = 0;
    int currentQuestion;
    string jsonString;

    public TextMeshProUGUI progressText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI questionText;

    public GameObject questionObject;
    public GameObject resultsObject;

    int grade;

    public Text resultsText;
    public Text gradeText;
    public Text bestGradeText;

    int steps;
    bool shouldFillStars = false;
    public Image star1,star2,star3;
    public Image optionA_bg, optionB_bg;
    public GameObject nextQuestionButton;
    public Color32 correctColor;
    public Color32 wrongColor;

    public ParticleSystem part_star1,part_star2,part_star3;

    public GameObject questionViewPort;
    public GameObject questionTextObj;
    public GameObject questionImageObj;
    public GameObject questionGapObj;

    public AudioManager am;

    public GameObject rightParticles;
    public GameObject wrongParticles;

    public Camera cam;

    private void Awake()
    {
        
        part_star1.Stop();
        part_star2.Stop();
        part_star3.Stop();
    }

    void Start()
    {
        jsonString = File.ReadAllText(FilePaths.selectedQuestionaryPath);
        questions = JsonHelper.FromJson<Question>(jsonString);
        currentQuestion = MenuVariables.selectedQuestionaryObj.lastAnsweredQuestion;
        questionsAmmount = questions.Length;
        titleText.text = MenuVariables.selectedQuestionaryObj.title;
        UpdateFields();

        star1.fillAmount = 0f;
        star2.fillAmount = 0f;
        star3.fillAmount = 0f;

        am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();


    }

    public void Results()
    {
        am.PlayBarFilling();
        questionObject.SetActive(false);
        resultsObject.SetActive(true);
        titleText.text = "Results";
        UpdateResults();
    }

    void UpdateResults()
    {
        resultsText.text = questionsAmmount.ToString() + " Perguntas\n" +
            rightAnswers.ToString() + " Acertadas\n" +
            (questionsAmmount - rightAnswers).ToString() + " Erradas\n";

        grade = (int)((float)rightAnswers / (float)questionsAmmount * 100f);
        gradeText.text = grade.ToString() + "%";

        if (grade > MenuVariables.selectedQuestionaryObj.bestscore)
        {
            bestGradeText.text = "NOVO MELHOR!";
            UpdateSave(grade,true);
        }
        else
        {
            bestGradeText.text = "Melhor: " + MenuVariables.selectedQuestionaryObj.bestscore.ToString() + "%";
            UpdateSave(grade,false);
        }
        GetStarsScore();
        shouldFillStars = true;
    }

    private void Update()
    {
        if(shouldFillStars)
        {
            shouldFillStars = !DealWithStars();
        }
    }

    public void Interrupt()
    {
        SaveProgress();
        SceneManager.LoadScene("QuestionarySelection",LoadSceneMode.Single);
    }

    void UpdateSave(int grade,bool bestScore)
    {
        if (bestScore)
        { MenuVariables.selectedQuestionaryObj.bestscore = grade; }

        float average = (float)(MenuVariables.selectedQuestionaryObj.averagescore * MenuVariables.selectedQuestionaryObj.timesAnswered);
        average += grade;
        MenuVariables.selectedQuestionaryObj.timesAnswered++;
        average /= MenuVariables.selectedQuestionaryObj.timesAnswered;
        MenuVariables.selectedQuestionaryObj.averagescore = (int)average;
        MenuVariables.selectedQuestionaryObj.lastAnsweredQuestion = 0;
        MenuVariables.selectedQuestionaryObj.correctAnswers = 0;
        MenuVariables.SaveQuestionary();

    }

    public void SaveProgress()
    {
        //Debug.Log("Saving Progress");
        MenuVariables.selectedQuestionaryObj.lastAnsweredQuestion = currentQuestion;
        //Debug.Log("Saving currentQuestion: " + currentQuestion.ToString());
        MenuVariables.selectedQuestionaryObj.correctAnswers = rightAnswers;
        //Debug.Log("Saving rightAnswers: " + rightAnswers.ToString());
        MenuVariables.SaveQuestionary();
        //Debug.Log("Finished Saving Progress");
    }

    public void Answer(string answer)
    {
        bool c = CheckIfAnsweredCorrectly(answer);

        optionA_bg.gameObject.SetActive(true);
        optionB_bg.gameObject.SetActive(true);
        nextQuestionButton.SetActive(true);


        if (c)
        {
            rightAnswers++;
            Instantiate(rightParticles,cam.ScreenToWorldPoint(Input.mousePosition),Quaternion.identity);
        }
        else
        {
            Instantiate(wrongParticles,cam.ScreenToWorldPoint(Input.mousePosition),Quaternion.identity);
        }

        am.PlayAnswer(c);
        


        switch (questions[currentQuestion].correctAnswer)
        {
            case "A":
                optionA_bg.color = correctColor;
                optionB_bg.color = wrongColor;
                break;
            case "B":
                optionA_bg.color = wrongColor;
                optionB_bg.color = correctColor;
                break;
        }


    }

    public void NextQuestion()
    {
        optionA_bg.gameObject.SetActive(false);
        optionB_bg.gameObject.SetActive(false);
        nextQuestionButton.SetActive(false);

        am.PlayButtonSFX(0);

        if (currentQuestion < questionsAmmount - 1)
        {
            currentQuestion++;
            UpdateFields();
        }
        else
        {
            Results();
        }
    }

    void UpdateFields()
    {

        foreach (Transform tf in questionViewPort.transform)
        {
            GameObject.Destroy(tf.gameObject);
        }

        progressText.text = "Pergunta " + (currentQuestion + 1).ToString() + " de " + questionsAmmount.ToString();
        BuildQuestion(questions[currentQuestion].question);
    }

    void BuildQuestion(string s)
    {
        CreateGap();
        QuestionStructure qs = FindImageTags(s);
        int textId = 0;
        int imgId = 0;
        foreach (char c in qs.order)
        {
            switch (c)
            {
                case 't': CreateText(qs.texts[textId]); textId++; break;
                case 'i': CreateImage(qs.imgs[imgId]); imgId++; break;
                default:
                    break;
            }
            CreateGap();
        }
        CreateAnswers();
            CreateGap();
    }

    void CreateAnswers()
    {
        string s = "Alternativas:\n\n";
        
        s += "A: " + questions[currentQuestion].answerA + "\n\n";
        s += "B: " + questions[currentQuestion].answerB + "\n";

        CreateText(s);
    }

    void CreateGap()
    {
        GameObject go = Instantiate(questionGapObj);
        go.transform.SetParent(questionViewPort.transform);
        go.transform.localScale = new Vector3(1f,1f);
    }

    void CreateText(string txt)
    {
        GameObject go = Instantiate(questionTextObj);
        TextMeshProUGUI t = go.GetComponent<TextMeshProUGUI>();
        t.text = txt;
        go.transform.SetParent(questionViewPort.transform);
        go.transform.localScale = new Vector3(1f, 1f);
    }

    void CreateImage(string img)
    {
        float viewPortWidth = questionViewPort.GetComponent<RectTransform>().rect.width - 40;
        float maxHeight = questionViewPort.GetComponent<RectTransform>().rect.height;


        GameObject go = Instantiate(questionImageObj);
        Image i = go.GetComponent<Image>();
        Sprite spr = Resources.Load<Sprite>(FilePaths.imagesPath+ img);

        float sizeRatio = 1f;

        sizeRatio = spr.rect.height / spr.rect.width;

        float newWidth = 1f;
        float newHeight = 1f;

        if(sizeRatio <= 1)
        {
            newWidth = viewPortWidth;
            newHeight = viewPortWidth * sizeRatio;
        }
        else
        {
            newHeight = viewPortWidth;
            newWidth = viewPortWidth * sizeRatio;
        }
        //Debug.Log("Ratio: " + sizeRatio);
        //Debug.Log("New Width x Height: " + newWidth + " - " + newHeight);
        

        i.overrideSprite = spr;


        go.transform.SetParent(questionViewPort.transform);
        go.transform.localScale = new Vector3(1f, 1f);

        go.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth,newHeight);

    }

    bool CheckIfAnsweredCorrectly(string answer)
    {
        if (questions[currentQuestion].correctAnswer == answer)
            return true;
        else
            return false;
    }

    void GetStarsScore()
    {
        if (grade == 100)
        {
            steps = 5;
        }
        else if (grade >= 80)
        {
            steps = 4;
        }
        else if (grade >= 60)
        {
            steps = 3;
        }
        else if (grade >= 50)
        {
            steps = 2;
        }
        else if (grade >= 25)
        {
            steps = 1;
        }
        else if (grade > 0)
        {
            steps = 0;
        }
        else
        {
            steps = -1;
        }



    }



    bool DealWithStars()
    {
        bool stop = false;
        float starFillSpeed = 2f * Time.deltaTime;

        if(steps <= 0)
        {
            stop = true;
        }

        if(steps > -1 && star1.fillAmount < .5f)
        {
            Debug.Log("Running through step 0");
            if(star1.fillAmount + starFillSpeed >= .5f)
            { 
            star1.fillAmount = .5f;
            }
            else
            {
            star1.fillAmount += starFillSpeed;
            }
        }
        else if (steps > 0 && star1.fillAmount < 1f)
        {
            Debug.Log("Running through step 1");
            if(star1.fillAmount + starFillSpeed >= 1f)
            { 
            star1.fillAmount = 1f;
                part_star1.Play();
                am.PlayStar(0);
            }
            else
            {
            star1.fillAmount += starFillSpeed;
            }
        }
        else if(steps > 1 && star2.fillAmount < .5f)
        {
            Debug.Log("Running through step 2");
            if(star2.fillAmount + starFillSpeed >= .5f)
            { 
            star2.fillAmount = .5f;
            }
            else
            {
            star2.fillAmount += starFillSpeed;
            }
        }
        else if (steps > 2 && star2.fillAmount < 1f)
        {
            Debug.Log("Running through step 3");
           
            if(star2.fillAmount + starFillSpeed >= 1f)
            { 
            star2.fillAmount = 1f;
                part_star2.Play();
                am.PlayStar(1);
            }
            else
            {
            star2.fillAmount += starFillSpeed;
            }
        }
        else if(steps > 3 && star3.fillAmount < .5f)
        {
           
           
            Debug.Log("Running through step 4");
            if(star3.fillAmount + starFillSpeed >= .5f)
            { 
            star3.fillAmount = .5f;
            }
            else
            {
            star3.fillAmount += starFillSpeed;
            }
        }
        else if (steps > 4 && star3.fillAmount < 1f)
        { 
           
            Debug.Log("Running through step 5");
            if(star3.fillAmount + starFillSpeed >= 1f)
            { 
            star3.fillAmount = 1f;
                part_star3.Play();
                am.PlayStar(2);
            }
            else
            {
            star3.fillAmount += starFillSpeed;
            }
        }
        else
        {
            am.StopBarFilling();
            stop = true;
        }

        return stop;
    }



    QuestionStructure FindImageTags(string s)
    {
    List<string> questText = new List<string>();
    List<string> images = new List<string>();
        List<char> order = new List<char>();
        bool first = true;
        bool startWithImage = false;

        int startPos = s.IndexOf("[img=");
        int endPos = s.IndexOf("]");
        while (startPos >= 0 && endPos >= 0)
        {
            if(first) { if (startPos == 0) { startWithImage = true; } first = !first; }

            if (s.Substring(0, startPos) != "" && s.Substring(0, startPos) != " ")
            {
                questText.Add(s.Substring(0, startPos));
                order.Add('t');
            }
            images.Add(s.Substring(startPos+5, endPos - startPos-5));
            order.Add('i');

            s = s.Remove(0, endPos + 1);
            startPos = s.IndexOf("[img=");
            endPos = s.IndexOf("]");
            //Debug.Log("Finished a wwhile");
        }
        if (s != "")
        {
            questText.Add(s);
            s = s.Remove(0, s.ToCharArray().Length - 1);
            order.Add('t');
        }

        string[] textsArr = questText.ToArray();
        string[] imgsArr = images.ToArray();
        



        foreach (string str in textsArr)
        {
            if(str.ToCharArray()[0] == ' ')
            {
                str.Remove(0);
            }
        }
        return new QuestionStructure(textsArr, imgsArr, order.ToArray(), startWithImage);
        
    }
}

