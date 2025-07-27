using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Gold", menuName = "Gold/ gold")]
public class PlayerMoneySO : ScriptableObject
{
    [Header("플레이어 돈")]
    public string goldName = "코인";
    public Sprite Icon;

    [Header("화폐")]
    public int gold = 100;

    [Header("갯수")]
    public int count = 1;

    [Header("설명")]
    [TextArea(2, 3)]
    public string description = "이 돈은 0원입니다.";

}
