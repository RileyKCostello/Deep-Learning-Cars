using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [Header("Text Boxes")]
    public GameObject tb1;
    public GameObject tb2;
    public GameObject tb3;
    public GameObject tb4;
    public GameObject tb5;
    public GameObject tb6;

    private Dictionary<GameObject, string> textes = new Dictionary<GameObject, string>();

    public void Awake()
    {
        textes[tb1] = "";
        textes[tb2] = "";
        textes[tb3] = "";
        textes[tb4] = "";
        textes[tb5] = "";
        textes[tb6] = "";
    }

    public string GetText(GameObject textBox)
    {
        return textBox.GetComponent<TMPro.TMP_InputField>().text;
    }

    public int GetNum(GameObject textBox)
    {
        return int.Parse(textBox.GetComponent<TMPro.TMP_InputField>().text);
    }

    public void SaveText()
    {
        List<GameObject> keys = new List<GameObject>(textes.Keys);
        foreach(var x in keys)
        {
            textes[x] = GetText(x);
        }
    }
    //Sets the text boxes back to what they were if not applied
    public void ResetText()
    {
        foreach(var x in textes)
        {
            x.Key.GetComponent<TMPro.TMP_InputField>().text = x.Value;
        }
    }

    public void ApplySettings()
    {
        SaveText();
        GHandler.numFrontRunners = GetNum(tb1);
        GHandler.numChildren = GetNum(tb2);
        GHandler.numMutants = GetNum(tb3);
        GHandler.numRandom = GetNum(tb4);
        GHandler.numChildMutations = GetNum(tb5);
        GHandler.numMutantMutations = GetNum(tb6);
        GHandler.childrenAreAveraged = true;
        GHandler.randomChildren = true;
    }

    public void StartRun()
    {
        SceneManager.LoadScene(1);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
