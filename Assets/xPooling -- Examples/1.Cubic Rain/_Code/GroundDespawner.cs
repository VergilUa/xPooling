using UnityEngine;

/// <summary>
/// Used to catch callbacks for the RainElement instances and decommissioning them back to the pool
/// </summary>
public class GroundDespawner : MonoBehaviour {
   private void OnTriggerEnter(Collider other) {
      if (other.TryGetComponent(out RainElement rainElement)) {
         rainElement.Decommission();
      }
   }
}
