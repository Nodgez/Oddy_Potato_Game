using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button QuitButton, openseaButton, websiteButton;

    private void Awake()
    {
        QuitButton.onClick.AddListener(() => AppControls.Instance.Quit());
        openseaButton.onClick.AddListener(() => AppControls.Instance.OpenOpensea());
        websiteButton.onClick.AddListener(() => AppControls.Instance.OpenWebSite());
    }
}
