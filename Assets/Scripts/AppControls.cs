using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppControls : MonoBehaviour
{
    private static AppControls instance;
    
    public static AppControls Instance
    {
        get {
            if (instance == null)
            {
                var appControl = new GameObject("App Control");
                instance = appControl.AddComponent<AppControls>();
            }

            return instance; 
        }
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void OpenOpensea()
    {
        Application.OpenURL("https://opensea.io/collection/the-odd-dystrict-zena");
    }
    
    public void OpenWebSite()
    {
        Application.OpenURL("https://the-odd-dystrict.com/");
    }
    
    public void ReloadWalletScene()
    {
        SceneManager.LoadScene(0);
    }
}