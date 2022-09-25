using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableLoader : MonoBehaviour
{
    public string key;
    private AsyncOperationHandle<Sprite> assetOp;
    private AsyncOperationHandle<IList<Sprite>> assetsOp;

    public 
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            StartCoroutine(LoadAssets());
    }

    public IEnumerator LoadAssets()
    {
        assetsOp = Addressables.LoadAssetsAsync<Sprite>(key, addressable => {
            Debug.Log(addressable.name);

        });
        yield return assetOp;

        //if (assetsOp.Status == AsyncOperationStatus.Succeeded)
        //{
        //    var goRenderer = new GameObject("Bundle sprite").AddComponent<SpriteRenderer>();
        //    goRenderer.sprite = assetOp.Result;
        //}
    }

    private void OnDestroy()
    {
        Addressables.Release(assetOp);
    }
}
