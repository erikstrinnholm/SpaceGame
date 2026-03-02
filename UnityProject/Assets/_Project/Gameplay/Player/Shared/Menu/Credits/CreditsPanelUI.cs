using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsPanelUI : MonoBehaviour {
    [Header("Data")]
    [SerializeField] private CreditsDatabase creditsDatabase;

    [Header("UI")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject sectionHeaderPrefab;
    [SerializeField] private GameObject creditEntryPrefab;

    private void OnEnable() {
        ClearContent();
        BuildCredits();
    }

    private void BuildCredits() {
        ClearContent();

        var grouped = new Dictionary<string, List<CreditEntry>>();

        foreach (var entry in creditsDatabase.entries) {
            if (!grouped.TryGetValue(entry.section, out var list)) {
                list = new List<CreditEntry>();
                grouped.Add(entry.section, list);
            }
            list.Add(entry);
        }

        foreach (var group in grouped) {
            // --- Section header ---
            CreateTextElement(sectionHeaderPrefab, group.Key);

            // --- Entries ---
            foreach (var entry in group.Value) {
                CreateTextElement(creditEntryPrefab, entry.text);
            }
        }
    }

    private void CreateTextElement(GameObject prefab, string text) {
        GameObject go = Instantiate(prefab, contentRoot);

        TMP_Text tmp = go.GetComponentInChildren<TMP_Text>();
        if (tmp == null) {
            Debug.LogError($"Prefab '{prefab.name}' is missing a TMP_Text component.");
            return;
        }

        tmp.text = text;
    }

    private void ClearContent() {
        for (int i = contentRoot.childCount - 1; i >= 0; i--) {
            Destroy(contentRoot.GetChild(i).gameObject);
        }
    }
}
