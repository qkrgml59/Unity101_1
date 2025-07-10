using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JsonSaveExample : MonoBehaviour
{

    [Header("UI")]
    public InputField nameInput;                     //�̸� �Է� UI
    public Text levelText;                           //���� �ؽ�Ʈ
    public Text goldText;                            //�� �ؽ�Ʈ
    public Text playerTimeText;                      //�÷��̾� �ð� �ؽ�Ʈ
    public Button saveButton;                          //���̺� ��ư
    public Button loadButton;                         //�ε� ��ư

    PlayerData playerData;                //�÷��̾� ������ Ŭ���� ����
    string saveFilePath;                 //���� ��� Ȯ�ο� 


    // Start is called before the first frame update
    void Start()
    {
        //�������� ��� ����
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");

        //������ �ʱ�ȭ
        playerData = new PlayerData();
        playerData.playerName = "���ο� �÷��̾�";
        playerData.level = 1;
        playerData.gold = 100;
        playerData.playtime = 0f;
        playerData.position = Vector3.zero;

        saveButton.onClick.AddListener(SaveToJson);
        loadButton.onClick.AddListener(LoadFromJSON);

        //�ڵ� �ε�
        LoadFromJSON();
        UpdateUI();

        Debug.Log(saveFilePath);
    }

    // Update is called once per frame
    void Update()
    {
        playerData.playtime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.L))
        {
            playerData.level++;
            playerData.gold += 50;
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            playerData.gold += 10;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        nameInput.text = playerData.playerName;
        levelText.text = "Lv : " + playerData.level;
        goldText.text = "Gold : " + playerData.gold;
        playerTimeText.text = "PlayTime : " + playerData.playtime;
    }

    void SaveToJson()
    {
        playerData.playerName = nameInput.text;

        string jsonData = JsonUtility.ToJson(playerData, true);

        File.WriteAllText(saveFilePath, jsonData);

        Debug.Log("���� �Ϸ�");
    }

    void LoadFromJSON()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);

            playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            Debug.Log("�ҷ����� �Ϸ�");
        }
        else
        {
            Debug.Log("���� �����̾����ϴ�.");
        }

        UpdateUI();
}
}
