using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("그룹 구성원 템플릿")]
    public GroupMemberSO[] groupMembers;

    [Header("아이템 템플릿")]
    public ItemSO foodItem;              //음식 아이템SO
    public ItemSO fuelItem;              //연료 아이템SO
    public ItemSO medicineItem;         //의약품 아이템SO

    [Header("참조 UI")]
    public Text dayText;                                //날짜 표시 UI
    public Text[] memberStatusTexts;                    //맴버 상태 표시 UI
    public Button nextDayButton;                        //다음 날 버튼
    public Text inventoryText;                          //인벤토리 표시


    [Header("아이템 버튼")]
    public Button feedButton;         //음식 주기
    public Button heatButton;         //난방 하기
    public Button healButton;          //치료 하기



    [Header("게임 상태")]
    int currentDay;                          //현재 날짜
    public int food = 5;                     //음식 개수
    public int fuel = 3;                     //연료 개수
    public int medicine = 4;                // 의약품 개수

    [Header("특정 맴버아이템 소모 버튼")]
    public Button[] individualFoodButtons;
    public Button[] individualHealButtons;

    [Header("이벤트 시스템")]
    public EventSO[] events;                 //이벤트 목록
    public GameObject eventPopup;            //이벤트 팝업 패널
    public Text eventTitleText;              //이벤트 제목
    public Text eventDescriptionText;        //이벤트 설명
    public Button eventConfirmbutton;        //이벤트 닫기(확인) 버튼



    //런타임 데이터
    public int[] memberHealth;
    public int[] memberHunger;
    public int[] memberBodyTemp;


    // Start is called before the first frame update
    void Start()
    {

        currentDay = 1;


        InitializeGroup();
        UpdateUI();

        nextDayButton.onClick.AddListener(NextDay);         //다음 날 버튼 클릭 시 NextDay 함수 호출
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
        int memberCount = groupMembers.Length;                //그룹 맴버의 길이 만큼 인원 수 할당
        memberHealth = new int[memberCount];                  //그룹 맴버 길이 만큼 배열 할당
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

        inventoryText.text = $"음식   : {food}개\n" +
                             $"연료   : {fuel}개\n" +
                             $"의약품 : {medicine}개\n";

        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberStatusTexts[i] != null)
            {
                GroupMemberSO member = groupMembers[i];

                string status = GetMemberStatus(i);

                memberStatusTexts[i].text =
                    $"{member.memberName} {status} \n" +
                    $"체력   : {memberHealth[i]} \n" +
                    $"배고픔 : {memberHunger[i]} \n" +
                    $"체온   : {memberBodyTemp[i]} 도";
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

            //나이에 따른 배고픔 조정
            float hungerMltiplier = member.ageGroup == GroupMemberSO.AgeGroup.Child ? 0.8f : 1.0f;

            //상태 감소
            memberHunger[i] -= Mathf.RoundToInt(baseHungerLoss * hungerMltiplier);              //맴버별 배고픔 저항 설정
            memberBodyTemp[i] -= Mathf.RoundToInt(baseTempLoss * member.coldResistance);       //멤버별 추위 저항력


            //건강 체크
            if (memberHunger[i] <= 0) memberHunger[i] -= 15;
            if (memberBodyTemp[i] <= 32) memberHealth[i] -= 10;
            if (memberBodyTemp[i] <= 30) memberHealth[i] -= 20;

            //최소값 제한
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
        //사망 체크
        if (memberHealth[memberIndex] <= 0)
            return "(사망)";

        //가장 위험한 상태부터 체크
        if (memberBodyTemp[memberIndex] <= 30) return "(심각한 저체온증)";
        else if (memberHealth[memberIndex] <= 20) return "(위험)";
        else if (memberHunger[memberIndex] <= 10) return "(굶주림)";
        else if (memberBodyTemp[memberIndex] <= 32) return "(저체온증)";
        else if (memberHealth[memberIndex] <= 50) return "(약함)";
        else if (memberHunger[memberIndex] <= 30) return "(배고픔)";
        else if (memberBodyTemp[memberIndex] <= 35) return "(추위)";
        else return "(건강)";


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
            Debug.Log("게임 오버! 모든 구성원이 혹독한 상황을 이겨내지 못했습니다.");
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

    public void UseFoodItem()                                         //음식 아이템 사용
    {
        if (food <= 0 || foodItem == null) return;                   //오류 방지 처리

        food--;
        UseItemOnAllMembers(foodItem);
        UpdateUI();
    }

    public void UseFuelItem()                                         //음식 아이템 사용
    {
        if (fuel <= 0 || fuelItem == null) return;                   //오류 방지 처리

        fuel--;
        UseItemOnAllMembers(fuelItem);
        UpdateUI();
    }

    public void UseMedicItem()                                         //음식 아이템 사용
    {
        if (medicine <= 0 || medicineItem == null) return;                   //오류 방지 처리

        medicine--;
        UseItemOnAllMembers(medicineItem);
        UpdateUI();
    }

    void UseItemOnAllMembers(ItemSO item)
    {
        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberHealth[i] > 0)                    //살아있는 가족만
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

        //개인 특성 적용해서 아이템 효과 계산
        int actualHealth = Mathf.RoundToInt(item.healthEffect * member.recoveryRate);
        int actualHunger = Mathf.RoundToInt(item.hungerEffect * member.foodEfficiency);
        int actualTemp = item.tempEffect;

        //효과 적용
        memberHealth[memberIndex] += actualHealth;
        memberHunger[memberIndex] += actualHunger;
        memberBodyTemp[memberIndex] += actualTemp;

        //최대치 제한
        memberHealth[memberIndex] = Mathf.Min(memberHealth[memberIndex], member.maxHealth);
        memberHunger[memberIndex] = Mathf.Min(memberHunger[memberIndex], member.maxHunger);
        memberBodyTemp[memberIndex] = Mathf.Min(memberBodyTemp[memberIndex], member.normalBodyTemp);

    }

    void ApplyEventEffects(EventSO eventSO)
    {
        //자원변화
        food += eventSO.foodChange;
        fuel += eventSO.fuelChange;
        medicine += eventSO.medicineChange;

        //자원 최소값 보정
        food = Mathf.Max(0, food);
        fuel = Mathf.Max(0, fuel);
        medicine = Mathf.Max(0, medicine);

        //모든 살아있는 멤버에게 상태 변화적용
        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberHealth[i] > 0)
            {
                memberHealth[i] += eventSO.healthChange;
                memberHunger[i] += eventSO.hungerChange;
                memberBodyTemp[i] += eventSO.tempChange;

                //제한값 적용
                GroupMemberSO member = groupMembers[i];
                memberHealth[i] = Mathf.Clamp(memberHealth[i], 0, member.maxHealth);
                memberHunger[i] = Mathf.Clamp(memberHunger[i], 0, member.maxHunger);
                memberBodyTemp[i] = Mathf.Clamp(memberBodyTemp[i], 0, member.normalBodyTemp);
            }
        }
    }

    void ShowEventPopup(EventSO eventSO)
    {
        //팝업 활성화
        eventPopup.SetActive(true);

        //텍스트 설정
        eventTitleText.text = eventSO.eventTitel;
        eventDescriptionText.text = eventSO.eventDescription;

        //이벤트 효과 적용
        ApplyEventEffects(eventSO);

        //게임 진행 일시정지
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

        //전체 확률 합 구하기
        for (int i = 0; i < events.Length; i++)
        {
            totalProbability += events[i].probability;
        }

        if (totalProbability == 0)
            return;

        int roll = Random.Range(1, totalProbability + 1 + 50);            //전체확률 더하기에 아무것도 없을 확률 50
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
