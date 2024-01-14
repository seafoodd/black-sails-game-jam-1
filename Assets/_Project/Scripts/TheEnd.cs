using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEnd : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private void Awake()
    {
        anim.Play("The End");
    }
}
