using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiSelectDropdown : TMP_Dropdown
{
    public List<int> SelectedIndices { get; } = new();

    protected override DropdownItem CreateItem(DropdownItem template)
    {
        DropdownItem item = base.CreateItem(template);

        // Evitar que se cierre al pulsar
        item.toggle.onValueChanged.RemoveAllListeners();
        item.toggle.onValueChanged.AddListener(isOn =>
        {
            int idx = item.toggle.transform.GetSiblingIndex();
            if (isOn)
            {
                if (!SelectedIndices.Contains(idx)) SelectedIndices.Add(idx);
            }
            else
            {
                SelectedIndices.Remove(idx);
            }
            UpdateCaption();
        });

        return item;
    }

    private void UpdateCaption()
    {
        if (captionText == null) return;
        captionText.text = SelectedIndices.Count switch
        {
            0 => "(Ninguno)",
            1 => options[SelectedIndices[0]].text,
            _ => $"({SelectedIndices.Count} seleccionados)"
        };
    }

    public void RefreshChecks()   // Úsalo cuando cambies SelectedIndices por código
    {
        foreach (Toggle t in GetComponentsInChildren<Toggle>(true))
            t.isOn = SelectedIndices.Contains(t.transform.GetSiblingIndex());
        UpdateCaption();
    }
}
