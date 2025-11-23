using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class EnsureRenderOnlyLayer2D : MonoBehaviour
{
    [MenuItem("URP/Ensure 2D Render Only Layer")]
    static void EnsureRenderObjectsFeature2D()
    {
        var selected = Selection.activeObject as Renderer2DData;
        if (selected == null)
        {
            Debug.LogError("Please select a Renderer2DData asset in the Project window!");
            return;
        }

        bool found = false;

        foreach (var feature in selected.rendererFeatures)
        {
            if (feature != null && feature.GetType().Name == "RenderObjects")
            {
                // Only process if it's already a 2D-compatible feature
                SerializedObject so = new SerializedObject(feature);
                var useRenderingLayerProp = so.FindProperty("m_UseRenderingLayers");
                if (useRenderingLayerProp != null)
                {
                    useRenderingLayerProp.boolValue = true; // enable Render Only Rendering Layer
                    so.ApplyModifiedProperties();
                    found = true;
                    Debug.Log("Render Objects 2D feature found and updated!");
                }
            }
        }

        if (!found)
        {
            // Create a new RenderObjects 2D feature
            var renderObjects2D = ScriptableObject.CreateInstance("RenderObjects") as ScriptableRendererFeature;

            // Enable the Render Only Rendering Layer property
            SerializedObject so = new SerializedObject(renderObjects2D);
            var useRenderingLayerProp = so.FindProperty("m_UseRenderingLayers");
            if (useRenderingLayerProp != null)
                useRenderingLayerProp.boolValue = true;

            so.ApplyModifiedProperties();

            selected.rendererFeatures.Add(renderObjects2D);
            EditorUtility.SetDirty(selected);
            Debug.Log("2D Render Objects feature added with Render Only Rendering Layer enabled!");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
