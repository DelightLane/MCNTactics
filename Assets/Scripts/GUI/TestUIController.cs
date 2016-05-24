using UnityEngine;
using System.Collections;

public class TestUIController : MonoBehaviour
{ 
    public void MoveTest()
    {
        var selected = GameManager.Instance.SelectedObj as UnitObject;

        if(selected != null)
        {
            selected.EnqueueActor(typeof(MoveActor));

            selected.OnTouchEvent(eTouchEvent.TOUCH);
        }
    }

    public void AttackTest()
    {
        var selected = GameManager.Instance.SelectedObj as UnitObject;

        if (selected != null)
        {
            selected.EnqueueActor(typeof(AttackActor));
            
            selected.OnTouchEvent(eTouchEvent.TOUCH);
        }
    }
}
