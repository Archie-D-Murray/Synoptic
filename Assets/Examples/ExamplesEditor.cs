#if UNITY_EDITOR
using System.Linq;

using UnityEditor;

using UnityEngine;

[InitializeOnLoad]
public class ExampleEditor {

    const string _examplesDefine = "AI_EXAMPLES";

    static ExampleEditor() {
        AddDefine();
    }

    public static void AddDefine() {
#if AI_EXAMPLES
        Debug.Log($"[AI Examples]: {_examplesDefine} already defined in project");
#else
        Debug.Log($"[AI Examples]: Adding define: {_examplesDefine} as examples are present");
        UnityEditor.Build.NamedBuildTarget group = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        string[] defines = new string[10];
        PlayerSettings.GetScriptingDefineSymbols(group, out defines);

        if (!defines.Contains(_examplesDefine)) {
            ArrayUtility.Add(ref defines, _examplesDefine);
        }

        PlayerSettings.SetScriptingDefineSymbols(group, defines);
#endif
    }
}

#endif