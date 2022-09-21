using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImportantMessages : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;

    private static ImportantMessages instance;
    public static ImportantMessages Instance
    {
        get {return instance; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    public void ShowMessage(string message)
    {
        canvasGroup.alpha = 1;
        text.text = message;
    }

    public void HideUI()
    {
        canvasGroup.alpha = 0;

    }
}
