using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(
    fileName = "CreditsDatabase",
    menuName = "Credits/Credits Database",
    order = 2)]
public class CreditsDatabase : ScriptableObject {
    public List<CreditEntry> entries = new();
}

