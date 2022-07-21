using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControl : MonoBehaviour
{
    public bool isPlayed = false;
    public FruitManager fruitManager; //인스펙터 할당

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject == fruitManager.newFruitGameObject && !isPlayed) {
            isPlayed = true;
            fruitManager.EffectSoundPlay(EffectSound.Drop);
            Debug.Log("음악재생");
        }
        
    }
}
