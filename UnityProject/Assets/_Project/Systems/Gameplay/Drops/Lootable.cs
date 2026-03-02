using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Lootable : MonoBehaviour, ILootSource {
    [SerializeField] DropTable dropTable;
    [SerializeField] Transform dropOrigin;

    public DropTable DropTable => dropTable;
    public Transform DropOrigin => dropOrigin != null ? dropOrigin : transform;

    protected virtual void DropLoot() {
        GameRoot.Instance.Drop.RollAndSpawn(this);
    }
}

