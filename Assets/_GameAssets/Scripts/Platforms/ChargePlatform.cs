// using UnityEngine;

// public class ChargePlatform : MonoBehaviour
// {
//     [SerializeField] private float interactDistance = 2f;
//     private bool isCharging = false;

//     private void Update()
//     {
//         GameObject drone = GameObject.FindWithTag("Player");
//         if (drone == null || isCharging) return;

//         float distance = Vector3.Distance(transform.position, drone.transform.position);

//         if (distance <= interactDistance && Input.GetKeyDown(KeyCode.F)) // F tuşu
//         {
//             DroneStatus status = drone.GetComponent<DroneStatus>();
//             if (status != null)
//             {
//                 isCharging = true;
//                 StartCoroutine(status.StartChargingOverTime(() => isCharging = false));
//             }
//         }
//     }
// }
