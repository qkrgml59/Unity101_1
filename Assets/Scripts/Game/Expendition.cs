using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class Expendition : MonoBehaviour
{
    [Header("탐방 데이터")]
    public ExpeditionSO[] expeditions;     //탐방 종류들


    [Header("탐방 UI")]
    public Button expeditionButton;                //타방 시작 버튼
    public Button[] memberButtons;                 //멤버 선택 버튼들
    public GameObject memberSelectPanel;               //멤버 선택 패널    
    public Text expeditionInforText;                //선택된 탐방 정보
    public Text resultText;                       //결과 표시 텍스트

    private SurvivalGameManager gameManager;
    private ExpeditionSO currentExpedition;


    [Header("장비 시스템")]
    public EquipmentSO[] availableEquipments;           //사용 가능한 장비들
    public Dropdown equipmentDropdown;                //장비 선택 드롭다운UI

    public int selectedEquipmentIndex = 0;           //선택된 장비 인덱스
    public int[] equipmentDurability;                //각 장비의 내구도
    

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
            memberButtons[i].onClick.AddListener(() => StartExpedition(memberIndex));  //멤버 버튼 클릭 시 StartExpedition 호출
        }

        //내구도 배열 초기화
        InititalizeEquipmentDurability();

        //드롭다운 설정 추가
        SetupEquipementDropDown();
        equipmentDropdown.onValueChanged.AddListener(OnEquipmentChanged);
    }

    void OnEquipmentChanged(int equipmentIndex)
    {
        selectedEquipmentIndex = equipmentIndex;
       
        UpdateExpeditionInfo();                                 //탐방 정보 업데이트
    }

    void UpdateExpeditionInfo()                //탐방 정보를 표시하는 함수
    {
        if (currentExpedition != null)
        {
            EquipmentSO selectedEquip = availableEquipments[selectedEquipmentIndex];

            //부러진 장비는 보너스 없음
            int equipBounse = (selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0) ? 0 : selectedEquip.successBouns;
            int totalSuccessrate = currentExpedition.baseSuccessRate + equipBounse;

            string durabilityInfo = "";

            if (selectedEquipmentIndex >0)
            {
                
                
                    if (equipmentDurability[selectedEquipmentIndex] <= 0) durabilityInfo = "(부러진 상태 - 효과 없음)";  //부러진 장비 표시
                    else durabilityInfo = $"(내구도 : {equipmentDurability[selectedEquipmentIndex]}/{selectedEquip.maxDurability})";
                
            }

            expeditionInforText.text = $"탐방 : {currentExpedition.expeditionName}\n" +
                                      $"{currentExpedition.description}\n" +
                                      $"기본 성공률 : {currentExpedition.baseSuccessRate}%\n" +
                                      $"장비 보너스 : +{equipBounse}%{durabilityInfo}\n" +
                                      $"최종 성공률 : {totalSuccessrate}%\n";

        }
    }

    void UpdateMemberButtons()                                 //멤버 버튼 업데이트 정보
    { 
        for (int i = 0; i < memberButtons.Length && i < gameManager.groupMembers.Length; i ++)
        {
            GroupMemberSO member = gameManager.groupMembers[i];
            bool canGo = gameManager.memberHealth[i] > 20;                  //체력 20 이상일 때 탐방 가능

            Text buttonText = memberButtons[i].GetComponentInChildren<Text>();
            buttonText.text = $"{member.memberName}\n 체력 : {gameManager.memberHealth[i]}";
            memberButtons[i].interactable = canGo;
        }
    }

    public void OpenMemberSelect()
    {
        //새로운 탐방 랜덤 선택
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

        //부러진 장비는 효과 없음
        bool equipmentBroken = selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0;
        int equipBounse = equipmentBroken ? 0 : selectedEquip.successBouns;
        int rewardBonus = equipmentBroken ? 0 : selectedEquip.rewardBonus;


        //성공률 계산 (ExpeditionSO의 기본 성공률 + 장비보너스)
        
        int finalSuccessRate = currentExpedition.baseSuccessRate + equipBounse;
        finalSuccessRate = Mathf.Clamp(finalSuccessRate, 5, 95);

        bool success = Random.Range(1, 101) <= finalSuccessRate;

        //장비 내구도 감소 ( 맨손 제외,부러지지 않은 장비만)
        if (selectedEquipmentIndex > 0 && !equipmentBroken)
        {
            equipmentDurability[selectedEquipmentIndex] -= 1;
            selectedEquipmentIndex = 0;
            SetupEquipementDropDown();                //드롭다운 업데이트
        }

        if (success)
        {
            //성공 ExpeditionSO 보상 적용
            gameManager.food += currentExpedition.sucessFoodReward + rewardBonus;
            gameManager.fuel += currentExpedition.successFuelReward + rewardBonus; ;
            gameManager.medicine += currentExpedition.successMedicineReward + rewardBonus;

            //탐방 완료 한 맴버 약간 피로
            gameManager.memberHunger[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} 성공! (성공률 : {finalSuccessRate + rewardBonus}%)\n" +
                         $"음식 : {currentExpedition.sucessFoodReward + rewardBonus}, 연료 + {currentExpedition.successFuelReward + rewardBonus}," +
                         $"약품 + {currentExpedition.successFuelReward + rewardBonus}";

            resultText.color = Color.green;
         }
        else
        {
            //실패 : ExpeditionSO 패널티 적용
            gameManager.memberHealth[memberIndex] += currentExpedition.failHealthPenalty;
            gameManager.memberHunger[memberIndex] += currentExpedition.failHungerPenalty;
            gameManager.memberBodyTemp[memberIndex] += currentExpedition.failTempPenalty;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} 실패! (성공률 : {finalSuccessRate}%)\n" +
                $"체력 - {currentExpedition.failHealthPenalty}, 배고픔 - {currentExpedition.failHungerPenalty}, " +
                $" 온도 - {currentExpedition.failTempPenalty}";

            resultText.color = Color.red;
        }

        //최소값 보정

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

    void InititalizeEquipmentDurability()                               //장비 내구도 셋팅 하는 횟수
    {
        equipmentDurability = new int[availableEquipments.Length];                  //장비 숫자 만큼 배열 선언 ( 동적 선언 )

        for (int i = 0; i < availableEquipments.Length; i++)
        {
            equipmentDurability[i] = availableEquipments[i].maxDurability;           //사용가능한 내구도를 배열에 넣어준다.
        }
    }

    void SetupEquipementDropDown()
    {
        equipmentDropdown.ClearOptions();                  //드롭다운 초기화

        //장비 옵션들을 드롭 다운에 추가
        for (int i = 0; i < availableEquipments.Length; i++)
        {
            string equipName = availableEquipments[i].equipmentName;

            //내구도가 0이면 (부러진 표시) , 맨손 (인덱스 0)은 제외
            if (i==0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData(equipName));

            }
            else if (equipmentDurability[i] <= 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} (부러진"));

            }
            else
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} ({equipmentDurability[i]} / {availableEquipments[i].maxDurability})"));
            }
        }

        equipmentDropdown.value = 0;                  //기본값으로 첫번째 장비 선택
        equipmentDropdown.RefreshShownValue();       //데이터 변경이 있을 경우 보여지는 값을 함수를 통해 리셋 한다.

    }


}
