using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAreaConfig", menuName = "Task System/Area Config")]
public class AreaConfig : ScriptableObject
{
    public string areaID;
    public List<TaskDefinition> tasks;
}
