using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentMusic : MonoBehaviour
{
    private static PermanentMusic instance ;
    public void Awake () {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
