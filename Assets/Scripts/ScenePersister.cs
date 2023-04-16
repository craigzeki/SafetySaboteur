using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersister : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
