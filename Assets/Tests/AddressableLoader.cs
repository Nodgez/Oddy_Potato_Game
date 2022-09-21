using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class AddressableLoader : MonoBehaviour
{
    public string key;
    private AsyncOperationHandle<Sprite> assetOp;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            StartCoroutine(LoadAssets());
    }

    public IEnumerator LoadAssets()
    {
        assetOp = Addressables.LoadAssetAsync<Sprite>(key);
        yield return assetOp;

        if (assetOp.Status == AsyncOperationStatus.Succeeded)
        {
            var goRenderer = new GameObject("Bundle sprite").AddComponent<SpriteRenderer>();
            goRenderer.sprite = assetOp.Result;
        }
    }

    private void OnDestroy()
    {
        Addressables.Release(assetOp);
    }
}
