using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName = "";
    string posx = "x";
    string posy = "y";
    string posz = "z";

    List<ISave> save =new List<ISave>();
    public string SceneName
    {
        get { return PlayerPrefs.GetString(sceneName); }
    }
    public Vector3 Pos
    {

        get
        {
            if (PlayerPrefs.HasKey("DogKnight"))
            {
                return new Vector3(PlayerPrefs.GetFloat(posx), PlayerPrefs.GetFloat(posy), PlayerPrefs.GetFloat(posz));
            }
            else
            {
                return new Vector3(0,1,33);
            }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SavePlayerData();
            QuestManager.Instance.SaveQuestManager();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ContinueGame();
            QuestManager.Instance.LoadQuestManager();
        }
    }
    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("DogKnight"))
        {
            ScenesController.Instance.TransitionToLoadGame(SceneName);

            GameManager.Instance.playerState.transform.position = Pos;
            QuestManager.Instance.LoadQuestManager();

        }

    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerState.characterData, "DogKnight",GameManager.Instance.playerState.transform.position);
        InventoryManager.Instance.SaveData();
        QuestManager.Instance.SaveQuestManager();

        SaveSuccess();

    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerState.characterData, "DogKnight");
        //InventoryManager.Instance.LoadData();

    }

    public void Save(object data,string key,Vector3 pos)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("x", pos.x);
        PlayerPrefs.SetFloat("y", pos.y);
        PlayerPrefs.SetFloat("z", pos.z);

        PlayerPrefs.Save();
    }
    public void Load(object data,string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }

    public void Save(object data,string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();

    }


    public void AddSave(ISave s)
    {
        save.Add(s);
    }
    public void RemoveSave(ISave s)
    {
        save.Remove(s);
    }
    public void SaveSuccess()
    {
        foreach (var isave in save)
        {
            isave.SaveSuccess();
        }
    }
}
