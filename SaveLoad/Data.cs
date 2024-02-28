using System.Collections.Generic;
using UnityEngine;

public class Data
{
    // ��Ҫ����ĳ�������
    public string sceneToSave;

    // �洢����͵��˵�����
    public Dictionary<string, SerializeVector3> characterPosDict = new Dictionary<string, SerializeVector3>();
    // �洢Ѫ��������ֵ
    public Dictionary<string, float> floatSavedData = new Dictionary<string, float>();

    /// <summary>
    /// ����Ҫ����ĳ������л�Ϊ�ַ���
    /// </summary>
    /// <param name="gameSceneSO"></param>
    public void SaveGameScene(GameSceneSO gameSceneSO)
    {
        sceneToSave = JsonUtility.ToJson(gameSceneSO);
        Debug.Log(sceneToSave);
    }

    /// <summary>
    /// ����Ҫ��ȡ�ĳ������ַ��������л�ΪGameSceneSO����
    /// </summary>
    /// <returns></returns>
    public GameSceneSO getSaveScene()
    {
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);
        return newScene;
    }
}


public class SerializeVector3
{
    public float x, y, z;

    public SerializeVector3(Vector3 position)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
