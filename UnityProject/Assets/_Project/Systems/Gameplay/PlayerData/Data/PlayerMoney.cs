using System;
using UnityEngine;

[Serializable]
public class PlayerMoney {
    [SerializeField] private int credits = 0;
    public int Credits => credits;
    public Action<int> OnCreditsChanged;

    // ------------------ CONSTRUCTORS ------------------
    public PlayerMoney(int startCredits = 0) { SetCredits(startCredits); }

    // ------------------ PUBLIC API ------------------
    public bool CanAfford(int amount) {
        return amount <= 0 || credits >= amount;
    }
    public void AddCredits(int amount) {
        if (amount <= 0) return;
        SetCredits(credits + amount);
    }
    public bool TrySpendCredits(int amount) {
        if (amount <= 0) return false;
        if (credits < amount) return false;
        SetCredits(credits - amount);
        return true;
    }
    public void ResetCredits(int newAmount = 0) {
        SetCredits(newAmount);
    }

    // ------------------ INTERNAL ------------------
    private void SetCredits(int value) {
        credits = Mathf.Max(0, value);
        OnCreditsChanged?.Invoke(credits);
    }

    // ------------------ SAVE / LOAD ------------------
    public PlayerMoneySaveData ToSaveData() {
        return new PlayerMoneySaveData {
            credits = credits
        };
    }
    public void LoadFromSaveData(PlayerMoneySaveData data) {
        if (data == null) {
            SetCredits(0);
            return;
        }
        SetCredits(data.credits);
    }
}
