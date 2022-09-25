using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OddySelectionUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private LayoutGroup oddyParent;
    [SerializeField] private Button oddyButton;

    private Trigger oddySelectedTrigger;

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
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
