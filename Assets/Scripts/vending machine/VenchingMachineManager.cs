using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;



public class VenchingMachineManager : MonoBehaviour
{
    [Header("����� ���ø�")]
    public DrinkSO[] Drinks;     //���� ����



    [Header("���Ǳ� ��ư")]
    public Button[] drinkButton;   //���� ��ư��

    [Header("���� UI")]
    public Text[] drinksPriceText;      //���� ���� UI
    public Text inventoryText;         //�÷��̾� �κ��丮 
    public Text insertMoney;           //���Ǳ⿡ ���Ե� �ݾ�
    public Text changeText;          //�Ž����� UI


    [Header("�÷��̾� �� ���ø�")]
    public PlayerMoneySO[] gold;             //ȭ�� ����
    public PlayerMoneySO[] count;          //ȭ�� ����
    //public PlayerMoneySO[] goldtype;        //ȭ�� ����  �̰� �� �־����� ���̴�... �̰� �־ �ǹ̰� �������ٵ�

    public int insertedMoney = 0;            //���Ǳ⿡ ���� �� �� �ݾ�

    [Header("��� ��ư")]
    public Button[] goldButton;





    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();


        for (int i = 0; i < drinkButton.Length; i++)
        {
            int drinksindex = i; // ���� ����
            drinkButton[i].onClick.AddListener(() => BuyDrink(drinksindex));

            
        }

        // => ���� ��  for (int i = 0; i < drinksButton.Length && i < gold.LongLength; i++)
        // {
        //     int goldindex = i;
        //    drinksButton[i].onClick.AddListener(() => UseGold(goldindex));

        //  } ���� �ٺξ� �� ���� ��ư�� �����µ� �����°� �ƴ϶� ��尡 ����ž־־־֤��޾֤�
        //�׷��ϱ� ���� ��ư ������ �� ��尡 ���屸��....... ����...

        for (int i = 0; i < goldButton.Length; i++)
        {
            int goldindex = i;
            goldButton[i].onClick.AddListener(() => InsertGold(goldindex));      //��� ��ư Ŭ�� �� InsertGold ȣ��

        }

