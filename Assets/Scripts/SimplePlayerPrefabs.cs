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

    int currentScore = 0;                 //���� ���ھ�




    // Start is called before the first frame update
    void Start()
    {
        saveButton.onClick.AddListener(SaveData);                        //���̺� ��ư ������ �� SaveData �Լ� ����
        loadButton.onClick.AddListener(LoadData);                        //�ε� ������ ��ư ������ �� LoadData �Լ� ����

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

    void SaveData()            //������ ���� �Լ�
    {
        PlayerPrefs.SetString("PlayerName", nameInput.text);          //�÷��̾��� �̸��� UI�� �Է¹޾Ƽ� "PlayerName" �̸� ���� ���� ����
        PlayerPrefs.SetInt("HighScore", currentScore);        //���� ���ھ "HighScore" �̸� ���� Ű�� ����
        PlayerPrefs.Save();

        Debug.Log("���� �Ϸ�");
    }

    void LoadData()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "PlayerName");     //PlayerName Ű���� �����͸� �����´�
        int savedScore = PlayerPrefs.GetInt("HighScore", 0 );                      //HighScore Ű���� �����͸� �����´�

        nameInput.text = savedName;
        currentScore = savedScore;
        scoreText.text = "Score " + currentScore;

        Debug.Log("�ҷ����� �Ϸ�");
    }

}
