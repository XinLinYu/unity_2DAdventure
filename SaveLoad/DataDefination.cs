using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefination : MonoBehaviour
{
    public PersistentType persistentType;
    // Œ®“ª±Í ∂
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
