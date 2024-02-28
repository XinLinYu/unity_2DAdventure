using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using Newtonsoft.Json;

[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("事件监听")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private List<ISaveable> saveableList = new List<ISaveable>();

    private Data saveData;

    // 持久化存储文件的路径
    private string jsonFolder;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        saveData = new Data();
        jsonFolder = Application.persistentDataPath + "/SAVE DATA/";
        // 读取数据
        ReadSaveData();
    }

    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;
    }

    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }

    private void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
    }

    public void RegisterSaveData(ISaveable saveable)
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }

    public void UnregisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }

    public void Save()
    {
        // 遍历每一个saveable，进行数据的存储
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }
        // 最终存储路径，精确到文件
        var resultSavePath = jsonFolder + "data.sav";
        // 序列化存储数据为JSON
        var jsonData = JsonConvert.SerializeObject(saveData);
        // 判断目标目录是否已经拥有存储文件
        if (!File.Exists(resultSavePath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        // 开始写入
        File.WriteAllText(resultSavePath, jsonData);
    }

    public void Load()
    {
        // 遍历每一个saveable，进行数据的存储
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }

    private void ReadSaveData()
    {
        var resultSavePath = jsonFolder + "data.sav";
        if (File.Exists(resultSavePath))
        {
            // 从目录读取数据存储文件
            var stringData = File.ReadAllText(resultSavePath);
            // 反序列化为Data对象数据
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);
            saveData = jsonData;
        }
    }
}
