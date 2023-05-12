using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private GameManager _gameManager;
    private GameObject _objPlayer;
    private void OnEnable()
    {
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void FindObjPlayer()
    {
        if (!_objPlayer)
        {
            _objPlayer = GameObject.Find("Level " + _gameManager.curLvl + "/Player");
        }
    }
}
