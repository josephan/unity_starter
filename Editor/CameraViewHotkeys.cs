using System.Collections;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Adds a 'Camera' menu containing various views to switch between in the current SceneView
/// </summary>
public static class CameraViewHotkeys {
    /// <summary>
    /// The rotation to restore when going back to perspective view. If we don't have anything,
    /// default to the 'Front' view. This avoids the problem of an invalid rotation locking out
    /// any further mouse rotation
    /// </summary>
    static Quaternion sPerspectiveRotation = Quaternion.Euler(0, 0, 0);

    /// <summary>
    /// Whether the camera should tween between views or snap directly to them
    /// </summary>
    static bool sShouldTween = true;


    /// <summary>
    /// When switching from a perspective view to an orthographic view, record the rotation so
    /// we can restore it later
    /// </summary>
    static private void StorePerspective() {
        if (SceneView.lastActiveSceneView.orthographic == false) {
            sPerspectiveRotation = SceneView.lastActiveSceneView.rotation;
        }
    }

    /// <summary>
    /// Apply an orthographic view to the scene views camera. This stores the previously active
    /// perspective rotation if required
    /// </summary>
    /// <param playerName="newRotation">The new rotation for the orthographic camera</param>
    static private void ApplyOrthoRotation(Quaternion newRotation) {
        StorePerspective();

        SceneView.lastActiveSceneView.orthographic = true;

        if (sShouldTween) {
            SceneView.lastActiveSceneView.LookAt(SceneView.lastActiveSceneView.pivot, newRotation);
        } else {
            SceneView.lastActiveSceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, newRotation);
        }

        SceneView.lastActiveSceneView.Repaint();
    }

    static bool IsSceneActive() {
        return UnityEditor.EditorWindow.focusedWindow.GetType() == typeof(UnityEditor.SceneView);
    }


    [MenuItem("Window/Camera Hotkeys/Top")]
    static void TopCamera() {
        if (!IsSceneActive()) return;
        ApplyOrthoRotation(Quaternion.Euler(90, 0, 0));
    }


    [MenuItem("Window/Camera Hotkeys/Bottom")]
    static void BottomCamera() {
        if (!IsSceneActive()) return;
        ApplyOrthoRotation(Quaternion.Euler(-90, 0, 0));
    }


    [MenuItem("Window/Camera Hotkeys/Left")]
    static void LeftCamera() {
        if (!IsSceneActive()) return;
        ApplyOrthoRotation(Quaternion.Euler(0, 90, 0));
    }


    [MenuItem("Window/Camera Hotkeys/Right")]
    static void RightCamera() {
        if (!IsSceneActive()) return;
        ApplyOrthoRotation(Quaternion.Euler(0, -90, 0));
    }


    [MenuItem("Window/Camera Hotkeys/Front")]
    static void FrontCamera() {
        if (!IsSceneActive()) return;
        ApplyOrthoRotation(Quaternion.Euler(0, 0, 0));
    }

    [MenuItem("Window/Camera Hotkeys/Back")]
    static void BackCamera() {
        if (!IsSceneActive()) return;
        ApplyOrthoRotation(Quaternion.Euler(0, 180, 0));
    }


    [MenuItem("Window/Camera Hotkeys/Persp Camera")]
    static void PerspCamera() {
        if (!IsSceneActive()) return;
        if (SceneView.lastActiveSceneView.camera.orthographic == true) {
            if (sShouldTween) {
                SceneView.lastActiveSceneView.LookAt(SceneView.lastActiveSceneView.pivot, sPerspectiveRotation);
            } else {
                SceneView.lastActiveSceneView.LookAtDirect(SceneView.lastActiveSceneView.pivot, sPerspectiveRotation);
            }

            SceneView.lastActiveSceneView.orthographic = false;

            SceneView.lastActiveSceneView.Repaint();
        }
    }
}