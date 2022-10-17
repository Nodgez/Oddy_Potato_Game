using Blockchain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OddyLoader : MonoBehaviour
{
    public NFTLoader nftLoader;
    public OddySelectionUI oddySelectionUI;
    public Material oddyMaterial;
    public SpriteRenderer backgroundRenderer;

    [SerializeField] private ParticleSystem oddyGlitchEffect;
    [SerializeField] private TimeManagement timeManagement;

    private void Awake()
    {        
        nftLoader.nftCreated += CreateThumbnail;
        nftLoader.onLoadComplete += oddySelectionUI.Show;
    }


    private void OnDestroy()
    {
        nftLoader.nftCreated -= CreateThumbnail;
        nftLoader.onLoadComplete -= oddySelectionUI.Show;
    }

    private void CreateThumbnail(NFT oddy)
    {
        if (oddy is FullNFT)
            CreateThumbnail(oddy as FullNFT);

        else
            CreateThumbnail(oddy as SimpleNFT);
    }

    private void CreateThumbnail(FullNFT oddy)
    {
        var oddySprite = Sprite.Create(oddy.image, new Rect(0, 0, oddy.image.width, oddy.image.height), Vector2.zero);
        oddySelectionUI.AddOddyToDisplay(oddySprite, oddy.Background, () =>
        {
            //Start Timer
            timeManagement.StartTime(20);
            var oddyObject = oddy.CreateGameObject();
            if (oddyObject == null)
                return;

            var movement = oddyObject.AddComponent<Movement>();
            movement.Speed = 6;

            oddyObject.AddComponent<CapsuleCollider2D>();
            oddyObject.layer = LayerMask.NameToLayer("Player");

            var glitchParticle = Instantiate<ParticleSystem>(oddyGlitchEffect, oddyObject.transform);
            glitchParticle.transform.localPosition = Vector3.zero;
            glitchParticle.transform.localRotation = Quaternion.identity;
        });
    }

    private void CreateThumbnail(SimpleNFT oddy)
    {
        var oddySprite = oddy.Sprite;
        var oddyBG = oddy.Background;
        oddySelectionUI.AddOddyToDisplay(oddySprite, oddyBG, () =>
        {
            //Start Timer
            timeManagement.StartTime(20);
            var oddyObject = oddy.CreateGameObject(oddyMaterial);
            if (oddyObject == null)
                return;

            var movement = oddyObject.AddComponent<Movement>();
            movement.Speed = 6;

            var collider = oddyObject.AddComponent<CapsuleCollider2D>();
            collider.offset = new Vector2(0, 1.75f);
            oddyObject.layer = LayerMask.NameToLayer("Player");

            var glitchParticle = Instantiate<ParticleSystem>(oddyGlitchEffect, oddyObject.transform.GetChild(0));
            glitchParticle.transform.localPosition = Vector3.zero;
            glitchParticle.transform.localRotation = Quaternion.identity;

            backgroundRenderer.sprite = oddy.Background;
        });
    }
}
