using UnityEngine;

public class SelectableTaskItem : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject selectedImage;

    public void Select()
    {
        if (selectedImage != null)
            selectedImage.SetActive(true);
    }

    public void Deselect()
    {
        if (selectedImage != null)
            selectedImage.SetActive(false);
    }
}
