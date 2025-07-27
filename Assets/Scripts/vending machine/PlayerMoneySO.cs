using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Gold", menuName = "Gold/ gold")]
public class PlayerMoneySO : ScriptableObject
{
    [Header("�÷��̾� ��")]
    public string goldName = "����";
    public Sprite Icon;

    [Header("ȭ��")]
    public int gold = 100;

    [Header("����")]
    public int count = 1;

    [Header("����")]
    [TextArea(2, 3)]
    public string description = "�� ���� 0���Դϴ�.";

}
