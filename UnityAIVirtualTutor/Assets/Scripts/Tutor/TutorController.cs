using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace VirtualTutor
{
    public enum TutorState
    {
        IdleFacingStudent,
        TurnToBoard,
        WriteBoard,
        Explain,
        Listening,
        Goodbye
    }

    public class TutorController : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Transform studentCamera;
        [SerializeField] private Transform boardTransform;
        [SerializeField] private ChalkBoard chalkBoard;
        [SerializeField] private LipSyncController lipSyncController;
        [SerializeField] private AudioSource audioSource;

        [Header("Services")]
        [SerializeField] private TTSService ttsService;
        [SerializeField] private STTService sttService;
        [SerializeField] private GPTService gptService;

        [Header("Tuning")]
        [SerializeField] private float rotateTowardsStudentSpeed = 2.5f;
        [SerializeField] private float rotateTowardsBoardSpeed = 3.0f;

        public TutorState CurrentState { get; private set; } = TutorState.IdleFacingStudent;
        public string CurrentLessonContext { get; private set; } = string.Empty;

        void Reset()
        {
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponent<AudioSource>();
            lipSyncController = GetComponentInChildren<LipSyncController>();
            chalkBoard = FindObjectOfType<ChalkBoard>();
        }

        public void OnFaceStudent()
        {
            StopAllCoroutines();
            if (studentCamera != null)
            {
                StartCoroutine(RotateTowards(studentCamera.position, rotateTowardsStudentSpeed));
            }
            PlayAnimationSafe("FaceStudentIdle");
            CurrentState = TutorState.IdleFacingStudent;
            if (lipSyncController != null) lipSyncController.SetActive(true);
        }

        public void OnTurnToBoard()
        {
            StopAllCoroutines();
            if (boardTransform != null)
            {
                StartCoroutine(RotateTowards(boardTransform.position, rotateTowardsBoardSpeed));
            }
            PlayAnimationSafe("TurnToBoard");
            CurrentState = TutorState.TurnToBoard;
            if (lipSyncController != null) lipSyncController.SetActive(false);
        }

        public async void OnWriteBoard(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;
            PlayAnimationSafe("ChalkWrite");
            CurrentState = TutorState.WriteBoard;
            if (chalkBoard != null)
            {
                chalkBoard.Write(content, ChalkStyle.Chalk, Color.white);
            }
            await Say("Let's go through this step by step...");
            if (studentCamera != null && boardTransform != null)
            {
                Vector3 mid = Vector3.Lerp(boardTransform.position, studentCamera.position, 0.25f);
                StartCoroutine(RotateTowards(mid, rotateTowardsBoardSpeed));
            }
        }

        public async void OnExplainFacingStudent(string text = "Now that we’ve written this, let’s understand what it means...")
        {
            OnFaceStudent();
            PlayAnimationSafe("ExplainGesture");
            CurrentState = TutorState.Explain;
            await Say(text);
        }

        public void OnLipSync(string text)
        {
            if (lipSyncController != null)
            {
                lipSyncController.Play(text, voicePreset: lipSyncController.DefaultVoicePreset, expression: "teaching");
            }
        }

        public async void OnStudentResponse(string input)
        {
            CurrentState = TutorState.Listening;
            string response = gptService != null
                ? await gptService.GenerateResponseAsync(input, CurrentLessonContext)
                : $"You asked: {input}";
            await Say(response);
        }

        public async void OnAssessProgress()
        {
            string strengths = "syntax";
            string weaknesses = "loops";
            await Say($"You're improving in {strengths}, but let's revisit {weaknesses} next time.");
        }

        public async void OnLessonComplete()
        {
            PlayAnimationSafe("GoodbyeWave");
            CurrentState = TutorState.Goodbye;
            await Say("Great job today! Let's continue next time!");
        }

        public async void Greet()
        {
            OnFaceStudent();
            await Say("Hello! I’m your AI Tutor. Which subject would you like to learn today?");
        }

        public async Task Say(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            if (lipSyncController != null)
            {
                lipSyncController.Play(text, voicePreset: lipSyncController.DefaultVoicePreset, expression: "teaching");
            }
            AudioClip clip = null;
            if (ttsService != null)
            {
                try { clip = await ttsService.GenerateAsync(text); }
                catch (Exception ex) { Debug.LogWarning($"TTS generation failed: {ex.Message}"); }
            }
            if (clip != null && audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
                await WaitForSecondsAsync(clip.length);
            }
            else
            {
                float approx = Mathf.Clamp(text.Length / 15.0f, 1.0f, 6.0f);
                await WaitForSecondsAsync(approx);
            }
        }

        private IEnumerator RotateTowards(Vector3 worldTargetPosition, float speed)
        {
            Vector3 targetFlat = new Vector3(worldTargetPosition.x, transform.position.y, worldTargetPosition.z);
            Vector3 direction = (targetFlat - transform.position).normalized;
            if (direction.sqrMagnitude < 0.0001f) yield break;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
                yield return null;
            }
            transform.rotation = targetRotation;
        }

        private void PlayAnimationSafe(string stateName)
        {
            if (animator == null || string.IsNullOrEmpty(stateName)) return;
            animator.CrossFadeInFixedTime(stateName, 0.15f);
        }

        private static async Task WaitForSecondsAsync(float seconds)
        {
            float start = Time.time;
            while (Time.time - start < seconds)
            {
                await Task.Yield();
            }
        }
    }
}