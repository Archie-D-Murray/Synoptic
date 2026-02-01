using System;
using System.IO;

using UnityEngine;
using UnityEngine.InputSystem;

using Utilities;

[DefaultExecutionOrder(-99)]
public class Settings : Singleton<Settings> {

    public const string MasterMixerVolume = "MasterVolume";
    public const string BGMMixerVolume = "BGMVolume";
    public const string SFXMixerVolume = "SFXVolume";

    public SettingsData Defaults = new SettingsData();

    [Header("Settings")]
    public float VoiceVolume = 2;
    public float SFXVolume = 0.5f;
    public float MouseSensitivity = 0.5f;
    public bool HorizontalHeadShake;
    public bool VerticalHeadBob;
    public bool InvertYAxis;
    public bool VoiceChatEnabled = true;

    [Header("Key Bindings")]
    public string Bindings;

    [Header("Data Utilities")]
    private static string settingsFilePath = Path.Combine(Application.dataPath, "userSettings.json");

    protected override void Awake() {
        base.Awake();
        LoadSettings();
    }

    public void SaveSettings() {
        if (!File.Exists(settingsFilePath)) {
            File.Create(settingsFilePath);
        }

        var data = new SettingsData {
            VoiceVolume = VoiceVolume,
            SFXVolume = SFXVolume,
            MouseSensitivity = MouseSensitivity,
            HorizontalHeadShake = HorizontalHeadShake,
            VerticalHeadBob = VerticalHeadBob,
            InvertYAxis = InvertYAxis,
            VoiceChatEnabled = VoiceChatEnabled,
            Bindings = Bindings
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(settingsFilePath, json);
    }

    public void ResetToDefault() {
        VoiceVolume = Defaults.VoiceVolume;
        SFXVolume = Defaults.SFXVolume;
        MouseSensitivity = Defaults.MouseSensitivity;
        HorizontalHeadShake = Defaults.HorizontalHeadShake;
        VerticalHeadBob = Defaults.VerticalHeadBob;
        InvertYAxis = Defaults.InvertYAxis;
        VoiceChatEnabled = Defaults.VoiceChatEnabled;
        Bindings = Defaults.Bindings;
        SaveSettings();
    }

    public void LoadSettings() {
        if (!File.Exists(settingsFilePath)) {
            File.WriteAllText(settingsFilePath, JsonUtility.ToJson(Defaults, true));

            // NOTE: We may be waiting on FileSystem here so might as well use same data
            VoiceVolume = Defaults.VoiceVolume;
            SFXVolume = Defaults.SFXVolume;
            MouseSensitivity = Defaults.MouseSensitivity;
            HorizontalHeadShake = Defaults.HorizontalHeadShake;
            VerticalHeadBob = Defaults.VerticalHeadBob;
            InvertYAxis = Defaults.InvertYAxis;
            VoiceChatEnabled = Defaults.VoiceChatEnabled;
            Bindings = Defaults.Bindings;
        } else {
            string json = File.ReadAllText(settingsFilePath);
            if (json != "") {
                var data = JsonUtility.FromJson<SettingsData>(json);
                VoiceVolume = data.VoiceVolume;
                SFXVolume = data.SFXVolume;
                MouseSensitivity = data.MouseSensitivity;
                HorizontalHeadShake = data.HorizontalHeadShake;
                VerticalHeadBob = data.VerticalHeadBob;
                InvertYAxis = data.InvertYAxis;
                VoiceChatEnabled = data.VoiceChatEnabled;
                Bindings = data.Bindings;
            }
        }
    }
}

[Serializable]
public class SettingsData {
    public float VoiceVolume = 0;
    public float SFXVolume = 0.5f;
    public float MouseSensitivity = 0.5f;
    public bool HorizontalHeadShake = false;
    public bool VerticalHeadBob = false;
    public bool InvertYAxis = false;
    public bool VoiceChatEnabled = true;
    public string Bindings = string.Empty;
}