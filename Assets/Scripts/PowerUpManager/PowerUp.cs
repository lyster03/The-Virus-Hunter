using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : ScriptableObject
{
    public Sprite icon;
    public string title;

    [Header("Description (Shown in UI)")]
    [TextArea]
    public string description;

    public abstract void Apply(GameObject target);
}
