using UnityEngine;
using System.Collections;

public class GameManager : MCN.Singletone<GameManager>
{
    private TacticsObject _selectedObj;

    public TacticsObject SelectedObj
    {
        get
        {
            if(_selectedObj is MCN.DecoInstance)
            {
                return (_selectedObj as MCN.DecoInstance).Get;
            }

            return _selectedObj;
        }

        set
        {
            _selectedObj = value;
        }
    }

    private GameManager(){}
}
