using UnityEngine;
using System.Collections;

public class GameManager : FZ.Singletone<GameManager>
{
    private SelectHandler _selectHandler;

    private GameManager()
    {
        _selectHandler = new SelectHandler();
    }

    #region SelectHandler
    private class SelectHandler
    {
        private TacticsObject _selectedObj;

        public void Set(TacticsObject obj)
        {
            _selectedObj = obj;
        }

        public TacticsObject Get()
        {
            return _selectedObj;
        }

        public void Action(System.Type actType)
        {
            var selected = _selectedObj as ActionObject;

            if (selected != null)
            {
                selected.EnqueueActor(actType);
            }
        }

        public void CancelAction()
        {
            var selected = _selectedObj as ActionObject;

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

    public TacticsObject SelectedObj
    {
        get
        {
            return _selectHandler.Get();
        }

        set
        {
            _selectHandler.Set(value);
        }
    }

    public void Action(System.Type actType)
    {
        _selectHandler.Action(actType);
    }

    public void CancelAction()
    {
        _selectHandler.CancelAction();
    }
    #endregion
}
