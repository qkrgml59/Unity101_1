using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Drink", menuName = "Drink/ Drinks")]
public class DrinkSO : ScriptableObject
{
    [Header("������ ����")]
    public string DrinkName = "����";
    public Sprite protrait;

    [Header("����")]
    [Range(100, 5000)]
    public int price = 0;

    [Header("����")]
    [TextArea(2, 3)]
    public string description = "���ִ� ���� �Դϴ�.";


}

