using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : MonoBehaviour
{

    public LocalAssetFactory localAssetFactory;
    private static FactoryManager _instance;
    public static FactoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FactoryManager>();
            }
            return _instance;
        }
    }


}
