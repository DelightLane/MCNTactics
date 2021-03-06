﻿using UnityEngine;
using System.Collections;

public class TempActionUI : MonoBehaviour
{
    private GameObject _moveBtn;
    private GameObject _atkBtn;
    private GameObject _cancelBtn;

    void Start()
    {
        _moveBtn = GameObject.Find("MoveBtn");
        _atkBtn = GameObject.Find("AttackBtn");
        _cancelBtn = GameObject.Find("CancelBtn");
    }

    void Update()
    {
        if(GameManager.Get<GameManager.Select>().Target == null)
        {
            _moveBtn.SetActive(false);
            _atkBtn.SetActive(false);
            _cancelBtn.SetActive(false);
        }
        else
        {
            _moveBtn.SetActive(true);
            _atkBtn.SetActive(true);
            _cancelBtn.SetActive(true);
        }
    }

    public void MoveTest()
    {
        GameManager.Get<GameManager.Select>().Action(typeof(MoveActor));
    }

    public void AttackTest()
    {
        GameManager.Get<GameManager.Select>().Action(typeof(AttackActor));
    }

    public void CancelTest()
    {
        GameManager.Get<GameManager.Select>().CancelAction();
    }

    public void EndTurnTest()
    {
        GameManager.Get<GameManager.Turn>().ForceEndTurn();
    }
}
