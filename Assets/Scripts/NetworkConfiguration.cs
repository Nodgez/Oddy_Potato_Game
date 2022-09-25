using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Blockchain {
    [CreateAssetMenu(fileName = "Network Configuration", menuName = "Blockchain", order = 0)]
    public class NetworkConfiguration : ScriptableObject
    {
        public Chain chain;
        public string network = "mainnet";
        public string contractAddress = "";
        public string ipfsGateway = "https://dweb.link/ipfs/";

        public string GetRequestURI(string uri)
        {
            return uri.Replace("ipfs://", this.ipfsGateway);
        }
    }


    public enum Chain
    {
        ethereum,
        avalanche,
        binance,
        moonbeam,
        polygon,
        xdai,
        harmony,
        cronos
    }

    public class NFTContract
    {
        public string contract { get; set; }
        public string tokenId { get; set; }
        public string uri { get; set; }
        public string balance { get; set; }

        public override string ToString()
        {
            return string.Format("Contract: {0},\nToken ID: {1},\nURI: {2},\nbalance: {3},\n", contract, tokenId, uri, balance);
        }
    }


    public abstract class NFT
    {
        protected string tokenID;
    }

    public class FullNFT : NFT
    {
        public Texture2D image;
        public string name;
        public List<NFTAttribute> attributes = new List<NFTAttribute>();
        private Sprite background;

        public Sprite Background
        {
            get { return background; }
        }

        public FullNFT(Texture2D image, string tokenID, string name, List<NFTAttribute> attributes)
        {
            this.image = image;
            this.tokenID = tokenID;
            this.name = name;
            this.attributes = attributes;
            foreach (var att in attributes)
                Debug.Log(att.ToString());
        }

        public GameObject CreateGameObject()
        {
            var nft = new GameObject("NFT");
            nft.transform.position = Vector3.down * 2.5f;
            foreach (var att in attributes)
            {
                var child = new GameObject(att.value);
                var renderer = child.AddComponent<SpriteRenderer>();
                renderer.sprite = att.graphic;
                renderer.sortingOrder = att.GetLayerIndex();

                child.transform.SetParent(nft.transform);
                child.transform.localPosition = Vector3.zero;
            }
            return nft;
        }
    }

    public class SimpleNFT : NFT
    {
        private Sprite sprite;
        private Sprite background;
        public Sprite Sprite
        { 
            get { return sprite; }
        }

        public Sprite Background
        {
            get { return background; }
        }

        public SimpleNFT(string tokenID, Sprite image, Sprite background)
        {
            Debug.Log("Constructing Oddy:  " + tokenID);
            this.sprite = image;
            this.tokenID = tokenID;
            this.background = background;
        }

        public GameObject CreateGameObject(Material defaultMaterial)
        {
            var nft = new GameObject("NFT");
            nft.transform.position = Vector3.down * 5f;
            var child = new GameObject(tokenID);
            var renderer = child.AddComponent<SpriteRenderer>();
            renderer.flipX = true;
            renderer.material = defaultMaterial;
            renderer.sprite = sprite;
            renderer.sortingOrder = 5;

            child.transform.SetParent(nft.transform);
            child.transform.localPosition = Vector3.zero;
            return nft;
        }
    }

    public class NFTAttribute
    {
        private static Dictionary<string, int> layerOrderMap = new Dictionary<string, int>()
        {
            {"Backgrounds", 0 },
            {"Base Skin", 1 },
            {"Body Modifications", 2 },
            {"Mech Skins And Rips", 3 },
            {"Heart", 4 },
            {"Clothing", 5 },
            {"Mouth", 6 },
            {"Eyes", 7 }
        };
        public string traitType;
        public string value;

        public Sprite graphic;

        public NFTAttribute(string traitType, string value)
        {
            this.traitType = traitType;
            this.value = value.Replace(" ", "_");

            graphic = Resources.Load<Sprite>(this.value);
            if (graphic == null)
            {
                Debug.LogError("Could not find graphic " + this.value + " in Resources");
            }
        }

        public int GetLayerIndex()
        {
            return layerOrderMap[this.traitType];
        }

        public override string ToString()
        {
            return string.Format("trait_type: {0}, value: {1}", traitType, value);
        }
    }
}