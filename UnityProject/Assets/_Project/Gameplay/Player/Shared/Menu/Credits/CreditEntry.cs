using UnityEngine;


[CreateAssetMenu(
    fileName = "CreditEntry",
    menuName = "Credits/Credit Entry",
    order = 1)]
public class CreditEntry : ScriptableObject {
    [Header("Grouping")]
    public string section;   // e.g. "Textures & Materials"

    [Header("Display")]
    [TextArea(2, 4)]
    public string text;      // What the player sees
}
