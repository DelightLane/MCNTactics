using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitDebugUI : MonoBehaviour
{
    Text _name;
    Text _team;
    Text _actorQueue;

    void Start()
    {
        _name = transform.GetChild(0).GetComponent<Text>();
        _team = transform.GetChild(1).GetComponent<Text>();
        _actorQueue = transform.GetChild(2).GetComponent<Text>();
    }

	void Update ()
    {
        DisplayUnitInfo();
    }

    private void DisplayUnitInfo()
    {
        var unit = GameManager.Get<GameManager.Select>().Target as UnitObject;
        
        if(unit != null)
        {
            string name = unit.UnitName;
            string team = unit.Team.ToString();
            string actions = GetQueueActorNames(unit);

            _name.text = string.Format("name : {0}", name);
            _team.text = string.Format("team : {0}", team);
            _actorQueue.text = string.Format("ActorQueue : {0}", actions);
        }
        else
        {
            _name.text = "name :";
            _team.text = "team :";
            _actorQueue.text = "ActorQueue :";
        }
    }

    private string GetQueueActorNames(UnitObject unit)
    {
        string queueActors = string.Empty;

        foreach(var actor in unit.GetQueueActorNames())
        {
            if(queueActors == string.Empty)
            {
                queueActors = actor;
                continue;
            }

            queueActors = string.Format("{0}, {1}", queueActors, actor);
        }

        return queueActors;
    }
}
