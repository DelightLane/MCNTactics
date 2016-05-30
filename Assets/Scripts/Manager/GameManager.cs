using UnityEngine;
using System.Collections;

public class GameManager : FZ.Singletone<GameManager>
{
    private TacticsObject _selectedObj;

    public TacticsObject SelectedObj
    {
        get
        {
            return _selectedObj;
        }

        set
        {
            _selectedObj = value;
        }
    }

    private GameManager() { }

    public void Action(System.Type actType)
    {
        var selected = GameManager.Instance.SelectedObj as ActionObject;

        if (selected != null)
        {
            selected.EnqueueActor(actType);
        }
    }

    public void CancelAction()
    {
        var selected = GameManager.Instance.SelectedObj as ActionObject;

        if (selected != null)
        {
            if (selected.HasActor())
            {
                selected.DequeueActor();
            }
            else
            {
                selected.Deselect();
            }
        }
    }
}
