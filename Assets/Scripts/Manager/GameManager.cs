using UnityEngine;
using System.Collections;

public class GameManager : MCN.Singletone<GameManager>
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

    private GameManager(){}
}
