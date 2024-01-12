using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public static DontDestroyOnLoad instance;

    //[HideInInspector] public string objectID;

    private void Awake()
    {
        //objectID = name + transform.position.ToString();
        for(int i = 0; i < Object.FindObjectsOfType<DontDestroyOnLoad>().Length; i++)
        {
            if(Object.FindObjectsOfType<DontDestroyOnLoad>()[i] != this)
            {
                if(Object.FindObjectsOfType<DontDestroyOnLoad>()[i].name == gameObject.name)
                {
                    Destroy(gameObject);
                }
            }
        }

        DontDestroyOnLoad(gameObject);
    }

   /* private void Start()
    {
    }*/
}