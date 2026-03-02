using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Responsibilities: Defines what can drop and how often
[CreateAssetMenu(menuName = "Loot/Drop Table")]
public class DropTable : ScriptableObject {
    public List<DropEntry> entries;
    public int rolls = 1;
}

