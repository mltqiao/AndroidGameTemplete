using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DataController _dataController;
    private TestModeManager _testModeManager;
    [HideInInspector]
    public int curLvl;
    [HideInInspector]
    public int curMoney;
    
    private void OnEnable()
    {
        _dataController = GameObject.Find("Data Controller").GetComponent<DataController>();
        _testModeManager = transform.GetComponent<TestModeManager>();
        if (_testModeManager.testMode)
        {
            curLvl = _testModeManager.testLvl;
            PlayerPrefs.SetInt("CurLvl",curLvl);
            curMoney = _testModeManager.testMoney;
            PlayerPrefs.SetInt("CurMoney",curMoney);
        }
        else
        {
            curLvl = PlayerPrefs.GetInt("CurLvl",1);
            curMoney = PlayerPrefs.GetInt("CurMoney",0);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        InstantiateLvl(curLvl);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void InstantiateLvl(int lvlNum)
    {
        if (lvlNum != 0)
        {
            if (lvlNum % _dataController.maxLevelNumber == 0)
            {
                lvlNum = _dataController.maxLevelNumber;
            }
            else
            {
                lvlNum = lvlNum % _dataController.maxLevelNumber;
            }
            var objCurLevel = (GameObject)Instantiate(Resources.Load("LevelData/Level " + lvlNum));
            objCurLevel.name = "Level " + lvlNum;
        }
    }
}
