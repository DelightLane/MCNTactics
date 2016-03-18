using UnityEngine;
using System.Collections;

public class GameManager : MCN.Singletone<GameManager>
{
    private PlaceableObject _selectedObj;

    public PlaceableObject SelectedObj
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
