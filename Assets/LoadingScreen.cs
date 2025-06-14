// using System.Collections;
// using UnityEngine;

// public class LoadingScreen : MonoBehaviour
// {
//     [SerializeField] private GameObject loadingPanel;
//     [SerializeField] private float loadingDuration = 2f; // ne kadar gözüksün

//     private void Start()
//     {
//         StartCoroutine(HideLoadingPanelAfterDelay());
//     }

//     private IEnumerator HideLoadingPanelAfterDelay()
//     {
//         if (loadingPanel != null)
//             loadingPanel.SetActive(true);

//         yield return new WaitForSeconds(loadingDuration);

//         if (loadingPanel != null)
//             loadingPanel.SetActive(false);
//     }
// }
