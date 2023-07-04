using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class hudScript : MonoBehaviour
{
    public GameObject generationBanner;

    public void ToMain()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateGeneration(int gen)
    {
        generationBanner.GetComponent<TMPro.TextMeshProUGUI>().text = "Generation: " + gen.ToString();
    }

}
