using UnityEngine;
using System.Collections.Generic;

public enum TapObjectType
{
    Plate2,
    Spoon,
    Fork,
    Cup,
    Glass
}

public class ArrangementSequenceManager : MonoBehaviour
{
    public static ArrangementSequenceManager Instance;

    private int step = 0;
    private HashSet<TapObjectType> utensilsTapped = new(); // Spoon/Fork checker

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool CanTap(TapObjectType type)
    {
        switch (step)
        {
            case 0: return type == TapObjectType.Plate2;
            case 1:
                if (type == TapObjectType.Spoon || type == TapObjectType.Fork)
                {
                    utensilsTapped.Add(type);
                    if (utensilsTapped.Contains(TapObjectType.Spoon) && utensilsTapped.Contains(TapObjectType.Fork))
                        step++;
                    return true;
                }
                return false;
            case 2: return type == TapObjectType.Cup;
            case 3: return type == TapObjectType.Glass;
            default: return false;
        }
    }

    public void AdvanceStepIfNeeded(TapObjectType type)
    {
        if (step == 0 && type == TapObjectType.Plate2) step++;
        else if (step == 2 && type == TapObjectType.Cup) step++;
        else if (step == 3 && type == TapObjectType.Glass) step++;
    }
}
