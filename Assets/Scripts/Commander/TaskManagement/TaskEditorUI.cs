using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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

    // ✅ Cambiado: ya no se asigna desde CommanderDroneManager aquí dentro
    private List<DroneData> availableDrones = new List<DroneData>();

    // ✅ Nuevo método para inyectar los drones desde fuera
    public void SetAvailableDrones(List<DroneData> drones)
    {
        availableDrones = drones ?? new List<DroneData>();
    }

    public void Show(Action<TaskData> onConfirm, TaskData taskToEdit = null)
    {
        gameObject.SetActive(true);
        onConfirmCallback = onConfirm;
        editingTask = taskToEdit;

        // ✅ Validamos antes de mostrar
        if (availableDrones == null || availableDrones.Count == 0)
        {
            Debug.LogWarning("⚠️ No hay drones disponibles para mostrar en el dropdown.");
            dropdownDrone.ClearOptions();
            dropdownDrone.AddOptions(new List<string> { "Ninguno" });
            dropdownDrone.interactable = false;
            return;
        }

        dropdownDrone.interactable = true;
        dropdownDrone.ClearOptions();
        List<string> droneNames = availableDrones.ConvertAll(d => d.droneName);
        dropdownDrone.AddOptions(droneNames);

        if (taskToEdit != null)
        {
            inputTitle.text = taskToEdit.title;
            inputDescription.text = taskToEdit.description;
            dropdownStatus.value = dropdownStatus.options.FindIndex(o => o.text == taskToEdit.status);

            int index = availableDrones.FindIndex(d => d == taskToEdit.assignedDrone);
            dropdownDrone.value = index >= 0 ? index : 0;
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
        {
            editingTask = new TaskData();
            editingTask.id = Guid.NewGuid().ToString(); // 🔑 ID único solo si es nueva
        }

        editingTask.title = inputTitle.text;
        editingTask.description = inputDescription.text;
        editingTask.status = dropdownStatus.options[dropdownStatus.value].text;

        if (dropdownDrone.value >= 0 && dropdownDrone.value < availableDrones.Count)
        {
            editingTask.assignedDrone = availableDrones[dropdownDrone.value];
        }
        else
        {
            Debug.LogWarning("⚠️ Índice de dron inválido en el dropdown. Se asigna null.");
            editingTask.assignedDrone = null;
        }

        gameObject.SetActive(false);
        onConfirmCallback?.Invoke(editingTask);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

}
