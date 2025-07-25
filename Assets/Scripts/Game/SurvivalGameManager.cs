using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("�׷� ������ ���ø�")]
    public GroupMemberSO[] groupMembers;

    [Header("������ ���ø�")]
    public ItemSO foodItem;              //���� ������SO
    public ItemSO fuelItem;              //���� ������SO
    public ItemSO medicineItem;         //�Ǿ�ǰ ������SO

    [Header("���� UI")]
    public Text dayText;                                //��¥ ǥ�� UI
    public Text[] memberStatusTexts;                    //�ɹ� ���� ǥ�� UI
    public Button nextDayButton;                        //���� �� ��ư
    public Text inventoryText;                          //�κ��丮 ǥ��


    [Header("������ ��ư")]
    public Button feedButton;         //���� �ֱ�
    public Button heatButton;         //���� �ϱ�
    public Button healButton;          //ġ�� �ϱ�



    [Header("���� ����")]
    int currentDay;                          //���� ��¥
    public int food = 5;                     //���� ����
    public int fuel = 3;                     //���� ����
    public int medicine = 4;                // �Ǿ�ǰ ����

    [Header("Ư�� �ɹ������� �Ҹ� ��ư")]
    public Button[] individualFoodButtons;
    public Button[] individualHealButtons;

    [Header("�̺�Ʈ �ý���")]
    public EventSO[] events;                 //�̺�Ʈ ���
    public GameObject eventPopup;            //�̺�Ʈ �˾� �г�
    public Text eventTitleText;              //�̺�Ʈ ����
    public Text eventDescriptionText;        //�̺�Ʈ ����
    public Button eventConfirmbutton;        //�̺�Ʈ �ݱ�(Ȯ��) ��ư



    //��Ÿ�� ������
    public int[] memberHealth;
    public int[] memberHunger;
    public int[] memberBodyTemp;


    // Start is called before the first frame update
    void Start()
    {

        currentDay = 1;


        InitializeGroup();
        UpdateUI();

        nextDayButton.onClick.AddListener(NextDay);         //���� �� ��ư Ŭ�� �� NextDay �Լ� ȣ��
        feedButton.onClick.AddListener(UseFoodItem);
        healButton.onClick.AddListener(UseMedicItem);
        heatButton.onClick.AddListener(UseFuelItem);

        for (int i = 0; i < individualFoodButtons.Length && i < groupMembers.LongLength; i++)
        {
            int memberindex = i;
            individualFoodButtons[i].onClick.AddListener(() => GiveFoodToMember(memberindex));

        }

        for (int i = 0; i < individualHealButtons.Length && i < groupMembers.LongLength; i++)
        {
            int memberindex = i;
            individualHealButtons[i].onClick.AddListener(() => HealMember(memberindex));
        }

        eventPopup.SetActive(false);
        eventConfirmbutton.onClick.AddListener(CloseEventPopup);

    }



    void InitializeGroup()
    {
        int memberCount = groupMembers.Length;                //�׷� �ɹ��� ���� ��ŭ �ο� �� �Ҵ�
        memberHealth = new int[memberCount];                  //�׷� �ɹ� ���� ��ŭ �迭 �Ҵ�
        memberHunger = new int[memberCount];
        memberBodyTemp = new int[memberCount];

        for (int i = 0; i < memberCount; i++)
        {
            if (groupMembers[i] != null)
            {
                memberHealth[i] = groupMembers[i].maxHealth;
                memberHunger[i] = groupMembers[i].maxHunger;
                memberBodyTemp[i] = groupMembers[i].normalBodyTemp;
            }
        }
    }

    public void UpdateUI()
    {
        dayText.text = $"Day{currentDay}";

        inventoryText.text = $"����   : {food}��\n" +
                             $"����   : {fuel}��\n" +
                             $"�Ǿ�ǰ : {medicine}��\n";

        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberStatusTexts[i] != null)
            {
                GroupMemberSO member = groupMembers[i];

                string status = GetMemberStatus(i);

                memberStatusTexts[i].text =
                    $"{member.memberName} {status} \n" +
                    $"ü��   : {memberHealth[i]} \n" +
                    $"����� : {memberHunger[i]} \n" +
                    $"ü��   : {memberBodyTemp[i]} ��";
            }


            UpdateTextColor(memberStatusTexts[i], memberHealth[i]);
        }
    }

    void ProcessDilyChange()
    {
        int baseHungerLoss = 15;
        int baseTempLoss = 1;

        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] == null) continue;


            GroupMemberSO member = groupMembers[i];

            //���̿� ���� ����� ����
            float hungerMltiplier = member.ageGroup == GroupMemberSO.AgeGroup.Child ? 0.8f : 1.0f;

            //���� ����
            memberHunger[i] -= Mathf.RoundToInt(baseHungerLoss * hungerMltiplier);              //�ɹ��� ����� ���� ����
            memberBodyTemp[i] -= Mathf.RoundToInt(baseTempLoss * member.coldResistance);       //����� ���� ���׷�


            //�ǰ� üũ
            if (memberHunger[i] <= 0) memberHunger[i] -= 15;
            if (memberBodyTemp[i] <= 32) memberHealth[i] -= 10;
            if (memberBodyTemp[i] <= 30) memberHealth[i] -= 20;

            //�ּҰ� ����
            memberHunger[i] = Mathf.Max(0, memberHunger[i]);
            memberBodyTemp[i] = Mathf.Max(25, memberBodyTemp[i]);
            memberHealth[i] = Mathf.Max(0, memberHealth[i]);



        }
    }

    public void NextDay()
    {
        currentDay += 1;
        ProcessDilyChange();
        CheckRandomEvent();
        UpdateUI();
        CheckGameOver();
    }

    string GetMemberStatus(int memberIndex)
    {
        //��� üũ
        if (memberHealth[memberIndex] <= 0)
            return "(���)";

        //���� ������ ���º��� üũ
        if (memberBodyTemp[memberIndex] <= 30) return "(�ɰ��� ��ü����)";
        else if (memberHealth[memberIndex] <= 20) return "(����)";
        else if (memberHunger[memberIndex] <= 10) return "(���ָ�)";
        else if (memberBodyTemp[memberIndex] <= 32) return "(��ü����)";
        else if (memberHealth[memberIndex] <= 50) return "(����)";
        else if (memberHunger[memberIndex] <= 30) return "(�����)";
        else if (memberBodyTemp[memberIndex] <= 35) return "(����)";
        else return "(�ǰ�)";


    }

    void CheckGameOver()
    {
        int aliveCount = 0;

        for (int i = 0; i < memberHealth.Length; i++)
        {
            if (memberHealth[i] > 0) aliveCount++;
        }

        if (aliveCount == 0)
        {
            nextDayButton.interactable = false;
            Debug.Log("���� ����! ��� �������� Ȥ���� ��Ȳ�� �̰ܳ��� ���߽��ϴ�.");
        }
    }

    void UpdateTextColor(Text text, int health)
    {
        if (health <= 0)
            text.color = Color.gray;
        else if (health <= 20)
            text.color = Color.red;
        else if (health < 50)
            text.color = Color.yellow;
        else
            text.color = Color.white;
    }

    public void UseFoodItem()                                         //���� ������ ���
    {
        if (food <= 0 || foodItem == null) return;                   //���� ���� ó��

        food--;
        UseItemOnAllMembers(foodItem);
        UpdateUI();
    }

    public void UseFuelItem()                                         //���� ������ ���
    {
        if (fuel <= 0 || fuelItem == null) return;                   //���� ���� ó��

        fuel--;
        UseItemOnAllMembers(fuelItem);
        UpdateUI();
    }

    public void UseMedicItem()                                         //���� ������ ���
    {
        if (medicine <= 0 || medicineItem == null) return;                   //���� ���� ó��

        medicine--;
        UseItemOnAllMembers(medicineItem);
        UpdateUI();
    }

    void UseItemOnAllMembers(ItemSO item)
    {
        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberHealth[i] > 0)                    //����ִ� ������
            {
                ApplyItemEffect(i, item);
            }
        }
    }



    public void GiveFoodToMember(int memberIndex)
    {
        if (food <= 0 || foodItem == null) return;
        if (memberHealth[memberIndex] <= 0) return;

        food--;
        ApplyItemEffect(memberIndex, foodItem);
        UpdateUI();
    }

    public void HealMember(int memberIndex)
    {
        if (medicine <= 0 || medicineItem == null) return;
        if (memberHealth[memberIndex] <= 0) return;

        medicine--;
        ApplyItemEffect(memberIndex, medicineItem);
        UpdateUI();
    }



    void ApplyItemEffect(int memberIndex, ItemSO item)
    {
        GroupMemberSO member = groupMembers[memberIndex];

        //���� Ư�� �����ؼ� ������ ȿ�� ���
        int actualHealth = Mathf.RoundToInt(item.healthEffect * member.recoveryRate);
        int actualHunger = Mathf.RoundToInt(item.hungerEffect * member.foodEfficiency);
        int actualTemp = item.tempEffect;

        //ȿ�� ����
        memberHealth[memberIndex] += actualHealth;
        memberHunger[memberIndex] += actualHunger;
        memberBodyTemp[memberIndex] += actualTemp;

        //�ִ�ġ ����
        memberHealth[memberIndex] = Mathf.Min(memberHealth[memberIndex], member.maxHealth);
        memberHunger[memberIndex] = Mathf.Min(memberHunger[memberIndex], member.maxHunger);
        memberBodyTemp[memberIndex] = Mathf.Min(memberBodyTemp[memberIndex], member.normalBodyTemp);

    }

    void ApplyEventEffects(EventSO eventSO)
    {
        //�ڿ���ȭ
        food += eventSO.foodChange;
        fuel += eventSO.fuelChange;
        medicine += eventSO.medicineChange;

        //�ڿ� �ּҰ� ����
        food = Mathf.Max(0, food);
        fuel = Mathf.Max(0, fuel);
        medicine = Mathf.Max(0, medicine);

        //��� ����ִ� ������� ���� ��ȭ����
        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberHealth[i] > 0)
            {
                memberHealth[i] += eventSO.healthChange;
                memberHunger[i] += eventSO.hungerChange;
                memberBodyTemp[i] += eventSO.tempChange;

                //���Ѱ� ����
                GroupMemberSO member = groupMembers[i];
                memberHealth[i] = Mathf.Clamp(memberHealth[i], 0, member.maxHealth);
                memberHunger[i] = Mathf.Clamp(memberHunger[i], 0, member.maxHunger);
                memberBodyTemp[i] = Mathf.Clamp(memberBodyTemp[i], 0, member.normalBodyTemp);
            }
        }
    }

    void ShowEventPopup(EventSO eventSO)
    {
        //�˾� Ȱ��ȭ
        eventPopup.SetActive(true);

        //�ؽ�Ʈ ����
        eventTitleText.text = eventSO.eventTitel;
        eventDescriptionText.text = eventSO.eventDescription;

        //�̺�Ʈ ȿ�� ����
        ApplyEventEffects(eventSO);

        //���� ���� �Ͻ�����
        nextDayButton.interactable = false;


    }

    public void CloseEventPopup()
    {
        eventPopup.SetActive(false);
        nextDayButton.interactable = true;
        UpdateUI();
    }

    void CheckRandomEvent()
    {
        int totalProbability = 0;

        //��ü Ȯ�� �� ���ϱ�
        for (int i = 0; i < events.Length; i++)
        {
            totalProbability += events[i].probability;
        }

        if (totalProbability == 0)
            return;

        int roll = Random.Range(1, totalProbability + 1 + 50);            //��üȮ�� ���ϱ⿡ �ƹ��͵� ���� Ȯ�� 50
        int cumualtive = 0;

        for (int i = 0; i < events.Length; i ++)
        {
            cumualtive += events[i].probability;
            if (roll <= cumualtive)
            {
                ShowEventPopup(events[i]);
                return;
            }
        }
    }

}
