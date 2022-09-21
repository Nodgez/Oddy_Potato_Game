
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blockchain;
using Newtonsoft.Json;
using System.IO;
using SimpleJSON;
using UnityEngine.Networking;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NFTLoader : MonoBehaviour
{
    private Dictionary<string, NFT> loadedNfts = new Dictionary<string, NFT>();
    public Action<NFT> nftCreated;

    [SerializeField] private NetworkConfiguration config;
    [SerializeField] private EventPool eventPool;
    [SerializeField] private Trigger trigger;

    private AsyncOperationHandle<IList<Sprite>> oddysLoadingOp;
    private List<string> nftIDs = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        //Load();
    }

    public void Load()
    {
        var account = PlayerPrefs.GetString("Account");
        AddHeldTokensToIDs(account);
    }

    public async void AddHeldTokensToIDs(string account)
    {
        ImportantMessages.Instance.ShowMessage("Fetching THODDYS...");

        int first = 500;
        int skip = 0;
        string response = await EVM.AllErc721(config.chain.ToString(), config.network, account, config.contractAddress, first, skip);
        try
        {
            nftIDs.Clear();
            NFTContract[] erc721s = JsonConvert.DeserializeObject<NFTContract[]>(response);

            foreach (var nft in erc721s)
            {
                var idAsNumber = int.Parse(nft.tokenId);
                var folderID = (idAsNumber / 1000) * 1000;
                var folderPath = string.Format("{0}-{1}", folderID + 1, folderID + 1000);
                var basePath = @"Assets/Thoddys/ThoddyNoBG/" + folderPath;
                var fullPath = Path.Combine(basePath, nft.tokenId + ".png");
                nftIDs.Add(fullPath);

                Debug.Log("Added NFT ID: " + fullPath);
                //CreateSimpleNFT(idAsNumber.ToString());
            }

            oddysLoadingOp = Addressables.LoadAssetsAsync<Sprite>(nftIDs,
            null,
            Addressables.MergeMode.Union);

            oddysLoadingOp.Completed += OddysLoadingOp_Completed;
        }
        catch
        {
            print("Error: " + response);
        }

        ImportantMessages.Instance.HideUI();
    }

    private void OddysLoadingOp_Completed(AsyncOperationHandle<IList<Sprite>> opResults)
    {
        foreach (var s in opResults.Result)
        {
            var oddyid = s.name;
            Debug.Log("attempting to create oddy for: " + oddyid);
            CreateSimpleNFT(oddyid, s);
        }
    }

    private void OnDestroy()
    {
        Addressables.Release(oddysLoadingOp);
    }

    private async void CreateNFT(string tokenID)
    {
        ImportantMessages.Instance.ShowMessage("Requesting attributes for ODDY " + tokenID + ".....");
        var tokenURI = await ERC721.URI(config.chain.ToString(), config.network, config.contractAddress, tokenID);
        var requestURI = config.GetRequestURI(tokenURI);
        var metadataRequest = UnityWebRequest.Get(requestURI);

        await metadataRequest.SendWebRequest();

        var data = System.Text.Encoding.UTF8.GetString(metadataRequest.downloadHandler.data);
        var json = JSON.Parse(data);

        var imageURI = config.GetRequestURI(json["image"]);
        var pfpRequest = UnityWebRequestTexture.GetTexture(imageURI);

        await pfpRequest.SendWebRequest();

        var pfp = DownloadHandlerTexture.GetContent(pfpRequest);

        var attributes = new List<NFTAttribute>();
        foreach (var attribute in json["attributes"].AsArray)
        {
            var val = attribute.Value;
            if (val["trait_type"] == "Backgrounds")
            {
                continue;
            }
            attributes.Add(new NFTAttribute(val["trait_type"], val["value"]));
        }
        
        var nft = new FullNFT(pfp, tokenID, json["name"], attributes);
        loadedNfts.Add(tokenID, nft);
        nftCreated?.Invoke(nft);

        ImportantMessages.Instance.HideUI();
    }
    
    private void CreateSimpleNFT(string tokenID, Sprite image)
    {
        //ImportantMessages.Instance.ShowMessage("Requesting attributes for ODDY " + tokenID + ".....");
        //var tokenURI = await ERC721.URI(config.chain.ToString(), config.network, config.contractAddress, tokenID);
        //var requestURI = config.GetRequestURI(tokenURI);
        //var metadataRequest = UnityWebRequest.Get(requestURI);

        //await metadataRequest.SendWebRequest();

        //var data = System.Text.Encoding.UTF8.GetString(metadataRequest.downloadHandler.data);
        //var json = JSON.Parse(data);

        //var imageURI = config.GetRequestURI(json["image"]);
        //var pfpRequest = UnityWebRequestTexture.GetTexture(imageURI);

        //await pfpRequest.SendWebRequest();
        //var pfp = DownloadHandlerTexture.GetContent(pfpRequest);

        
        var nft = new SimpleNFT(tokenID, image);
        loadedNfts.Add(tokenID, nft);
        nftCreated?.Invoke(nft);

        //ImportantMessages.Instance.HideUI();
    }

    public NFT GetLoadedNFT(string id)
    {
        if (loadedNfts.ContainsKey(id))
            return loadedNfts[id];

        return null;//should maybe just default to any loaded NFT
    }
}
