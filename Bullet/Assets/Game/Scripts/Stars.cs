using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
