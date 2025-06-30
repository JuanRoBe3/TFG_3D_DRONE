# TFG_3D_DRONE

Unity project simulating a 3D drone control system with a role-based interface: **Commander** and **Pilot**. Real-time communication between clients is handled via MQTT.

## 🚀 Main Features

- Drone model selection using AssetBundles *(pending due to partial knowledge)*
- Commander interface: live drone coordinates + droneView, task assignment, search zones selection
- Pilot interface: joystick and VR controls (Meta XR SDK), drone view streaming
- Scene synchronization using MQTT (camera views, task status, zones, etc.)

## 🧱 Technologies Used

- Unity 6000.0.28f1
- C# (Unity scripting)
- MQTT (via [MQTTnet](https://github.com/dotnet/MQTTnet))
- Meta XR SDK + OpenXR
- AssetBundles *(pending due to partial knowledge)*
- VR integration for Quest/PC

## 🔧 How to Run

1. Clone this repository
2. Open the project with Unity Hub (version **6000.0.28f1** or newer)
3. Unity will automatically resolve dependencies from `manifest.json`
4. Open the scene `RoleSelectionScene` to choose between Pilot and Commander roles
5. For Pilot, make sure a VR headset is connected (e.g. Meta Quest)

## 🎮 Controls Overview

| Action             | Input                                 |
|--------------------|----------------------------------------|
| Move               | W / A / S / D                         |
| Rotate (Yaw)       | Q / E                                 |
| Ascend / Descend   | Joystick buttons 4 / 5 (Logitech X56) |
| View (Commander)   | Top-down map                          |
| View (Pilot)       | First-person VR                       |

## 📁 Folder Structure

Assets/               → Project scripts, prefabs, UI, etc.
Packages/             → Unity package dependencies
ProjectSettings/      → Unity project settings
Builds/               → Output builds (ignored by Git)

## ✅ TO DO

- [ ] Use AssetBundles

## 👤 Author

Juanito

---
