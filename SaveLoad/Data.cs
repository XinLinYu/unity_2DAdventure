using System.Collections.Generic;
using UnityEngine;

public class Data
{
    // 需要保存的场景名称
    public string sceneToSave;

    // 存储人物和敌人的坐标
    public Dictionary<string, SerializeVector3> characterPosDict = new Dictionary<string, SerializeVector3>();
    // 存储血量和能量值
    public Dictionary<string, float> floatSavedData = new Dictionary<string, float>();

    /// <summary>
    /// 将需要保存的场景序列化为字符串
    /// </summary>
    /// <param name="gameSceneSO"></param>
    public void SaveGameScene(GameSceneSO gameSceneSO)
    {
        sceneToSave = JsonUtility.ToJson(gameSceneSO);
        Debug.Log(sceneToSave);
    }

    /// <summary>
    /// 将需要读取的场景从字符串反序列化为GameSceneSO对象
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
