using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplePlayerPrefabs : MonoBehaviour
{
    public InputField nameInput;
    public Text scoreText;
    public Button saveButton;
    public Button loadButton;

    int currentScore = 0;                 //현재 스코어




    // Start is called before the first frame update
    void Start()
    {
        saveButton.onClick.AddListener(SaveData);                        //세이브 버튼 눌렀을 때 SaveData 함수 실행
        loadButton.onClick.AddListener(LoadData);                        //로드 데이터 버튼 눌렀을 때 LoadData 함수 실행

        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentScore += 10;
            scoreText.text = "Score " + currentScore;
        }
    }

    void SaveData()            //데이터 저장 함수
    {
        PlayerPrefs.SetString("PlayerName", nameInput.text);          //플레이어의 이름을 UI로 입력받아서 "PlayerName" 이름 지은 곳에 저장
        PlayerPrefs.SetInt("HighScore", currentScore);        //현재 스코어를 "HighScore" 이름 지은 키로 저장
        PlayerPrefs.Save();

        Debug.Log("저장 완료");
    }

    void LoadData()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "PlayerName");     //PlayerName 키에서 데이터를 가져온다
        int savedScore = PlayerPrefs.GetInt("HighScore", 0 );                      //HighScore 키에서 데이터를 가져온다

        nameInput.text = savedName;
        currentScore = savedScore;
        scoreText.text = "Score " + currentScore;

        Debug.Log("불러오기 완료");
    }

}
