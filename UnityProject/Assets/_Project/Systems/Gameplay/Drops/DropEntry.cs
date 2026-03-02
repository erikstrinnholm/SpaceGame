
//Responsibilities: One possible roll in a drop table
[System.Serializable]
public class DropEntry {
    public LootDefinition loot;   // item, resource, currency
    public float weight;
    public int minAmount;
    public int maxAmount;
}
