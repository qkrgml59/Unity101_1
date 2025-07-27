using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;



public class VenchingMachineManager : MonoBehaviour
{
    [Header("음료수 템플릿")]
    public DrinkSO[] Drinks;     //음료 정보



    [Header("자판기 버튼")]
    public Button[] drinkButton;   //음료 버튼들

    [Header("참조 UI")]
    public Text[] drinksPriceText;      //음료 가격 UI
    public Text inventoryText;         //플레이어 인벤토리 
    public Text insertMoney;           //자판기에 투입된 금액
    public Text changeText;          //거스름돈 UI


    [Header("플레이어 돈 템플릿")]
    public PlayerMoneySO[] gold;             //화폐 종류
    public PlayerMoneySO[] count;          //화폐 갯수
    //public PlayerMoneySO[] goldtype;        //화폐 종류  이걸 왜 넣었을까 희연이는...

    public int insertedMoney = 0;            //자판기에 투입 된 총 금액

    [Header("골드 버튼")]
    public Button[] goldButton;





    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();

        for (int i = 0; i < drinkButton.Length; i++)
        {
            int drinksindex = i; // 꼭 지역 변수로 저장! (클로저 문제 방지)
            drinkButton[i].onClick.AddListener(() => BuyDrink(drinksindex));

            if (drinksPriceText[i] == null)
            {
                Debug.LogWarning($"priceText[{i}] is null!");
            }
            else
            {
                drinksPriceText[i].text = Drinks[i].name + "\n" + Drinks[i].price + "원";
            }
        }

        // => 수정 전  for (int i = 0; i < drinksButton.Length && i < gold.LongLength; i++)
        // {
        //     int goldindex = i;
        //    drinksButton[i].onClick.AddListener(() => UseGold(goldindex));

        //  } 박희연 바부야 왜 음료 버튼을 누르는데 사지는게 아니라 골드가 실행돼애애애애ㅐ앵애ㅐ
        //그러니까 구매 버튼 눌렀을 때 골드가 생겼구나....... 하하...

        for (int i = 0; i < goldButton.Length; i++)
        {
            int goldindex = i;
            goldButton[i].onClick.AddListener(() => InsertGold(goldindex));      //골드 버튼 클릭 시 InsertGold 호출

        }

        //위에 코드 참고해서 내가 적었음! 맞는 거 같다!
        //람다식을 사용해서 복잡한 코드가 정리되는 거 같아서 눈으로 보기는 좋다.
        //처음에 막 적은 내 코드를 보다가 지피티가 수정해주면서 혼자 해나가니까 좀 머리속에 더 들어오는 느낌
        //교수님 코드 보면서 천천히 하려하니까 한계가 좀 있긴 했는데 몇개는 수정 안 했던 코드들이 있어서 기분은 좋았다.

    }

    public void InsertGold(int goldIndex)
    {

        // if (goldIndex <= 0) return;
        //  if (count[goldIndex] == null || count[goldIndex].count<= 0) return;

        //  count[goldIndex].count--;                        
        // BuyDrink(goldIndex, Drinks[goldIndex]);                
        // UpdateUI(); 

        // 이전 코드...어거지로 데이터들 끼리 넣어보고 어거지로 우김 이래서 
        //음료수 버튼 누르면 돈이 찼었던 거 같음.


        if (goldIndex < 0 || count[goldIndex] == null) return;
        if (count[goldIndex].count <= 0) return;                    //금액 누적

        insertedMoney += gold[goldIndex].gold;                    //금액 누적
        count[goldIndex].count--;                              //금액 사용

        UpdateUI();


    }





    public void BuyDrink(int drinkIndex)     // 음료 구매
    {
        //if (goldName[goldIndex] <= 0) return;
        if (drinkIndex <= 0 || drinkIndex >= Drinks.Length) return;  //유효한 인덱스인지 확인

        DrinkSO drink = Drinks[drinkIndex];  //음료 정보 가져오기
        //PlayerMoneySO playerMoney = gold[goldIndex]; 플레이어 돈이 아니라 음료 정보

        if (insertedMoney >= drink.price) // > 투입된 금액이 음료 가격보다 크거나 같으면 구매 가능
        {
            insertedMoney -= drink.price; //투입된 금액에서 음료 가격 차감


            if (insertedMoney > 0)
            {
                if (changeText != null)
                    changeText.text = $"거스름 돈 : {insertedMoney}원"; //거스름돈 UI 업데이트

                insertedMoney = 0;
            }
            else
            {
                // 거스름돈이 없다면 UI를 빈 값으로
                if (changeText != null)
                    changeText.text = $"거스름돈: 없음";
            }
        }
        else
        {
            // 금액이 부족하면 구매 실패
            Debug.Log("금액 부족! 구매 실패");

            if (changeText != null)
                changeText.text = "금액이 부족합니다!";
        }

        // UI 갱신
        UpdateUI();


        //골드를 차감해야함.               // 골드를 음료 가격을 데이터로 확인해 차감함

        // 이전 코드 = goldIndex -= drink.price; -> 인덱스 값을 줄이는게 아님 근데 난 인덱스 값을 줄였음...
    }




    public void UseGold(int goldIndex)               //골드 사용
    {

        if (goldIndex < 0 || gold[goldIndex] == null) return;
        if (gold[goldIndex].gold == 0) return;


        //if (goldIndex <= 0 || Gold500 == null) return;
        //if (goldIndex <= 0 || Gold1000 == null) return;

        PlayerMoneySO playerMoney = gold[goldIndex];

        playerMoney.gold--;

        UpdateUI();
    }




    //처음에 초기화 코드를 안 써서 시작 할 때 자금이 안 떴음 데이터는 OK
    public void UpdateUI()
    {
        inventoryText.text = "";             //처음에 초기화 코드를 안 써서 시작 할 때 자금이 안 떴음 데이터는 OK

        insertMoney.text = ""; // 초기화

        for (int i = 0; i < gold.Length; i++)
        {
            if (gold[i] != null)
            {
                inventoryText.text += $"{gold[i].goldName} : {count[i].count}개\n";
                insertMoney.text += $"{gold[i].gold}원 투입됨\n";
            }
        }

        insertMoney.text += $"\n총 투입 금액: {insertedMoney}원";

        // 음료 가격 UI 업데이트
        for (int i = 0; i < Drinks.Length && i < drinksPriceText.Length; i++)
        {
            if (Drinks[i] != null && drinksPriceText[i] != null)
            {
                drinksPriceText[i].text = $"{Drinks[i].name}\n{Drinks[i].price} 원";
            }
        }

        //오류났음. for (int i = 0; i < goldName.Length; i++)인데, Drinks[i], drinksPriceText[i] 접근할 때
        //Drinks.Length나 drinksPriceText.Length가 goldName.Length보다 짧으면 발생함.  두 UI를 다르게 해야하는데 
        //무리하게 두개를 같이 쓰려고 해서 난 거 같다...... 근데 이렇게 안 하면 어떻게 해야할지 모르겠다..
        //그냥 내가 바보 for문 아래쓰면 되는 건데 그 안에 넣어서 안 됐던 거였다.

    }

}
        //이전 코드 =>  insertMoney.text += $"{gold}원 투입됨\n";  맨날 초기화 안 해서 자꾸 돈이 참.. 버그











