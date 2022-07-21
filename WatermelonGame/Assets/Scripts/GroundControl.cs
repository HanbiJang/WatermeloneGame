using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControl : MonoBehaviour
{
    public bool isPlayed = false;
    public FruitManager fruitManager; //�ν����� �Ҵ�

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject == fruitManager.newFruitGameObject && !isPlayed) {
            isPlayed = true;
            fruitManager.EffectSoundPlay(EffectSound.Drop);
            Debug.Log("�������");
        }
        
    }
}
