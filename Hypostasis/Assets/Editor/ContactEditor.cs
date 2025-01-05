#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Custom editor for the Contact ScriptableObject to add large headers and custom icons in the Inspector.
/// </summary>
[CustomEditor(typeof(Contact))]
public class ContactEditor : Editor
{
    private static readonly string IconPath = "Assets/Editor/Icons/contact_icon.png"; // Ensure the icon is in this path

    public override void OnInspectorGUI()
    {
        // Set custom icon for the Contact ScriptableObject
        SetCustomIcon();

        // Fetch the Contact ScriptableObject
        Contact contact = (Contact)target;

        // Set a custom large header (e.g., the title of the scriptable object)
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 24, // Large font size for the header
            alignment = TextAnchor.MiddleCenter, // Center the title
            fontStyle = FontStyle.Bold
        };

        // Display the large header/title
        GUILayout.Label("Contact Information", headerStyle);

        // Add an icon (this assumes you have an icon in your Resources folder)
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath); // Change to Texture2D
        if (icon != null)
        {
            GUILayout.Label(new GUIContent(" ", icon), GUILayout.Width(50), GUILayout.Height(50)); // Display icon
        }
        else
        {
            GUILayout.Label("No Icon Available", GUILayout.Width(50), GUILayout.Height(50));
        }

        // Add some space after the header and icon
        GUILayout.Space(10);

        // Draw the default inspector fields
        DrawDefaultInspector();

        // Optionally add custom buttons or functionality, such as save and sync
        if (GUILayout.Button("Save to JSON"))
        {
            string path = EditorUtility.SaveFilePanel("Save Contact as JSON", "", contact.entityName + ".json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                contact.SaveToJson(path);
            }
        }

        if (GUILayout.Button("Sync to Sync Folder"))
        {
            string syncFolderPath = EditorUtility.OpenFolderPanel("Select Sync Folder", "", "");
            if (!string.IsNullOrEmpty(syncFolderPath))
            {
                contact.SyncToFolder(syncFolderPath);
            }
        }
    }
    public void SaveToJson(string filePath)
    {
        string json = JsonUtility.ToJson(this, true); // Serialize the object to JSON
        File.WriteAllText(filePath, json); // Write JSON to file
        Debug.Log($"Contact saved to JSON: {filePath}");
    }
    /// <summary>
    /// Sets the custom icon for the Contact ScriptableObject.
    /// </summary>
    private void SetCustomIcon()
    {
        // Load the custom icon from the resources path
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath); // Change to Texture2D

        // Only set the icon if it exists
        if (icon != null)
        {
            EditorGUIUtility.SetIconForObject(target, icon); // Set custom icon
        }
        else
        {
            Debug.LogWarning("Custom icon not found at path: " + IconPath); // Warn if no icon found
        }
    }
}
#endif
