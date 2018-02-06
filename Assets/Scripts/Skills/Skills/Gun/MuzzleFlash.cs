﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject FlashHolder;
    public Sprite[] FlashSprites;
    public SpriteRenderer[] SpriteRenderers;

    public float flashTime;

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        FlashHolder.SetActive(true);

        int flashSpriteIndex = Random.Range(0, FlashSprites.Length);
        for (int i = 0; i < SpriteRenderers.Length; i++)
        {
            SpriteRenderers[i].sprite = FlashSprites[flashSpriteIndex];
        }

        Invoke("Deactivate", flashTime);
    }

    void Deactivate()
    {
        FlashHolder.SetActive(false);
    }

}
