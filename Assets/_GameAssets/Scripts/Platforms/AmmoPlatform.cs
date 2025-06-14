// using UnityEngine;

// public class AmmoPlatform : MonoBehaviour
// {
//     [SerializeField] private float interactDistance = 2f;
//     private bool isReloading = false;

//     private void Update()
//     {
//         GameObject drone = GameObject.FindWithTag("Player");
//         if (drone == null || isReloading) return;

//         float distance = Vector3.Distance(transform.position, drone.transform.position);

//         if (distance <= interactDistance && Input.GetKeyDown(KeyCode.F))
//         {
//             DroneStatus status = drone.GetComponent<DroneStatus>();
//             if (status != null)
//             {
//                 isReloading = true;
//                 drone.GetComponent<MonoBehaviour>().StartCoroutine(
//                     status.StartReloadingOverTime(() => isReloading = false)
//                 );
//             }
//         }
//     }
// }
