using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Question
{
    public string question;
    public string answerA;
    public string answerB;
    public string correctAnswer;
}

[System.Serializable]
public class Questionary
{
    public int id;
    public string title;
    public string description;
    public string abrev;
    public string file;
    public int lastAnsweredQuestion;
    public int correctAnswers;
    public int averagescore;
    public int bestscore;
    public int timesAnswered;
}

public static class MenuVariables
{
    public static int selectedDiscipline;
    public static string selectedQuestionary;
    public static int startFromQuestion;
    public static Questionary selectedQuestionaryObj;
    public static Questionary[] availableQuestionaries;

    public static void SaveQuestionary()
    {
        availableQuestionaries[selectedQuestionaryObj.id] = selectedQuestionaryObj;
        //Debug.Log(selectedQuestionaryObj);
        string json = JsonHelper.ToJson<Questionary>(availableQuestionaries,true);
        string path = FilePaths.selectedDisciplinePath + PathHelper.DisciplineIdToPath(selectedDiscipline) + "_questionaries.json";
        //Debug.Log(json);
        //Debug.Log(path);
        File.WriteAllText(path,json);
    }

    public static void RemoveAllProgress()
    {
        string[] subDirectories = Directory.GetDirectories(FilePaths.questionariesPath);

        foreach (string sd in subDirectories)
        {
            string name = new DirectoryInfo(sd).Name;
            string file = sd + "/" + name + "_questionaries.json";
            string json = File.ReadAllText(file);

            availableQuestionaries = JsonHelper.FromJson<Questionary>(json);

            foreach (Questionary q in availableQuestionaries)
            {
                q.averagescore = 0;
                q.bestscore = 0;
                q.correctAnswers = 0;
                q.lastAnsweredQuestion = 0;
                q.timesAnswered = 0;
            }
            json = JsonHelper.ToJson<Questionary>(availableQuestionaries,true);
            File.WriteAllText(file,json);
        }
    }
}

public static class FilePaths
{
    public static string questionariesPath = Application.dataPath + "/Data/Questionaries/";
    public static string imagesPath = "Images/";
    public static string selectedDisciplinePath;
    public static string selectedQuestionaryPath;
}

public static class PathHelper
{

    public static string DisciplineIdToPath(int id)
    {
        switch (id)
        {
            case 0:
                return "por";
            case 1:
                return "fis";
            case 2:
                return "qui";
            case 3:
                return "mat";
            case 4:
                return "lit";
            case 5:
                return "geo";
            case 6:
                return "soc";
            case 7:
                return "his";
            case 8:
                return "ing";
            case 9:
                return "bio";
            case 10:
                return "art";
            default:
                return null;
        }
    }

}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array,bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper,prettyPrint);
    }
}