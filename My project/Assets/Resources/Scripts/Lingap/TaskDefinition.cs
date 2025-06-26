using System;
using UnityEngine;

[Serializable]
public class TaskDefinition
{
    public string taskID;          // Unique task identifier
    [TextArea] public string description; // UI-friendly description
    public int goalCount = 1;      // How many times this task must be completed
}
