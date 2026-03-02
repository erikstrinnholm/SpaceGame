using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentNavigator {

    //same as BuildSlotList
    private readonly int weaponSlot1 = 0;
    private readonly int weaponSlot2 = 1;
    private readonly int weaponSlot3 = 2;
    private readonly int weaponSlot4 = 3;

    private readonly int shipSystemSlot1 = 4;
    private readonly int shipSystemSlot2 = 5;
    private readonly int shipSystemSlot3 = 6;
    private readonly int shipSystemSlot4 = 7;
    private readonly int shipSystemSlot5 = 8;
    private readonly int shipSystemSlot6 = 9;

    private readonly int storageSlot1 = 10;
    private readonly int storageSlot2 = 11;
    private readonly int storageSlot3 = 12;
    private readonly int storageSlot4 = 13;
    
    private readonly int storageSlot5 = 14;
    private readonly int dropSlotL = 15;
    private readonly int dropSlotR = 16;
    private readonly int trashSlot = 17;


    public int GetNextIndex(int current, Vector2 dir) {
        if (dir.x < 0)  return MoveLeft(current);
        if (dir.x > 0)  return MoveRight(current);
        if (dir.y > 0)  return MoveUp(current);
        if (dir.y < 0)  return MoveDown(current);
        return current;
    }

    private int MoveLeft(int current) {
        if (current == weaponSlot1 || current == weaponSlot3 || current == storageSlot1) return dropSlotL;
        if (current == dropSlotL) return dropSlotR;
        if (current == dropSlotR) return shipSystemSlot6;
        if (current == shipSystemSlot1) return weaponSlot2; //sus
        if (current == shipSystemSlot4) return weaponSlot4; //sus
        if (current == trashSlot) return storageSlot5;
        return current - 1;
    }
    private int MoveRight(int current) {
        if (current == dropSlotL) return weaponSlot3;
        if (current == weaponSlot2) return shipSystemSlot1; //sus
        if (current == weaponSlot4) return shipSystemSlot4; //sus
        if (current == shipSystemSlot3 || current == shipSystemSlot6 || current == trashSlot) return dropSlotR;
        if (current == storageSlot5) return trashSlot;
        if (current == dropSlotR) return dropSlotL;
        return current + 1;
    }
    private int MoveUp(int current) {
        if (current == dropSlotL) return current;
        if( current == dropSlotR) return current;
        if (current == weaponSlot1) return storageSlot1;
        if (current == weaponSlot2) return storageSlot1;
        if (current == weaponSlot3) return weaponSlot1;
        if (current == weaponSlot4) return weaponSlot2;
        if (current == storageSlot1) return weaponSlot4;
        if (current == storageSlot2) return current;
        if (current == storageSlot3) return current;
        if (current == storageSlot4) return shipSystemSlot4;
        if (current == storageSlot5) return shipSystemSlot5;
        if (current == shipSystemSlot1) return storageSlot4;
        if (current == shipSystemSlot2) return storageSlot5;
        if (current == shipSystemSlot3) return trashSlot;
        if (current == shipSystemSlot4) return shipSystemSlot1;
        if (current == shipSystemSlot5) return shipSystemSlot2;
        if (current == shipSystemSlot6) return shipSystemSlot3;
        if (current == trashSlot) return shipSystemSlot6;
        return current;

    }
    private int MoveDown(int current) {
        if (current == dropSlotL) return current;
        if (current == dropSlotR) return current;
        if (current == weaponSlot1) return weaponSlot3;
        if (current == weaponSlot2) return weaponSlot4;
        if (current == weaponSlot3) return storageSlot1;
        if (current == weaponSlot4) return storageSlot1;
        if (current == storageSlot1) return weaponSlot2;
        if (current == storageSlot2) return current;
        if (current == storageSlot3) return current;
        if (current == storageSlot4) return shipSystemSlot1;
        if (current == storageSlot5) return shipSystemSlot2;
        if (current == shipSystemSlot1) return shipSystemSlot4;        
        if (current == shipSystemSlot2) return shipSystemSlot5;
        if (current == shipSystemSlot3) return shipSystemSlot6;
        if (current == shipSystemSlot4) return storageSlot4;
        if (current == shipSystemSlot5) return storageSlot5;
        if (current == shipSystemSlot6) return trashSlot;
        if (current == trashSlot) return shipSystemSlot3;
        return current;
    }


}
