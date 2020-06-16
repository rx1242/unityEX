using UnityEditor;
using UnityEngine;

public class ProjectColore
{
    
    const string MenuPath = "Tools/ColoredProjectView";

    [MenuItem(MenuPath)]
    private static void ToggleEnabled()
    {
        Menu.SetChecked(MenuPath, !Menu.GetChecked(MenuPath));
    }

    [InitializeOnLoadMethod]
    private static void SetEvent()
    {
        EditorApplication.projectWindowItemOnGUI += OnGUI;
    }

    private static void OnGUI(string guid, Rect selectionRect)
    {
        if (!Menu.GetChecked(MenuPath))
        {
            return;
        }

        var assetPath = AssetDatabase.GUIDToAssetPath(guid);
        var pathLevel = CountWord(assetPath, "/");
        
        var originalColor = GUI.color;
        GUI.color = GetColorByPathLevel(pathLevel);
        GUI.Box(selectionRect, string.Empty);
        GUI.color = originalColor;
    }

    private static int CountWord(string source, string word)
    {
        return source.Length - source.Replace(word, "").Length;
    }
    
    private static Color GetColorByPathLevel(int pathLevel)
    {
        switch (pathLevel)
        {
            case 1: return new Color(1.4f, 1.4f, 0.0f, 0.1f);
            case 2: return new Color(1.6f, 0.0f, 0.0f, 0.1f);
            case 3: return new Color(0.0f, 1.6f, 0.0f, 0.1f);
            case 4: return new Color(0.8f, 0.0f, 1.4f, 0.1f);
            case 5: return new Color(0.0f, 0.8f, 1.6f, 0.1f);
            case 6: return new Color(0.0f, 0.0f, 0.0f, 0.1f);
        }
        return new Color(0, 0, 0, 0);
    }

}
