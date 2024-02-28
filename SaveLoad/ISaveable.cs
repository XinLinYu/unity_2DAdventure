public interface ISaveable
{
    DataDefination GetDataID();
    /// <summary>
    /// 把当前要保存的物体，注册到管理类中
    /// </summary>
    void RegisterSaveData() => DataManager.instance.RegisterSaveData(this);

    /// <summary>
    /// 将不存在的物体（敌人死亡等），从管理类中注销
    /// </summary>
    void UnregisterSaveData() => DataManager.instance.UnregisterSaveData(this);

    /// <summary>
    /// 把你需要保存的数据告诉本接口，传递给管理类
    /// </summary>
    void GetSaveData(Data data);

    /// <summary>
    /// 加载进度的时候，把本接口存储的数据通知你们，通过本接口告诉所有物体读取数据
    /// </summary>
    void LoadData(Data data);
}