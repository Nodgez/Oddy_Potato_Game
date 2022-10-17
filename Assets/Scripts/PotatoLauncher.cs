using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoLauncher : MonoBehaviour
{
    public static string OutOfPotatoEventKey = "Out_Of_Potato";

    [SerializeField] private Potato[] potatos;
    [SerializeField] private int potatoLimit = 30;
    [SerializeField] private Trigger outOfPotatoTrigger;
    [SerializeField] private Animator portalPrefab;
    [SerializeField] private BoxCollider2D spawnBounds;
    [SerializeField] private AudioClip portalSFX;

    [Range(0.5f,5f)]
    public float maxCooldown;
    
    [Range(0.01f,0.5f)]
    public float minCooldown;

    private float timer = 0;
    private float activeCooldown = 0;
    private int potatoTracker;
    private int potatoIndex = 0;
    private Queue<Animator> portalPool = new Queue<Animator>();

    private void Awake()
    {
        EventPool eventPool = GameObject.FindObjectOfType<EventPool>();
        eventPool.AddEventToPool(TimeManagement.TimeoutEventKey, () =>
        {
            enabled = false;
        });

        eventPool.AddEventToPool("Oddy_Selected", () =>
        {
            enabled = true;
        });

        for (int i = 0; i < 10; i++)
        {
            var portal = Instantiate(portalPrefab, this.transform);
            portalPool.Enqueue(portal);
        }

        potatoTracker = potatoLimit;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            potatoIndex = (potatoIndex + 1) % potatos.Length;
        }

        timer += Time.deltaTime;

        if (potatoTracker == 0)
        {
            outOfPotatoTrigger.IsTriggered = true;
            enabled = false;
        }

        if (timer > activeCooldown)
        {
            if (potatoTracker % 12 == 0)
                MakeItRain();
            else
                SpawnPotato();
            timer = 0;
        }
    }

    private void SpawnPotato()
    {
        //https://www.redblobgames.com/articles/probability/damage-rolls.html
        var r1 = RollDice(2, 20);
        var r2 = RollDice(2, 20);
        var r = Mathf.Max(r1, r2);

        if (r < 5)
            potatoIndex = 3;//sluggish
        else if (r < 22)
            potatoIndex = 0;//regualr
        else if (r < 38)
            potatoIndex = 1;//bouncy
        else
            potatoIndex = 2;//speedy

        var location = GetRandomLocationInBounds();
        var portal = portalPool.Dequeue();
        portal.transform.position = location + Vector3.back;
        portal.SetTrigger("Appear");
        StartCoroutine(EnqueuePortal(portal));
        AudioSource.PlayClipAtPoint(portalSFX, location, 1);

        activeCooldown = Random.Range(minCooldown, maxCooldown);
        var potato = Instantiate(potatos[potatoIndex], location, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward));
        potato.onPotatoDestroyed += () => { potatoTracker--; };
    }

    private void MakeItRain()
    {
        StartCoroutine(CO_MakeItRain());
    }

    private Vector3 GetRandomLocationInBounds()
    {
        var min = spawnBounds.bounds.min;
        var max = spawnBounds.bounds.max;
        var randomLocation = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

        return randomLocation;
    }

    IEnumerator EnqueuePortal(Animator portal)
    {
        yield return new WaitForSeconds(0.6f);
        portalPool.Enqueue(portal);
    }

    IEnumerator CO_MakeItRain()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.5f);
            SpawnPotato();
        }
    }

    private int RollDice(int n, int sides)
    {
        var value = 0;
        for (int i = 0; i < n; i++)
            value += Random.Range(0, sides + 1);

        return value;
    }
}
