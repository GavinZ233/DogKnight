using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScenesController : Singleton<ScenesController>
{
    private GameObject player;
    public GameObject playerPrefab;
    public Canvas loadScreen;
    public Slider slider;
    public Text numText;
    public Text tipsText;
    public GameObject panel;

    public Fader fadePrefab;
    bool fadeFinished;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        FindLoadCanvas();
        fadeFinished = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TransitionToMain();
        }
    }

    public void FindLoadCanvas()
    {
        var canvasMore = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvasMore)
        {
            if (canvas.name == "Load Canvas"&&canvas.isActiveAndEnabled)
            {
                loadScreen = canvas;
                slider = loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
                numText= loadScreen.transform.GetChild(0).GetChild(1).GetComponent<Text>();
                tipsText= loadScreen.transform.GetChild(0).GetChild(2).GetComponent<Text>();
                panel = loadScreen.transform.GetChild(0).gameObject;
                panel.SetActive(false);
            }
            
        }
        if (loadScreen=null)
        {
            Debug.LogError("没找到加载canvas");
        }
    }
    public void TransitionMachine(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transtion(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                

                StartCoroutine(Transtion(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    /// <summary>
    /// 根据当前场景名称与传送目标点类型传送
    /// </summary>
    /// <param name="name"></param>
    /// <param name="destinationTag"></param>
    /// <returns></returns>
    IEnumerator Transtion(string sceneName, DestinationTag destinationTag)
    {
        //添加fade
        SaveManager.Instance.SavePlayerData();
        // InventoryManager.Instance.SaveData();
        // QuestManager.Instance.SaveQuestManager();

        if (SceneManager.GetActiveScene().name != sceneName)       //不同场景传送
        {
            SaveManager.Instance.SavePlayerData();

            panel.SetActive(true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;//加载完成后，是否自动跳转

            while (!operation.isDone)
            {
                slider.value = operation.progress;
                numText.text = (operation.progress + 0.1) * 100 + "%";//progress最多到0.9

                if (operation.progress >= 0.9f)
                {
                    slider.value = 1;
                    numText.text = 100 + "%";
                    tipsText.text = "请按下任意按键继续";
                    if (Input.anyKeyDown)
                    {

                        operation.allowSceneActivation = true;
                        panel.SetActive(false);
                    }

                }

                yield return null;
            }
            Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            FindLoadCanvas();

        }
        else
        {
            Fader fade = Instantiate(fadePrefab);

            yield return StartCoroutine(fade.FadeOut(2f));

            player = GameManager.Instance.playerState.gameObject;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            yield return StartCoroutine(fade.FadeIn(2f));

            yield return null;

        }

    }
    IEnumerator LoadTranstion(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName)       //不同场景传送
        {
            //yield return SceneManager.LoadSceneAsync(sceneName);
            panel.SetActive(true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;//加载完成后，是否自动跳转

            while (!operation.isDone)
            {
                slider.value = operation.progress;
                numText.text = (operation.progress + 0.1) * 100 + "%";//progress最多到0.9

                if (operation.progress >= 0.9f)
                {
                    slider.value = 1;
                    numText.text = 100 + "%";
                    tipsText.text = "请按下任意按键继续";
                    if (Input.anyKeyDown)
                    {

                        operation.allowSceneActivation = true;

                        panel.SetActive(false);
                    }

                }

                yield return null;
            }
             player=Instantiate(playerPrefab, new Vector3(0, 1, 33), transform.rotation);
            player.transform.SetPositionAndRotation(SaveManager.Instance.Pos, transform.rotation);

            SaveManager.Instance.LoadPlayerData();
            FindLoadCanvas();

        }
        else
        {
            Fader fade = Instantiate(fadePrefab);

            yield return StartCoroutine(fade.FadeOut(2f));

            player = GameManager.Instance.playerState.gameObject;
            player.transform.SetPositionAndRotation(SaveManager.Instance.Pos, transform.rotation);
            yield return StartCoroutine(fade.FadeIn(2f));

            yield return null;

        }
    }


    public TransitionDestination GetDestination(DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach (var destination in entrances)
        {
            if (destination.destinationTag == destinationTag)
            {
                return destination;
            }
        }
        return null;
    }


    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadTranstion("Village"));

        
    }
    public void TransitionToLoadGame(string scene )
    {
        StartCoroutine(LoadTranstion(scene));

    }
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    IEnumerator LoadMain()
    {
        Fader fade = Instantiate(fadePrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }


    public void EndGame()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());

        }
    }
}
