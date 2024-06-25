using UnityEngine;
using UnityEditor;
using System.Reflection;

public static class CloseActiveWindow {
    [MenuItem("Edit/Close Active Window ^w", false, -101)]
    public static void CloseWindow() {
        var window = EditorWindow.focusedWindow;
        if (window != null) {
            BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            MethodInfo isDockedMethod = typeof(EditorWindow).GetProperty("docked", fullBinding).GetGetMethod(true);

            if ((bool)isDockedMethod.Invoke(window, null) == false) {
                window.Close();
            }
        }
    }
}
