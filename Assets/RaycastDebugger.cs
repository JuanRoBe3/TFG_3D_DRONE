using UnityEngine;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))          // clic izquierdo
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 10000f))
            {
                Debug.Log($"🎯 Raycast hit: {hit.collider.name}  (layer {hit.collider.gameObject.layer})");
            }
            else
            {
                Debug.Log("❌ Raycast no tocó nada; quizá un Canvas bloquea, o la cámara no ve el objeto.");
            }
        }
    }
}
