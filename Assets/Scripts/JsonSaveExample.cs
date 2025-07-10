using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JsonSaveExample : MonoBehaviour
{

    [Header("UI")]
    public InputField nameInput;                     //이름 입력 UI
    public Text levelText;                           //레벨 텍스트
    public Text goldText;                            //돈 텍스트
    public Text playerTimeText;                      //플레이어 시간 텍스트
    public Button saveButton;                          //세이브 버튼
    public Button loadButton;                         //로드 버튼

    PlayerData playerData;                //플레이어 데이터 클래스 선언
    string saveFilePath;                 //저장 경로 확인용 


    // Start is called before the first frame update
    void Start()
    {
        //저장파일 경로 설정
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");

        //데이터 초기화
        playerData = new PlayerData();
        playerData.playerName = "새로운 플레이어";
        playerData.level = 1;
        playerData.gold = 100;
        playerData.playtime = 0f;
        playerData.position = Vector3.zero;

        saveButton.onClick.AddListener(SaveToJson);
        loadButton.onClick.AddListener(LoadFromJSON);

        //자동 로드
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

        Debug.Log("저장 완료");
    }

    void LoadFromJSON()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);

            playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            Debug.Log("불러오기 완료");
        }
        else
        {
            Debug.Log("저장 파일이없습니다.");
        }

        UpdateUI();
}
}
