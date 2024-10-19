using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skin Data", menuName = "RunningPingu/Skins/New Skin")]
public class SkinData : ScriptableObject
{
    [Header("Data")]
    public string id;
    public Sprite icon;
    public int buyPrice = 100;

    private void GenerateRandomGuid() => id = Guid.NewGuid().ToString();

    private void OnValidate()
    {
        if (id == string.Empty)
            GenerateRandomGuid();
    }
}
