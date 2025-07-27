using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Drink", menuName = "Drink/ Drinks")]
public class DrinkSO : ScriptableObject
{
    [Header("아이템 정보")]
    public string DrinkName = "음료";
    public Sprite protrait;

    [Header("가격")]
    [Range(100, 5000)]
    public int price = 0;

    [Header("설명")]
    [TextArea(2, 3)]
    public string description = "맛있는 음료 입니다.";


}

