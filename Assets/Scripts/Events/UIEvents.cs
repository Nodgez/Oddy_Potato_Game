using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIEvents : MonoBehaviour
{
    private EventPool eventPool;
    private float score;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField] private CanvasGroup gameOverScreen;
    // Start is called before the first frame update
    void Start()
    {
        var PotatoHitEvent = new GameEvent();

        eventPool = GameObject.FindObjectOfType<EventPool>();
        eventPool.AddEventToPool("Potato_Hit", PotatoHitEvent_onHandleEvent);
        eventPool.AddEventToPool(PotatoLauncher.OutOfPotatoEventKey, GameOverEvent_onHandleEvent);
    }

    private void PotatoHitEvent_onHandleEvent()
    {
        score++;
        scoreText.text = "Spuds Collected: " + score.ToString();
    }


    private void GameOverEvent_onHandleEvent()
    {
        gameOverScreen.transform.Find("Pop Up/Game Over Text").GetComponent<TextMeshProUGUI>().text = "Amazing! you scored: " + score;
        gameOverScreen.transform.Find("Pop Up/Continue").GetComponent<Button>().onClick.AddListener(() => {
            eventPool.ClearEvents();
            SceneManager.LoadScene(0);
        });
        gameOverScreen.alpha = 1;
        gameOverScreen.blocksRaycasts = true;
        gameOverScreen.interactable = true;
    }

}
