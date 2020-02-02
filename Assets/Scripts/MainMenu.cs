using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Action0")
            || Input.GetButtonDown("Action1")
            || Input.GetButtonDown("Action2"))
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
