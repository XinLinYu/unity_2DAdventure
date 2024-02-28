using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefination : MonoBehaviour
{
    public PersistentType persistentType;
    // Ψһ��ʶ
    public string ID;

    private void OnValidate()
    {
        if (persistentType == PersistentType.ReadWrite)
        {
            if (ID == string.Empty)
            {
                ID = Guid.NewGuid().ToString();
            }
        } else
        {
            ID = string.Empty;
        }
    }
}
