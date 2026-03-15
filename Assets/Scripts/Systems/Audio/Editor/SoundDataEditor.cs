using System;
using System.Linq;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Editor
{
    [CustomEditor(typeof(SoundData))]
    public class SoundDataEditor : UnityEditor.Editor
    {
        SerializedProperty clipsProperty;
        SerializedProperty settingsProperty;
        SerializedProperty frequentSoundProperty;
        SerializedProperty volumeProperty;
        SerializedProperty pitchProperty;
        AudioSource previewSource;
        UnityEditor.Editor settingsEditor;
        bool unlockSettings;
        float previewDistance = 10f;

        void OnEnable()
        {
            clipsProperty = serializedObject.FindProperty("clips");
            clipsProperty.isExpanded = true;
            settingsProperty = serializedObject.FindProperty("settings");
            frequentSoundProperty = serializedObject.FindProperty("frequentSound");
            volumeProperty = serializedObject.FindProperty("volume");
            pitchProperty = serializedObject.FindProperty("pitch");

            var previewObject = EditorUtility.CreateGameObjectWithHideFlags(
                "AudioPreview", HideFlags.HideAndDontSave, typeof(AudioSource));
            previewSource = previewObject.GetComponent<AudioSource>();
        }

        void OnDisable()
        {
            if (previewSource != null) DestroyImmediate(previewSource.gameObject);
            if (settingsEditor == null) return;
            DestroyImmediate(settingsEditor);
            settingsEditor = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawSoundDataProperties();
            EditorGUILayout.Space();

            EditorGUILayout.Space();
            DrawPreviewButtons();
            EditorGUILayout.Space();

            DrawValidation();
            EditorGUILayout.Space();

            DrawSettingsPreview();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawSoundDataProperties()
        {
            EditorGUILayout.LabelField("Sound Data", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(clipsProperty, true);
            EditorGUILayout.PropertyField(settingsProperty);
            EditorGUILayout.PropertyField(frequentSoundProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Playback Settings", EditorStyles.boldLabel);

            EditorGUILayout.Slider(volumeProperty, 0f, 1f, new GUIContent("Volume"));
            EditorGUILayout.Slider(pitchProperty, -3f, 3f, new GUIContent("Pitch"));
        }

        void DrawPreviewButtons()
        {
            var soundData = (SoundData)target;
            var clip = soundData.GetClip();
            if (clip == null || previewSource == null) return;

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Basic 2D")) PlayPreview(false, false);
            if (GUILayout.Button("With Settings")) PlayPreview(true, false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            previewDistance = EditorGUILayout.FloatField(new GUIContent(
                "Distance",
                "Distance to simulate."),
                previewDistance
            );
            if (GUILayout.Button("At Distance", GUILayout.Width(100f))) PlayPreview(true, true);
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(previewSource == null || !previewSource.isPlaying))
                if (GUILayout.Button("Stop Preview")) previewSource.Stop();
        }

        void PlayPreview(bool applySettings, bool applyDistance)
        {
            var soundData = (SoundData)target;
            var clip = soundData.GetClip();
            if (clip == null || previewSource == null) return;

            previewSource.Stop();
            previewSource.clip = clip;
            previewSource.pitch = soundData.pitch;

            var simulatedDistanceVolume = 1f;

            if (applySettings && soundData.settings != null)
            {
                var settings = soundData.settings;
                previewSource.outputAudioMixerGroup = settings.mixerGroup;
                previewSource.loop = settings.loop;
                previewSource.mute = settings.mute;
                previewSource.bypassEffects = settings.bypassEffects;
                previewSource.bypassListenerEffects = settings.bypassListenerEffects;
                previewSource.bypassReverbZones = settings.bypassReverbZones;
                previewSource.priority = settings.priority;
                previewSource.panStereo = settings.panStereo;
                previewSource.reverbZoneMix = settings.reverbZoneMix;
                previewSource.dopplerLevel = settings.dopplerLevel;
                previewSource.spread = settings.spread;

                if (applyDistance)
                {
                    previewSource.spatialBlend = 0f;
                    var min = settings.minDistance;
                    var max = settings.maxDistance;
                    var dist = Mathf.Clamp(previewDistance, 0f, max);
                    simulatedDistanceVolume = settings.rolloffMode switch
                    {
                        AudioRolloffMode.Custom => settings.customRolloffCurve.Evaluate(dist / max),
                        AudioRolloffMode.Linear => 1f - Mathf.Clamp01((dist - min) / (max - min)),
                        AudioRolloffMode.Logarithmic => min / Mathf.Max(dist, min),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
                else
                    previewSource.spatialBlend = settings.spatialBlend;
            }
            else
            {
                // Reset to basic 2D defaults
                previewSource.spatialBlend = 0f;
                previewSource.bypassEffects = true;
                previewSource.bypassListenerEffects = true;
                previewSource.bypassReverbZones = true;
            }

            previewSource.volume = soundData.volume * simulatedDistanceVolume;
            previewSource.transform.position = Vector3.zero;
            previewSource.Play();
        }

        void DrawValidation()
        {
            var soundData = (SoundData)target;

            if (soundData.settings == null)
                EditorGUILayout.HelpBox("A SoundDataSettings asset is required.", MessageType.Warning);

            if (soundData.clips == null || soundData.clips.Length == 0)
            {
                EditorGUILayout.HelpBox("At least one AudioClip should be assigned.", MessageType.Warning);
                return;
            }

            if (soundData.clips.AsValueEnumerable().All(t => t != null)) return;
            EditorGUILayout.HelpBox("The clips array contains one or more null entries.", MessageType.Warning);
        }

        void DrawSettingsPreview()
        {
            var soundData = (SoundData)target;
            if (soundData.settings == null) return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Settings Preview", EditorStyles.boldLabel);

            // Toggle button to unlock editing
            unlockSettings = GUILayout.Toggle(
                unlockSettings, unlockSettings ? "Lock Settings" : "Edit Settings", "Button", GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();

            if (unlockSettings)
                EditorGUILayout.HelpBox(
                    "Warning: Modifying SoundDataSettings here will affect ALL SoundData " +
                    "assets that share this profile. Ensure this is your intended action.",
                    MessageType.Warning
                );

            using (new EditorGUI.DisabledScope(!unlockSettings))
            {
                CreateCachedEditor(soundData.settings, typeof(SoundDataSettingsEditor), ref settingsEditor);

                if (settingsEditor != null) settingsEditor.OnInspectorGUI();
            }
        }
    }
}