using UnityEngine;
using System.Collections;

public class TestUIController : MonoBehaviour
{
    private GameObject _moveBtn;
    private GameObject _atkBtn;

    void Start()
    {
        _moveBtn = GameObject.Find("MoveBtn");
        _atkBtn = GameObject.Find("AttackBtn");
    }

    void Update()
    {
        if(GameManager.Instance.SelectedObj == null)
        {
            _moveBtn.SetActive(false);
            _atkBtn.SetActive(false);
        }
        else
        {
            _moveBtn.SetActive(true);
            _atkBtn.SetActive(true);
        }
    }

    public void MoveTest()
    {
        GameManager.Instance.Action(typeof(MoveActor));
    }

    public void AttackTest()
    {
        GameManager.Instance.Action(typeof(AttackActor));
    }
}
