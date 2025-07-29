using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class Expendition : MonoBehaviour
{
    [Header("Ž�� ������")]
    public ExpeditionSO[] expeditions;     //Ž�� ������


    [Header("Ž�� UI")]
    public Button expeditionButton;                //Ÿ�� ���� ��ư
    public Button[] memberButtons;                 //��� ���� ��ư��
    public GameObject memberSelectPanel;               //��� ���� �г�    
    public Text expeditionInforText;                //���õ� Ž�� ����
    public Text resultText;                       //��� ǥ�� �ؽ�Ʈ

    private SurvivalGameManager gameManager;
    private ExpeditionSO currentExpedition;


    [Header("��� �ý���")]
    public EquipmentSO[] availableEquipments;           //��� ������ ����
    public Dropdown equipmentDropdown;                //��� ���� ��Ӵٿ�UI

    public int selectedEquipmentIndex = 0;           //���õ� ��� �ε���
    public int[] equipmentDurability;                //�� ����� ������
    

    public void Start()
    {
        gameManager = GetComponent<SurvivalGameManager>();

        memberSelectPanel.SetActive(false);
        resultText.text = "";
        expeditionInforText.text = "";

        expeditionButton.onClick.AddListener(OpenMemberSelect);

        for (int i = 0; i < memberButtons.Length; i++)
        {
            int memberIndex = i;
            memberButtons[i].onClick.AddListener(() => StartExpedition(memberIndex));  //��� ��ư Ŭ�� �� StartExpedition ȣ��
        }

        //������ �迭 �ʱ�ȭ
        InititalizeEquipmentDurability();

        //��Ӵٿ� ���� �߰�
        SetupEquipementDropDown();
        equipmentDropdown.onValueChanged.AddListener(OnEquipmentChanged);
    }

    void OnEquipmentChanged(int equipmentIndex)
    {
        selectedEquipmentIndex = equipmentIndex;
       
        UpdateExpeditionInfo();                                 //Ž�� ���� ������Ʈ
    }

    void UpdateExpeditionInfo()                //Ž�� ������ ǥ���ϴ� �Լ�
    {
        if (currentExpedition != null)
        {
            EquipmentSO selectedEquip = availableEquipments[selectedEquipmentIndex];

            //�η��� ���� ���ʽ� ����
            int equipBounse = (selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0) ? 0 : selectedEquip.successBouns;
            int totalSuccessrate = currentExpedition.baseSuccessRate + equipBounse;

            string durabilityInfo = "";

            if (selectedEquipmentIndex >0)
            {
                
                
                    if (equipmentDurability[selectedEquipmentIndex] <= 0) durabilityInfo = "(�η��� ���� - ȿ�� ����)";  //�η��� ��� ǥ��
                    else durabilityInfo = $"(������ : {equipmentDurability[selectedEquipmentIndex]}/{selectedEquip.maxDurability})";
                
            }

            expeditionInforText.text = $"Ž�� : {currentExpedition.expeditionName}\n" +
                                      $"{currentExpedition.description}\n" +
                                      $"�⺻ ������ : {currentExpedition.baseSuccessRate}%\n" +
                                      $"��� ���ʽ� : +{equipBounse}%{durabilityInfo}\n" +
                                      $"���� ������ : {totalSuccessrate}%\n";

        }
    }

    void UpdateMemberButtons()                                 //��� ��ư ������Ʈ ����
    { 
        for (int i = 0; i < memberButtons.Length && i < gameManager.groupMembers.Length; i ++)
        {
            GroupMemberSO member = gameManager.groupMembers[i];
            bool canGo = gameManager.memberHealth[i] > 20;                  //ü�� 20 �̻��� �� Ž�� ����

            Text buttonText = memberButtons[i].GetComponentInChildren<Text>();
            buttonText.text = $"{member.memberName}\n ü�� : {gameManager.memberHealth[i]}";
            memberButtons[i].interactable = canGo;
        }
    }

    public void OpenMemberSelect()
    {
        //���ο� Ž�� ���� ����
        if (expeditions.Length > 0)
        {
            currentExpedition = expeditions[Random.Range(0, expeditions.Length)];
            UpdateExpeditionInfo();
        }

        memberSelectPanel.SetActive(true);
        UpdateMemberButtons();
    }

    public void StartExpedition(int memberIndex)
    {
        if (currentExpedition == null) return;

        memberSelectPanel.SetActive(false);

        GroupMemberSO member = gameManager.groupMembers[memberIndex];
        EquipmentSO selectedEquip = availableEquipments[selectedEquipmentIndex];

        //�η��� ���� ȿ�� ����
        bool equipmentBroken = selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0;
        int equipBounse = equipmentBroken ? 0 : selectedEquip.successBouns;
        int rewardBonus = equipmentBroken ? 0 : selectedEquip.rewardBonus;


        //������ ��� (ExpeditionSO�� �⺻ ������ + ��񺸳ʽ�)
        
        int finalSuccessRate = currentExpedition.baseSuccessRate + equipBounse;
        finalSuccessRate = Mathf.Clamp(finalSuccessRate, 5, 95);

        bool success = Random.Range(1, 101) <= finalSuccessRate;

        //��� ������ ���� ( �Ǽ� ����,�η����� ���� ���)
        if (selectedEquipmentIndex > 0 && !equipmentBroken)
        {
            equipmentDurability[selectedEquipmentIndex] -= 1;
            selectedEquipmentIndex = 0;
            SetupEquipementDropDown();                //��Ӵٿ� ������Ʈ
        }

        if (success)
        {
            //���� ExpeditionSO ���� ����
            gameManager.food += currentExpedition.sucessFoodReward + rewardBonus;
            gameManager.fuel += currentExpedition.successFuelReward + rewardBonus; ;
            gameManager.medicine += currentExpedition.successMedicineReward + rewardBonus;

            //Ž�� �Ϸ� �� �ɹ� �ణ �Ƿ�
            gameManager.memberHunger[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ����! (������ : {finalSuccessRate + rewardBonus}%)\n" +
                         $"���� : {currentExpedition.sucessFoodReward + rewardBonus}, ���� + {currentExpedition.successFuelReward + rewardBonus}," +
                         $"��ǰ + {currentExpedition.successFuelReward + rewardBonus}";

            resultText.color = Color.green;
         }
        else
        {
            //���� : ExpeditionSO �г�Ƽ ����
            gameManager.memberHealth[memberIndex] += currentExpedition.failHealthPenalty;
            gameManager.memberHunger[memberIndex] += currentExpedition.failHungerPenalty;
            gameManager.memberBodyTemp[memberIndex] += currentExpedition.failTempPenalty;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ����! (������ : {finalSuccessRate}%)\n" +
                $"ü�� - {currentExpedition.failHealthPenalty}, ����� - {currentExpedition.failHungerPenalty}, " +
                $" �µ� - {currentExpedition.failTempPenalty}";

            resultText.color = Color.red;
        }

        //�ּҰ� ����

        GroupMemberSO memberSO = gameManager.groupMembers[memberIndex];
        gameManager.memberHunger[memberIndex] = Mathf.Max(0, gameManager.memberHunger[memberIndex]);
        gameManager.memberBodyTemp[memberIndex] = Mathf.Max(0, gameManager.memberBodyTemp[memberIndex]);
        gameManager.memberHealth[memberIndex] = Mathf.Max(0, gameManager.memberHealth[memberIndex]);

        gameManager.UpdateUI();

        Invoke("ClearResultText", 3f);

    }

    void ClearResultText()
    {
        resultText.text = "";
    }

    void InititalizeEquipmentDurability()                               //��� ������ ���� �ϴ� Ƚ��
    {
        equipmentDurability = new int[availableEquipments.Length];                  //��� ���� ��ŭ �迭 ���� ( ���� ���� )

        for (int i = 0; i < availableEquipments.Length; i++)
        {
            equipmentDurability[i] = availableEquipments[i].maxDurability;           //��밡���� �������� �迭�� �־��ش�.
        }
    }

    void SetupEquipementDropDown()
    {
        equipmentDropdown.ClearOptions();                  //��Ӵٿ� �ʱ�ȭ

        //��� �ɼǵ��� ��� �ٿ �߰�
        for (int i = 0; i < availableEquipments.Length; i++)
        {
            string equipName = availableEquipments[i].equipmentName;

            //�������� 0�̸� (�η��� ǥ��) , �Ǽ� (�ε��� 0)�� ����
            if (i==0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData(equipName));

            }
            else if (equipmentDurability[i] <= 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} (�η���"));

            }
            else
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} ({equipmentDurability[i]} / {availableEquipments[i].maxDurability})"));
            }
        }

        equipmentDropdown.value = 0;                  //�⺻������ ù��° ��� ����
        equipmentDropdown.RefreshShownValue();       //������ ������ ���� ��� �������� ���� �Լ��� ���� ���� �Ѵ�.

    }


}
