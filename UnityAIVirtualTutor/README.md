# Virtual 3D AI Tutor — Unity Interaction Control Logic

This repository contains Unity-ready C# scripts for a Virtual 3D AI Tutor capable of turning, gesturing, writing on a chalkboard, speaking with lip-sync, and responding to student input.

## Features
- Event-driven control API: `OnFaceStudent`, `OnTurnToBoard`, `OnWriteBoard`, `OnExplainFacingStudent`, `OnStudentResponse`, `OnAssessProgress`, `OnLessonComplete`
- Animator hooks for states: `FaceStudentIdle`, `TurnToBoard`, `ChalkWrite`, `ExplainGesture`, `GoodbyeWave`
- Chalk writing API via `ChalkBoard.Write(content, style, color)`
- Lip-sync controller stub to integrate with your facial rig
- TTS/STT/GPT service stubs with async APIs
- Demo bootstrap that runs a short Python "Hello, World!" flow

## Folder Structure
```
UnityAIVirtualTutor/
  Assets/
    Scripts/
      Tutor/
        TutorController.cs
      World/
        ChalkBoard.cs
      Avatar/
        LipSyncController.cs
      Services/
        TTSService.cs
        STTService.cs
        GPTService.cs
      Demo/
        DemoBootstrap.cs
```

## Getting Started
1. Open the `UnityAIVirtualTutor` folder in Unity (2021.3+ recommended).
2. In Player Settings > Other Settings, set Api Compatibility Level to ".NET 4.x".
3. Create an empty GameObject `Tutor` and add:
   - `TutorController`
   - `AudioSource`
   - `LipSyncController`
   - Assign `ChalkBoard` (add to a board object in your scene)
   - Optionally add `TTSService`, `STTService`, `GPTService` to the scene and assign to the controller
4. Provide references on `TutorController`:
   - `Student Camera`: Main Camera (student POV)
   - `Board Transform`: your blackboard transform
5. Set up Animator Controller for your avatar with states named:
   - `FaceStudentIdle`, `TurnToBoard`, `ChalkWrite`, `ExplainGesture`, `GoodbyeWave`
6. Add `DemoBootstrap` to any object, assign `TutorController`, enable `Run Demo`.
7. Play the scene.

## Integrations
- TTS: Replace `TTSService.GenerateAsync` with Azure/ElevenLabs/Polly output -> `AudioClip`
- STT: Implement `STTService.RecognizeAsync` using Whisper/Azure Speech
- GPT: Implement `GPTService.GenerateResponseAsync` to call your LLM provider

## Notes
- The TTS stub synthesizes a quiet sine wave for timing. Replace in production.
- The chalkboard currently logs write requests; plug into a line renderer or a GPU-drawing shader for chalk effects.
- All async calls are Unity-friendly (no blocking). Speech duration is awaited to prevent overlap.

## License
MIT