        //���� �ڵ� �����ؼ� ���� ������! �´� �� ����!
        //���ٽ��� ����ؼ� ������ �ڵ尡 �����Ǵ� �� ���Ƽ� ������ ����� ����.
        //ó���� �� ���� �� �ڵ带 ���ٰ� ����Ƽ�� �������ָ鼭 ȥ�� �س����ϱ� �� �Ӹ��ӿ� �� ������ ����
        //������ �ڵ� ���鼭 õõ�� �Ϸ��ϴϱ� �Ѱ谡 �� �ֱ� �ߴµ� ��� ���� �� �ߴ� �ڵ���� �־���
        //�ٵ� ��¥ �Լ� �ʹ� ���� ���� �ʿ䰡 ������
        //���� �ڵ嵵 �����ͷ� ���������� �� ������� ��

    }

  

    public void InsertGold(int goldIndex)
    {

        // if (goldIndex <= 0) return;
        //  if (count[goldIndex] == null || count[goldIndex].count<= 0) return;

        //  count[goldIndex].count--;                        
        // BuyDrink(goldIndex, Drinks[goldIndex]);                
        // UpdateUI(); 

        // ���� �ڵ�...������� �����͵� ���� �־�� ������� ��� �̷��� 
        //����� ��ư ������ ���� á���� �� ����.


        if (goldIndex < 0 || count[goldIndex] == null) return;
       
        if (count[goldIndex].count <= 0) return;                    //�ݾ� ����

        insertedMoney += gold[goldIndex].gold;                    //�ݾ� ����
        count[goldIndex].count--;                              //�ݾ� ���

        UpdateUI();


    }





    public void BuyDrink(int drinkIndex)     // ���� ����
    {
        //if (goldName[goldIndex] <= 0) return;
        if (drinkIndex <= 0 || drinkIndex >= Drinks.Length) return;  //��ȿ�� �ε������� Ȯ��

        DrinkSO drink = Drinks[drinkIndex];  //���� ���� ��������
        //PlayerMoneySO playerMoney = gold[goldIndex]; �÷��̾� ���� �ƴ϶� ���� ����

        if (insertedMoney >= drink.price) // > ���Ե� �ݾ��� ���� ���ݺ��� ũ�ų� ������ ���� ����
        {
            insertedMoney -= drink.price; //���Ե� �ݾ׿��� ���� ���� ����


            if (insertedMoney > 0)      //���Ե� �ݾ��� 0���� ũ��
            {
                if (changeText != null)
                    changeText.text = $"�Ž��� �� : {insertedMoney}��"; //�Ž����� UI ������Ʈ

                InventortyGold(insertedMoney); // ����� �ݾ��� �ٽ� �κ��丮��

                insertedMoney = 0;
            }
            else
            {
                // �Ž������� ���ٸ� UI�� �� ������
                if (changeText != null)
                    changeText.text = $"�Ž�����: ����";
            }
        }
        else
        {
            // �ݾ��� �����ϸ� ���� ����
            

            if (changeText != null)
                changeText.text = "�ݾ��� �����մϴ�!";
        }

        // UI ����
        UpdateUI();


        //��带 �����ؾ���.               // ��带 ���� ������ �����ͷ� Ȯ���� ������

        // ���� �ڵ� = goldIndex -= drink.price; -> �ε��� ���� ���̴°� �ƴ� �ٵ� �� �ε��� ���� �ٿ���...
        //�׳� ������ ���Ը� �ǰ� �ؼ� ���� ��� ������... 
        //���� á���� �� á���� Ȯ���ϰ� ���ִ� �ڵ带 �� ������.
    }

    void InventortyGold(int changeAmount)     //�Ž������� �κ��� �ִ� �Լ�
    {
        for (int i = gold.Length - 1; i >= 0; i--)   //ū �ݾ׺��� ������ ��
        {
            int denom = gold[i].gold;               // ���� ȭ�� ����
            int countToAdd = changeAmount / denom;       // �Ž������� ���� ȭ�� ������ �� �� �� �� �ִ��� ���

            if (countToAdd > 0)
            {
                count[i].count += countToAdd;          //�÷��̾� �κ��丮�� �ݾ� �߰�
                changeAmount -= countToAdd * denom;     // ���� �Ž����� ���
            } 
        }
    }
    //����ϴ� ���� �� �� �����ؾ� �� �� ����. 
    //���� �ڵ� => for (int i = 0; i < gold.Length; i++)
    // int gold = gold[i].gold; // ���� ȭ�� ����
    //���⼭ ������...  

    public void UseGold(int goldIndex)               //��� ���
    {

        if (goldIndex < 0 || gold[goldIndex] == null) return;
        if (gold[goldIndex].gold == 0) return;


        //if (goldIndex <= 0 || Gold500 == null) return;
        //if (goldIndex <= 0 || Gold1000 == null) return;   ������ ���鵵 ���ε��� �߾��� 
        // �̷��� PlayerMoneySO�� ����� ������ ����. ���ʿ� �ڵ忡�� ���� �״µ� 

        PlayerMoneySO playerMoney = gold[goldIndex];

        playerMoney.gold--;

        UpdateUI();
    }




    //ó���� �ʱ�ȭ �ڵ带 �� �Ἥ ���� �� �� �ڱ��� �� ���� �����ʹ� OK
    public void UpdateUI()
    {
        inventoryText.text = "";             //ó���� �ʱ�ȭ �ڵ带 �� �Ἥ ���� �� �� �ڱ��� �� ���� �����ʹ� OK

        insertMoney.text = ""; // �ʱ�ȭ

        for (int i = 0; i < gold.Length; i++)
        {
            if (gold[i] != null)
            {
                inventoryText.text += $"{gold[i].goldName} : {count[i].count}��\n";

                
                // ���� �ڵ� =>insertMoney.text += $"{gold[i].gold}�� ���Ե�\n";
                //��� ������ ��ü�� ��� ����� 
            }

           
            // insertMoney.text = $"�� ���� �ݾ� : {insertMoney}��"; // �� �ڵ� if�� �տ� �ּ� UI�� �̻��ϰ� ����
            //�� �ƴ� �ٺ����Ʒ� �����ִµ� �� �� ����

        }

        insertMoney.text += $"\n�� ���� �ݾ�: {insertedMoney}��";

        // ���� ���� UI ������Ʈ
        for (int i = 0; i < drinksPriceText.Length; i++)
        {
            drinksPriceText[i].text = $"{Drinks[i].price} ��"; // ���� ���� �ʱ�ȭ
            //��ŸƮ�� �����ϱ� �ʱ�ȭ�� �� �Ǵ� �� ���ٰ� ��
            // ��¾�� ������ �� ��������
        }


        //��������. for (int i = 0; i < goldName.Length; i++)�ε�, Drinks[i], drinksPriceText[i] ������ ��
        //Drinks.Length�� drinksPriceText.Length�� goldName.Length���� ª���� �߻���.  �� UI�� �ٸ��� �ؾ��ϴµ� 
        //�����ϰ� �ΰ��� ���� ������ �ؼ� �� �� ����...... �ٵ� �̷��� �� �ϸ� ��� �ؾ����� �𸣰ڴ�..
        //�׳� ���� �ٺ� for�� �Ʒ����� �Ǵ� �ǵ� �� �ȿ� �־ �� �ƴ� �ſ���.

    }

}
        //���� �ڵ� =>  insertMoney.text += $"{gold}�� ���Ե�\n";  �ǳ� �ʱ�ȭ �� �ؼ� �ڲ� ���� ��.. ����











