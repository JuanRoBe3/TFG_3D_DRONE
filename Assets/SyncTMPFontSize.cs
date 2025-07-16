using UnityEngine;
using TMPro;

public class SyncTMPFontSize : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] syncedTexts;

    private bool hasSynced = false;

    private void LateUpdate()
    {
        if (hasSynced || syncedTexts == null || syncedTexts.Length == 0) return;

        float minSize = float.MaxValue;

        foreach (var tmp in syncedTexts)
        {
            if (tmp == null || !tmp.enableAutoSizing) continue;
            tmp.ForceMeshUpdate(); // asegura que el auto size se haya aplicado
            minSize = Mathf.Min(minSize, tmp.fontSize);
        }

        foreach (var tmp in syncedTexts)
        {
            if (tmp == null) continue;
            tmp.enableAutoSizing = false;
            tmp.fontSize = minSize;
        }

        hasSynced = true; // Solo una vez
    }
}
