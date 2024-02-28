using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
    [Header("�¼�����")]
    public VoidEventSO newGameEvent;

    [Header("��������")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("�����޵�")]
    // �޵�ʱ��
    public float invulnerableDuration;
    // ��ʱ��
    [HideInInspector] public float invulnerableCounter;
    // �޵�״̬
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;

    // ��Transform���괫���¼���
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent onDie;

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        // ���Լ���������ݽ���ע��
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        // ���Լ���������ݽ���ע��
        ISaveable saveable = this;
        saveable.UnregisterSaveData();
    }

    private void NewGame()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }


    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0 ) 
            { 
                invulnerable = false;
            }
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
        {
            return;
        }
        if (currentHealth - attacker.damage > 0) 
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            // ִ������
            OnTakeDamage?.Invoke(attacker.transform);
        } else
        {
            currentHealth = 0;
            // ��������
            onDie?.Invoke();
        }
        // ����Ѫ���Ŀ۳�
        OnHealthChange?.Invoke(this);
    }

    /// <summary>
    /// ���������޵�
    /// </summary>
    private void TriggerInvulnerable()
    {
        if (!invulnerable) 
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            // ��������Ѫ������
            // ����Ѫ���Ŀ۳�
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            onDie?.Invoke();
        }
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        // �жϼ�������û�������ID������У���ֱ��ȡ��
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = new SerializeVector3(transform.position);
            // ��ȡѪ��������
            data.floatSavedData[GetDataID().ID + "health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "power"] = this.currentPower;
        } else
        {
            // û������ӽ�ȥ���м�¼
            data.characterPosDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            // ��Ѫ��������
            data.floatSavedData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    public void LoadData(Data data)
    {
        // ����������ID�����ȡ����
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();
            // ��ȡѪ�������������������
            this.currentHealth = data.floatSavedData[GetDataID().ID + "health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "power"];
        }

        // ����UI����ʾ
        OnHealthChange?.Invoke(this);
    }
}
