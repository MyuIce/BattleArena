using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class SceneSwitch : MonoBehaviour
{
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("StageChoice");
    }
    public void OnClickHowButton()
    {
        SceneManager.LoadScene("HowToPlay");
    }
    public void OnClickExitButton()
    {
        Application.Quit();
    }
    public void OnClickStage1()
    {
        SceneManager.LoadScene("Stage1");
    }
    public void OnClickStage2()
    {
        SceneManager.LoadScene("Stage2");
    }
    public void OnClickBackStageButton()
    {
        SceneManager.LoadScene("StageChoice");
    }
    public void OnClickBackTitleButton()
    {
        SceneManager.LoadScene("Title");
    }
    public void OnClickRetryStage1Button()
    {
        SceneManager.LoadScene("Stage1");
        
    }

}
