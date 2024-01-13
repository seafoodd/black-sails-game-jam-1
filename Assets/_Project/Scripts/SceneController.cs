using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private PlayerMovement pm;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel(float time = 1)
    {
        StartCoroutine(LoadLevel(time));
    }

    IEnumerator LoadLevel(float time)
    {
        transitionAnim.SetTrigger("End");
        pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        pm.OnAnimationPlaying(time);
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        pm.OnAnimationPlaying(1);
        transitionAnim.SetTrigger("Start");

    }

}
