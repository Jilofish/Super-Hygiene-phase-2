using System.Collections.Generic;

[System.Serializable]
public class AreaTaskSaveData
{
    public Dictionary<string, Dictionary<string, int>> areaTaskProgress = new();
}