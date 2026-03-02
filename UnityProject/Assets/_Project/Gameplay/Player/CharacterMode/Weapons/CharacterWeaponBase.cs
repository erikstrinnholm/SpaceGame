using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType {
    Unarmed = 0,
    Rifle = 1,
    Wrench = 2
}

public abstract class CharacterWeaponBase : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType Type => weaponType;

    [Header("Sockets (Pre-aligned in skeleton)")]
    [SerializeField] protected Transform handSocket;
    [SerializeField] protected Transform backSocket;

    protected bool isEquipped;
    public bool IsEquipped => isEquipped;

    // ================= ATTACH LOGIC =================
    public void AttachToHandSocket() {
        if (handSocket == null) return;
        transform.SetParent(handSocket, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        isEquipped = true;
    }
    public void AttachToBackSocket() {
        if (backSocket == null) return;
        transform.SetParent(backSocket, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;        
        isEquipped = false;
    }

    public abstract void OnAttackHeld();
    public abstract void OnAttackReleased();
    public abstract void OnReload();
    public abstract bool IsBusy { get; }
}
