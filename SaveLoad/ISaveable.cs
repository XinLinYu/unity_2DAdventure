public interface ISaveable
{
    DataDefination GetDataID();
    /// <summary>
    /// �ѵ�ǰҪ��������壬ע�ᵽ��������
    /// </summary>
    void RegisterSaveData() => DataManager.instance.RegisterSaveData(this);

    /// <summary>
    /// �������ڵ����壨���������ȣ����ӹ�������ע��
    /// </summary>
    void UnregisterSaveData() => DataManager.instance.UnregisterSaveData(this);

    /// <summary>
    /// ������Ҫ��������ݸ��߱��ӿڣ����ݸ�������
    /// </summary>
    void GetSaveData(Data data);

    /// <summary>
    /// ���ؽ��ȵ�ʱ�򣬰ѱ��ӿڴ洢������֪ͨ���ǣ�ͨ�����ӿڸ������������ȡ����
    /// </summary>
    void LoadData(Data data);
}