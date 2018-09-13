using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shareData : MonoBehaviour 
{

    public static shareData Instance
    {
        get; private set;
    }
    public int scoreNum = 0;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad (gameObject);

        scoreNum = 0;
    }
}
