using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("受伤无敌")]
    // 无敌时间
    public float invulnerableDuration;
    // 计时器
    [HideInInspector] public float invulnerableCounter;
    // 无敌状态
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;

    // 将Transform坐标传入事件中
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent onDie;

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        // 把自己与相关数据进行注册
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        // 把自己与相关数据进行注销
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
            // 执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
        } else
        {
            currentHealth = 0;
            // 触发死亡
            onDie?.Invoke();
        }
        // 进行血条的扣除
        OnHealthChange?.Invoke(this);
    }

    /// <summary>
    /// 触发受伤无敌
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
            // 死亡或者血量更新
            // 进行血条的扣除
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
        // 判断集合中有没有我这个ID，如果有，则直接取出
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = new SerializeVector3(transform.position);
            // 读取血量和能量
            data.floatSavedData[GetDataID().ID + "health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "power"] = this.currentPower;
        } else
        {
            // 没有则添加进去进行记录
            data.characterPosDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            // 存血量和能量
            data.floatSavedData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    public void LoadData(Data data)
    {
        // 如果有我这个ID，则读取出来
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();
            // 读取血量和能量给人物的属性
            this.currentHealth = data.floatSavedData[GetDataID().ID + "health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "power"];
        }

        // 更新UI的显示
        OnHealthChange?.Invoke(this);
    }
}
