using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTest : MonoBehaviour
{
    int Number = 0;
    public GameObject temp;

    // Start is called before the first frame update
    void Start()
    {
        FuntionTest_01();

        int myNumber = 10;

        Number = FuntionTest_02(myNumber);

        
    }

    void FuntionTest_01()             //따로 파라미터나 리턴 값이 필요 없을 때
    {
        Number += 1;
    }

    int FuntionTest_02(int num)
    {
        return num + 10;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
