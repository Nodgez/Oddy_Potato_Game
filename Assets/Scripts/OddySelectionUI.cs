using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OddySelectionUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup noOddysUI;
    [SerializeField] private LayoutGroup oddyParent;
    [SerializeField] private Button oddyButton;

    private Trigger oddySelectedTrigger;

    private void Awake()
    {
        var openSeaButton = noOddysUI.transform.Find("Open Sea").GetComponent<Button>();
        var websiteButton = noOddysUI.transform.Find("Website").GetComponent<Button>();
        var refreshWallet = noOddysUI.transform.Find("Refresh Wallet").GetComponent<Button>();

        openSeaButton.onClick.AddListener(AppControls.Instance.OpenOpensea);
        websiteButton.onClick.AddListener(AppControls.Instance.OpenWebSite);
        refreshWallet.onClick.AddListener(AppControls.Instance.ReloadWalletScene);
    }

    public void AddOddyToDisplay(Sprite oddyImage, Sprite oddyBG, UnityAction buttonCallback)
    {
        var newOddyButton = Instantiate(oddyButton, oddyParent.transform);
        newOddyButton.onClick.AddListener(buttonCallback);
        newOddyButton.name = oddyImage.name;
        newOddyButton.transform.Find("Portrait").GetComponent<Image>().sprite = oddyImage;
        newOddyButton.GetComponent<Image>().sprite = oddyBG;
        oddySelectedTrigger = newOddyButton.GetComponent<Trigger>();
        newOddyButton.onClick.AddListener(() =>
        {
            oddySelectedTrigger.IsTriggered = true;
            this.gameObject.SetActive(false);
        });
    }

    public void Show()
    {
        if (oddyParent.transform.childCount == 0)
        {
            noOddysUI.alpha = 1;
            noOddysUI.interactable = true;
            noOddysUI.blocksRaycasts = true;
        }

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        noOddysUI.alpha = 0;
        noOddysUI.interactable = false;
        noOddysUI.blocksRaycasts = false;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
