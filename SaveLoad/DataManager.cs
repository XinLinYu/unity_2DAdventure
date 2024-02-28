using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using Newtonsoft.Json;

[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("�¼�����")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private List<ISaveable> saveableList = new List<ISaveable>();

    private Data saveData;

    // �־û��洢�ļ���·��
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
        // ��ȡ����
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
        // ����ÿһ��saveable���������ݵĴ洢
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }
        // ���մ洢·������ȷ���ļ�
        var resultSavePath = jsonFolder + "data.sav";
        // ���л��洢����ΪJSON
        var jsonData = JsonConvert.SerializeObject(saveData);
        // �ж�Ŀ��Ŀ¼�Ƿ��Ѿ�ӵ�д洢�ļ�
        if (!File.Exists(resultSavePath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        // ��ʼд��
        File.WriteAllText(resultSavePath, jsonData);
    }

    public void Load()
    {
        // ����ÿһ��saveable���������ݵĴ洢
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
            // ��Ŀ¼��ȡ���ݴ洢�ļ�
            var stringData = File.ReadAllText(resultSavePath);
            // �����л�ΪData��������
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);
            saveData = jsonData;
        }
    }
}
