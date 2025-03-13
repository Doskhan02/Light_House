using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistanceManager : MonoBehaviour
{
    private GamePesistantData gamePesistantData;

    private List<IDataPersistance> dataPersistanceObjects;

    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;
    private FileDataHandler fileDataHandler;

    public static DataPersistanceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one data pesistance manager in the scene");
        }

        Instance = this;
    }

    private void Start()
    {
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName,useEncryption);
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gamePesistantData = new GamePesistantData();
    }
    public void SaveGame()
    {
        foreach (IDataPersistance dataPersistentObj in dataPersistanceObjects)
        {
            dataPersistentObj.SaveData(ref gamePesistantData);
        }

        fileDataHandler.Save(gamePesistantData);
    }
    public void LoadGame() 
    {
        this.gamePesistantData = fileDataHandler.Load();

        if (gamePesistantData == null)
        {
            Debug.Log("No data was found. Initializing new data");
            NewGame();
        }
        foreach (IDataPersistance dataPersistentObj in dataPersistanceObjects) 
        {
            dataPersistentObj.LoadData(gamePesistantData);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveGame();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistanceObjects);
    }
}
