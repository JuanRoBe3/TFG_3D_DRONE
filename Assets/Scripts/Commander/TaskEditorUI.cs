using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TaskEditorUI : MonoBehaviour
{
    public TMP_InputField inputTitle;
    public TMP_InputField inputDescription;
    public TMP_Dropdown dropdownStatus;
    public TMP_Dropdown dropdownDrone;

    public Button buttonConfirm;
    public Button buttonCancel;

    private Action<TaskData> onConfirmCallback;
    private TaskData editingTask;

    public void Show(Action<TaskData> onConfirm, TaskData taskToEdit = null)
    {
        gameObject.SetActive(true);
        onConfirmCallback = onConfirm;

        editingTask = taskToEdit;

        if (taskToEdit != null)
        {
            inputTitle.text = taskToEdit.title;
            inputDescription.text = taskToEdit.description;
            dropdownStatus.value = dropdownStatus.options.FindIndex(o => o.text == taskToEdit.status);
            dropdownDrone.value = dropdownDrone.options.FindIndex(o => o.text == taskToEdit.assignedDrone);
        }
        else
        {
            inputTitle.text = "";
            inputDescription.text = "";
            dropdownStatus.value = 0;
            dropdownDrone.value = 0;
        }
    }

    public void Confirm()
    {
        if (editingTask == null)
            editingTask = new TaskData();

        editingTask.title = inputTitle.text;
        editingTask.description = inputDescription.text;
        editingTask.status = dropdownStatus.options[dropdownStatus.value].text;
        editingTask.assignedDrone = dropdownDrone.options[dropdownDrone.value].text;

        gameObject.SetActive(false);
        onConfirmCallback?.Invoke(editingTask);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}
