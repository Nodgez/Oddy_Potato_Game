using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    private const int BOARDID = 4858;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private LayoutGroup container;
    [SerializeField] private RectTransform leaderboardEntryPrefab;
    private int sessionScore
    {
        get;
        set;
    }
    private EventPool eventPool;

    public string MemberID
    {
        get;
        set;    
    }

    private void Awake()
    {
        eventPool = GameObject.FindObjectOfType<EventPool>();
        eventPool.AddEventToPool("Potato_Hit", AddScore);
        eventPool.AddEventToPool("Oddy_Selected", SetName);

        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                print("response successful");
                eventPool.AddEventToPool(PotatoLauncher.OutOfPotatoEventKey, () => SubmitScore(sessionScore));
            }
            else
                print(response.Error);
        });
    }

    public void SetName()
    {
        LootLockerSDKManager.SetPlayerName(MemberID, (response) =>
        {
            if (response.success)
                Debug.Log("Set name to " + MemberID);
            else
                Debug.Log("Error setting name");
        });
    }

    private void SubmitScore(int score)
    {
        LootLockerSDKManager.SubmitScore(MemberID, score, BOARDID, (response) =>
          {
              if (response.success)
              {
                  print(MemberID + " Submit of score successful : " + score);
              }
              else
                  print(response.Error);
          });
    }

    public void GetTopLeaderboard()
    {
        ImportantMessages.Instance.ShowMessage("Getting Top 10 Scores....");
        LootLockerSDKManager.GetScoreList(BOARDID, 10, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successful leaderboard fetch");
                Show();
                foreach (var r in response.items)
                {
                    var entry = Instantiate(leaderboardEntryPrefab, container.transform);
                    entry.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = r.player.name;
                    entry.Find("Score").GetComponent<TMPro.TextMeshProUGUI>().text = r.score.ToString();
                }
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
            ImportantMessages.Instance.HideUI();
        });
    }

    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        for (var i = container.transform.childCount - 1; i > -1; i--)
            Destroy(container.transform.GetChild(i).gameObject);
    }
    
    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    private void AddScore()
    {
        sessionScore++;
    }

}
