using UnityEngine;
using System.Collections;

public class TestUIController : MonoBehaviour
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
        if(GameManager.Instance.SelectedObj == null)
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
        GameManager.Instance.Action(typeof(MoveActor));
    }

    public void AttackTest()
    {
        GameManager.Instance.Action(typeof(AttackActor));
    }

    public void CancelTest()
    {
        GameManager.Instance.CancelAction();
    }
}
