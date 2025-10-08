using System.Collections;
using UnityEngine;

namespace VirtualTutor
{
    public class DemoBootstrap : MonoBehaviour
    {
        [SerializeField] private TutorController tutor;
        [SerializeField] private bool runDemo = true;

        private void Awake()
        {
            if (tutor == null) tutor = FindObjectOfType<TutorController>();
        }

        private void Start()
        {
            if (runDemo && tutor != null)
            {
                StartCoroutine(DemoSequence());
            }
        }

        private IEnumerator DemoSequence()
        {
            tutor.Greet();
            yield return new WaitForSeconds(3f);
            tutor.OnTurnToBoard();
            yield return new WaitForSeconds(1f);
            tutor.OnWriteBoard("print('Hello, World!')");
            yield return new WaitForSeconds(3f);
            tutor.OnExplainFacingStudent();
            yield return new WaitForSeconds(3f);
            tutor.OnLessonComplete();
        }
    }
}