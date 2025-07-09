using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private int damage = 30;// 데미지 30


    [Header("반복문 선택 옵션")]
    [SerializeField] private int loopType = 0;

    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(h, 0, v) * 5f * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AreaAttack();
        }
    }

    void AreaAttack()
    {
        //범위 내 적 찾기
        List<Enemy> enemies = new List<Enemy>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach(Collider col in colliders)
        {
            Enemy enmey = col.GetComponent<Enemy>();
            if(enmey != null) enemies.Add(enmey);

        }

        switch (loopType)
        {
            case 0: 
                foreach (Enemy enemy in enemies)
                {
                    enemy.TakeDamage(damage);
                }
                break;
            case 1:
                for (int i = 0; i <enemies.Count; i++)
                {
                    enemies[i].TakeDamage(damage);
                }
                break;
            case 2:
                int j = 0;
                while (j < enemies.Count)
                {
                    enemies[j].TakeDamage(damage);
                    j++
                }
                break;
            case 3: 
                if(enemies.Count >0)
                {
                    int k = 0;
                    do
                    {
                        enemies[k].TakeDamage(damage);
                        k++
                    }
                    while (k < enemies.Count);
                }
                break;
        }
    }

    void OnDraGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
