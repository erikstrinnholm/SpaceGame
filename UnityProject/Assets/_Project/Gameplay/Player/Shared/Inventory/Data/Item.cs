using UnityEngine;

//This represents what the player currently owns
[System.Serializable]
public class Item {
    public ItemData data;
    public int count = 1;

    public int StackValue() {
        return data.value * count;
    }

    public Item(ItemData data, int count = 1) {
        this.data = data;
        this.count = count;
    }
}
