using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "New GroupMember", menuName = "Survival Game/Group Member")]
public class GroupMemberSO : ScriptableObject
{
    [Header("기본 정보")]
    public string memberName = "구성원";
    public Sprite protrait; // "초상화";
    public Gender gender = Gender.Male;
    public AgeGroup ageGroup = AgeGroup.Audlt;

    [Header("기본 스텟")]
    [Range(50, 100)]
    public int maxHealth = 100;            //체력
    [Range(50, 100)] 
    public int maxHunger = 100;           //배고픔
    [Range(36, 38)]
    public int normalBodyTemp = 37;        //섭씨 37도 (정상 체온)

    [Header("특성")]
    [Range(0.5f, 2.0f)]
    public float coldResistance = 1.0f;     //추위 저항력
    [Range(0.5f, 2.0f)]
    public float foodEfficiency = 1.0f;      //음식 효율
    [Range(0.8f, 1.5f)]
    public float recoveryRate = 1.0f;        //회복력

    [Header("설명")]
    [TextArea(2, 3)]
    public string description = "그룹 구성원 입니다.";

    public enum Gender
    {
        Male,                    //남성
        Female                   //여성
    }

    public enum AgeGroup
    {
        Child,              //아이
        Audlt,              //어른
        Elder                //노인
    }
}
