using System.Collections;
using System.Collections.Generic;
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



    }

    void UpdateExpeditionInfo()                //Ž�� ������ ǥ���ϴ� �Լ�
    {
        if (currentExpedition != null)
        {
            expeditionInforText.text = $"Ž�� : {currentExpedition.expeditionName}\n" +
                                      $"{currentExpedition.description}\n" +
                                      $"�⺻ ������ : {currentExpedition.baseSuccessRate}%";

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

        //������ ��� (ExpeditionSO�� �⺻ ������ + ��� ���ʽ�)
        int memberBouns = 0;
        int finalSuccessRate = currentExpedition.baseSuccessRate + memberBouns;
        finalSuccessRate = Mathf.Clamp(finalSuccessRate, 5, 95);

        bool success = Random.Range(1, 101) <= finalSuccessRate;

        if (success)
        {
            //���� ExpeditionSO ���� ����
            gameManager.food += currentExpedition.sucessFoodReward;
            gameManager.fuel += currentExpedition.successFuelReward;
            gameManager.medicine += currentExpedition.successMedicineReward;

            //Ž�� �Ϸ� �� �ɹ� �ణ �Ƿ�
            gameManager.memberHunger[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ����! (������ : {finalSuccessRate}%)\n" +
                         $"���� : {currentExpedition.sucessFoodReward}, ���� + {currentExpedition.successFuelReward}," +
                         $"��ǰ + {currentExpedition.successFuelReward}";

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

}
