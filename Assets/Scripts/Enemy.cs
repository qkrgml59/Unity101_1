using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 100;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.green;

    }

    public void TakeDamage(int damage)      //데미지 받는 함수
    {
        health -= damage;
        StartCoroutine(DamageEffect());

        if(health <= 0)                          //죽는 연출을 위한 체력 검사
        {
         
        }
    }

    IEnumerator DamageEffect()
    {
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.02f);
        GetComponent<Renderer>().material.color = Color.green;
    }

    IEnumerator Die()
    {
        GetComponent<Renderer>().material.color = Color.red;
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while (timer < 0.05f)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timer / 0.5f);
            yield return null;
        }

        Destroy(gameObject);
    }

}
