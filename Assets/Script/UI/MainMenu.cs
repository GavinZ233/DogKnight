using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;



public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;
    [SerializeField]
    PlayableDirector director;
    public GameObject eventsystem;

    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();
        director = FindObjectOfType<PlayableDirector>();
        director.Stop();

        newGameBtn.onClick.AddListener(NewGame);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        eventsystem.SetActive(true);


    }
    void PlayTimeline()
    {

         
        NewGame();
    }

    void NewGame()//director委托有返回值
    {
        eventsystem.SetActive(false);
        PlayerPrefs.DeleteAll();
        ScenesController.Instance.TransitionToFirstLevel();
    }
    void ContinueGame()
    {
        eventsystem.SetActive(false);

        if (PlayerPrefs.HasKey("DogKnight")) 
        {
            SaveManager.Instance.ContinueGame();

        }
        else
        {
            NewGame();

        }


    }
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("quit");

    }
}
