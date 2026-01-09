using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FindMissingScripts
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    public static void FindInScene()
    {
        int count = 0;
        var scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();

        foreach (var go in roots)
        {
            count += FindInGameObject(go);
        }

        Debug.Log($"✅ Scan terminé. Missing scripts trouvés: {count}");
    }

    private static int FindInGameObject(GameObject go)
    {
        int found = 0;

        // Check components on this object
        var components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                found++;
                Debug.LogWarning($"❌ Missing script on: {GetFullPath(go)}", go);
            }
        }

        // Recurse children
        for (int i = 0; i < go.transform.childCount; i++)
        {
            found += FindInGameObject(go.transform.GetChild(i).gameObject);
        }

        return found;
    }

    private static string GetFullPath(GameObject go)
    {
        string path = go.name;
        Transform t = go.transform.parent;
        while (t != null)
        {
            path = $"{t.name}/{path}";
            t = t.parent;
        }
        return path;
    }
}
