
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blockchain;
using Newtonsoft.Json;
using System.Linq;
using SimpleJSON;
using UnityEngine.Networking;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NFTLoader : MonoBehaviour
{
    private const string STOREDDATA = "nftIds.json";
    private Dictionary<string, NFT> loadedNfts = new Dictionary<string, NFT>();
    public Action<NFT> nftCreated;
    public Action onLoadComplete;

    [SerializeField] private NetworkConfiguration config;
    [SerializeField] private Trigger trigger;

    private List<Sprite> oddyPortraits = new List<Sprite>();
    private Dictionary<string, Sprite> oddyBackgrounds = new Dictionary<string, Sprite>();

    private AsyncOperationHandle<IList<Sprite>> oddysLoadingOp;
    private AsyncOperationHandle<IList<Sprite>> backgroundLoadingOp;

    private List<string> nftIDs = new List<string>();
    
    public void Load()
    {
        var account = PlayerPrefs.GetString("Account");
        AddHeldTokensToIDs(account);
    }

    public async void AddHeldTokensToIDs(string account)
    {
        ImportantMessages.Instance.ShowMessage("Fetching token id's...");

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
                nftIDs.Add(nft.tokenId);
            }

            
            //if (nftIDs.Count < 1)
            //    nftIDs.Add("265");//Allow a user to play with one of my zena
            if (nftIDs.Count > 0)
                RetrieveAssets();
            else
                onLoadComplete?.Invoke();
        }
        catch
        {
            print("Error: " + response);
        }
    }

    private void OddySpriteReceived(Sprite oddySprite)
    {
        oddyPortraits.Add(oddySprite);
    }

    private void BackgroundSpriteReceived(Sprite backgroundSprite)
    {
        oddyBackgrounds.Add(backgroundSprite.name, backgroundSprite);
    }

    //I should store the token id's locally and then check for changes from the wallet before looking to ipfs for data as it takes a long time
    //maybe the UI flow should have a sync wallet function instead of just loading them on play
    private async void RetrieveAssets()
    {
        ImportantMessages.Instance.ShowMessage("Getting portraits.....");

        oddysLoadingOp = Addressables.LoadAssetsAsync<Sprite>(nftIDs, OddySpriteReceived, Addressables.MergeMode.Union);
        await oddysLoadingOp;

        ImportantMessages.Instance.ShowMessage("Getting backgrounds.....");

        backgroundLoadingOp = Addressables.LoadAssetsAsync<Sprite>("backgrounds", BackgroundSpriteReceived);
        await backgroundLoadingOp;

        List<string> dataContainer = SaveDataManagement.Instance.LoadList("NFTData.json");
        List<string> oldData = new List<string>();
        foreach (var dataEntry in dataContainer)
        {
            var json = JSON.Parse(dataEntry);
            var tokenId = json["tokenID"].Value;
            var portrait = oddyPortraits.FirstOrDefault(x => x.name == tokenId);
            if (portrait == null)
            {
                oldData.Add(dataEntry);
                continue;
            }

            oddyPortraits.Remove(portrait);

            var attributeArray = json["data"]["attributes"].AsArray;
            Sprite backgroundSprite = null;
            foreach (JSONNode jsNode in attributeArray)
            {
                if (jsNode["trait_type"] == "Backgrounds")
                {
                    var backgroundId = jsNode["value"].Value;
                    if (oddyBackgrounds.ContainsKey(backgroundId))
                        backgroundSprite = oddyBackgrounds[backgroundId];

                    break;
                }
            }

            var nft = new SimpleNFT(tokenId, portrait, backgroundSprite);
            loadedNfts.Add(tokenId, nft);
            nftCreated?.Invoke(nft);
        }

        dataContainer.RemoveAll(item => {
            return oldData.Contains(item);
        });

        var trackingIndex = 0;
        foreach (var portrait in oddyPortraits)
        {
            ImportantMessages.Instance.ShowMessage(string.Format("Querying IPFS(This may take some time) {0}/{1}", trackingIndex, oddyPortraits.Count.ToString()));
            trackingIndex++;

            var tokenId = portrait.name;
            var tokenURI = await ERC721.URI(config.chain.ToString(), config.network, config.contractAddress, tokenId);
            var requestURI = config.GetRequestURI(tokenURI);
            var metadataRequest = UnityWebRequest.Get(requestURI);

            await metadataRequest.SendWebRequest();

            if (metadataRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                print("Could not load item " + tokenId);
                continue;
            }
            var data = System.Text.Encoding.UTF8.GetString(metadataRequest.downloadHandler.data);

            var storageNode = new JSONObject();
            storageNode["tokenID"] = tokenId;
            storageNode["data"] = JSON.Parse(data);
            dataContainer.Add(storageNode.ToString());

            print(string.Format("Parsing data for token {0}, \n{1}", tokenId, storageNode.ToString()));

            var json = JSON.Parse(data);
            var attributeArray = json["attributes"].AsArray;
            Sprite backgroundSprite = null;
            foreach (JSONNode jsNode in attributeArray)
            {
                if (jsNode["trait_type"] == "Backgrounds")
                {
                    var backgroundId = jsNode["value"].Value;
                    if (oddyBackgrounds.ContainsKey(backgroundId))
                        backgroundSprite = oddyBackgrounds[backgroundId];                   

                    break;
                }
            }

            var nft = new SimpleNFT(tokenId, portrait, backgroundSprite);
            loadedNfts.Add(tokenId, nft);
            nftCreated?.Invoke(nft);
        }

        SaveDataManagement.Instance.SaveList<string>("NFTData.json", dataContainer);
        ImportantMessages.Instance.HideUI();
        onLoadComplete?.Invoke();
    }

    private void OnDestroy()
    {
        Addressables.Release(oddysLoadingOp);
        Addressables.Release(backgroundLoadingOp);
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

    public NFT GetLoadedNFT(string id)
    {
        if (loadedNfts.ContainsKey(id))
            return loadedNfts[id];

        return null;//should maybe just default to any loaded NFT
    }
}
