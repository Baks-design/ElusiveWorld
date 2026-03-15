using System.Reflection;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data;
using UnityEditor;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Editor
{
    [CustomEditor(typeof(SoundDataSettings))]
    public class SoundDataSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty mixerGroupProperty;
        SerializedProperty loopProperty;
        SerializedProperty muteProperty;
        SerializedProperty bypassEffectsProperty;
        SerializedProperty bypassListenerEffectsProperty;
        SerializedProperty bypassReverbZonesProperty;
        SerializedProperty priorityProperty;
        SerializedProperty panStereoProperty;
        SerializedProperty spatialBlendProperty;
        SerializedProperty reverbZoneMixProperty;
        SerializedProperty dopplerLevelProperty;
        SerializedProperty spreadProperty;
        SerializedProperty minDistanceProperty;
        SerializedProperty maxDistanceProperty;
        SerializedProperty ignoreListenerVolumeProperty;
        SerializedProperty ignoreListenerPauseProperty;
        SerializedProperty rolloffModeProperty;
        SerializedProperty customRolloffCurveProperty;
        SerializedProperty spatialBlendCurveProperty;
        SerializedProperty spreadCurveProperty;
        SerializedProperty reverbZoneMixCurveProperty;
        GameObject dummyAudioObject;
        AudioSource dummyAudioSource;
        UnityEditor.Editor audioSourceEditor;
        MethodInfo audio3DGuiMethod;
        bool expanded3D = true;

        void OnEnable()
        {
            if (target == null) return;

            mixerGroupProperty = serializedObject.FindProperty("mixerGroup");
            loopProperty = serializedObject.FindProperty("loop");

            muteProperty = serializedObject.FindProperty("mute");
            bypassEffectsProperty = serializedObject.FindProperty("bypassEffects");
            bypassListenerEffectsProperty = serializedObject.FindProperty("bypassListenerEffects");
            bypassReverbZonesProperty = serializedObject.FindProperty("bypassReverbZones");

            priorityProperty = serializedObject.FindProperty("priority");
            panStereoProperty = serializedObject.FindProperty("panStereo");
            spatialBlendProperty = serializedObject.FindProperty("spatialBlend");
            reverbZoneMixProperty = serializedObject.FindProperty("reverbZoneMix");
            dopplerLevelProperty = serializedObject.FindProperty("dopplerLevel");
            spreadProperty = serializedObject.FindProperty("spread");

            minDistanceProperty = serializedObject.FindProperty("minDistance");
            maxDistanceProperty = serializedObject.FindProperty("maxDistance");

            ignoreListenerVolumeProperty = serializedObject.FindProperty("ignoreListenerVolume");
            ignoreListenerPauseProperty = serializedObject.FindProperty("ignoreListenerPause");

            rolloffModeProperty = serializedObject.FindProperty("rolloffMode");

            customRolloffCurveProperty = serializedObject.FindProperty("customRolloffCurve");
            spatialBlendCurveProperty = serializedObject.FindProperty("spatialBlendCurve");
            spreadCurveProperty = serializedObject.FindProperty("spreadCurve");
            reverbZoneMixCurveProperty = serializedObject.FindProperty("reverbZoneMixCurve");

            dummyAudioObject = new GameObject("DummyAudioSource_Preview")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            dummyAudioSource = dummyAudioObject.AddComponent<AudioSource>();

            audioSourceEditor = CreateEditor(dummyAudioSource);
            audio3DGuiMethod = audioSourceEditor.GetType().GetMethod(
                "Audio3DGUI",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
        }

        void OnDisable()
        {
            if (audioSourceEditor != null) DestroyImmediate(audioSourceEditor);
            if (dummyAudioObject != null) DestroyImmediate(dummyAudioObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(mixerGroupProperty);
            EditorGUILayout.PropertyField(loopProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Source Flags", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(muteProperty);
            EditorGUILayout.PropertyField(bypassEffectsProperty);
            EditorGUILayout.PropertyField(bypassListenerEffectsProperty);
            EditorGUILayout.PropertyField(bypassReverbZonesProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Playback", EditorStyles.boldLabel);

            DrawIntSliderWithLabels(priorityProperty, 0, 256, "Priority", "High", "Low");
            DrawSliderWithLabels(panStereoProperty, -1f, 1f, "Stereo Pan", "Left", "Right");
            DrawSliderWithLabels(spatialBlendProperty, 0f, 1f, "Spatial Blend", "2D", "3D");

            EditorGUILayout.Slider(reverbZoneMixProperty, 0f, 1.1f, new GUIContent("Reverb Zone Mix"));

            EditorGUILayout.Space();

            if (spatialBlendProperty.floatValue > 0f)
            {
                expanded3D = EditorGUILayout.Foldout(expanded3D, "3D Sound Settings", true);

                if (expanded3D)
                {
                    EditorGUI.indentLevel++;
                    DrawAudioSource3DSettings();
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Listener", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(ignoreListenerVolumeProperty);
            EditorGUILayout.PropertyField(ignoreListenerPauseProperty);

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Reset to Defaults"))
                ResetToDefaults();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawAudioSource3DSettings()
        {
            if (audio3DGuiMethod == null || dummyAudioSource == null) return;

            // Sync SoundDataSettings to Dummy AudioSource
            dummyAudioSource.dopplerLevel = dopplerLevelProperty.floatValue;
            dummyAudioSource.spread = spreadProperty.floatValue;
            dummyAudioSource.minDistance = minDistanceProperty.floatValue;
            dummyAudioSource.maxDistance = maxDistanceProperty.floatValue;
            dummyAudioSource.rolloffMode = (AudioRolloffMode)rolloffModeProperty.enumValueIndex;
            dummyAudioSource.spatialBlend = spatialBlendProperty.floatValue;
            dummyAudioSource.reverbZoneMix = reverbZoneMixProperty.floatValue;

            if (customRolloffCurveProperty.animationCurveValue != null)
                dummyAudioSource.SetCustomCurve(
                    AudioSourceCurveType.CustomRolloff, customRolloffCurveProperty.animationCurveValue);
            if (spatialBlendCurveProperty.animationCurveValue != null)
                dummyAudioSource.SetCustomCurve(
                    AudioSourceCurveType.SpatialBlend, spatialBlendCurveProperty.animationCurveValue);
            if (spreadCurveProperty.animationCurveValue != null)
                dummyAudioSource.SetCustomCurve(
                    AudioSourceCurveType.Spread, spreadCurveProperty.animationCurveValue);
            if (reverbZoneMixCurveProperty.animationCurveValue != null)
                dummyAudioSource.SetCustomCurve(
                    AudioSourceCurveType.ReverbZoneMix, reverbZoneMixCurveProperty.animationCurveValue);

            // Invoke native Audio3DGUI
            audioSourceEditor.serializedObject.Update();
            audio3DGuiMethod.Invoke(audioSourceEditor, null);
            audioSourceEditor.serializedObject.ApplyModifiedProperties();

            // Sync Dummy AudioSource back to SoundDataSettings
            dopplerLevelProperty.floatValue = dummyAudioSource.dopplerLevel;
            spreadProperty.floatValue = dummyAudioSource.spread;
            minDistanceProperty.floatValue = dummyAudioSource.minDistance;
            maxDistanceProperty.floatValue = dummyAudioSource.maxDistance;
            rolloffModeProperty.enumValueIndex = (int)dummyAudioSource.rolloffMode;

            customRolloffCurveProperty.animationCurveValue =
                dummyAudioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            spatialBlendCurveProperty.animationCurveValue =
                dummyAudioSource.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
            spreadCurveProperty.animationCurveValue =
                dummyAudioSource.GetCustomCurve(AudioSourceCurveType.Spread);
            reverbZoneMixCurveProperty.animationCurveValue =
                dummyAudioSource.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix);
        }

        void DrawSliderWithLabels(SerializedProperty property, float min, float max,
           string title, string leftLabel, string rightLabel)
        {
            var rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 14f);

            var sliderRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.Slider(sliderRect, property, min, max, new GUIContent(title));

            DrawSliderLabels(rect, leftLabel, rightLabel);
        }

        void DrawIntSliderWithLabels(SerializedProperty property, int min, int max,
           string title, string leftLabel, string rightLabel)
        {
            var rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 14f);
            var sliderRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.IntSlider(sliderRect, property, min, max, new GUIContent(title));

            DrawSliderLabels(rect, leftLabel, rightLabel);
        }

        void DrawSliderLabels(Rect totalRect, string leftLabel, string rightLabel)
        {
            var labelsRect = new Rect(
                totalRect.x + EditorGUIUtility.labelWidth,
                totalRect.y + EditorGUIUtility.singleLineHeight,
                totalRect.width - EditorGUIUtility.labelWidth - 55f,
                14f
            );

            GUIStyle leftStyle = new(EditorStyles.miniLabel) { alignment = TextAnchor.UpperLeft };
            GUIStyle rightStyle = new(EditorStyles.miniLabel) { alignment = TextAnchor.UpperRight };

            GUI.Label(labelsRect, leftLabel, leftStyle);
            GUI.Label(labelsRect, rightLabel, rightStyle);
        }

        void ResetToDefaults()
        {
            var tempObj = new GameObject("TempAudioSource")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            var tempSource = tempObj.AddComponent<AudioSource>();

            mixerGroupProperty.objectReferenceValue = tempSource.outputAudioMixerGroup;
            loopProperty.boolValue = tempSource.loop;

            muteProperty.boolValue = tempSource.mute;
            bypassEffectsProperty.boolValue = tempSource.bypassEffects;
            bypassListenerEffectsProperty.boolValue = tempSource.bypassListenerEffects;
            bypassReverbZonesProperty.boolValue = tempSource.bypassReverbZones;

            priorityProperty.intValue = tempSource.priority;
            panStereoProperty.floatValue = tempSource.panStereo;
            spatialBlendProperty.floatValue = tempSource.spatialBlend;
            reverbZoneMixProperty.floatValue = tempSource.reverbZoneMix;
            dopplerLevelProperty.floatValue = tempSource.dopplerLevel;
            spreadProperty.floatValue = tempSource.spread;

            minDistanceProperty.floatValue = tempSource.minDistance;
            maxDistanceProperty.floatValue = tempSource.maxDistance;

            ignoreListenerVolumeProperty.boolValue = tempSource.ignoreListenerVolume;
            ignoreListenerPauseProperty.boolValue = tempSource.ignoreListenerPause;

            rolloffModeProperty.enumValueIndex = (int)tempSource.rolloffMode;

            // Reset custom curves
            customRolloffCurveProperty.animationCurveValue =
                tempSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            spatialBlendCurveProperty.animationCurveValue =
                tempSource.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
            spreadCurveProperty.animationCurveValue =
                tempSource.GetCustomCurve(AudioSourceCurveType.Spread);
            reverbZoneMixCurveProperty.animationCurveValue =
                tempSource.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix);

            if (dummyAudioSource != null)
            {
                dummyAudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff,
                    tempSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
                dummyAudioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend,
                    tempSource.GetCustomCurve(AudioSourceCurveType.SpatialBlend));
                dummyAudioSource.SetCustomCurve(AudioSourceCurveType.Spread,
                    tempSource.GetCustomCurve(AudioSourceCurveType.Spread));
                dummyAudioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix,
                    tempSource.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix));
            }

            DestroyImmediate(tempObj);
        }
    }
}