#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

[CustomEditor(typeof(ContactDirectory))]
public class ContactDirectoryEditor : Editor
{
    private static readonly string IconPath = "Assets/Editor/Icons/contact_directory_icon.png"; // Path to the custom icon for Contact Directory

    public override void OnInspectorGUI()
    {
        // Set custom icon for the Contact Directory ScriptableObject
        SetCustomIcon();

        // Fetch the ContactDirectory ScriptableObject
        ContactDirectory contactDirectory = (ContactDirectory)target;

        // Set a custom large header (e.g., the title of the scriptable object)
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 24, // Large font size for the header
            alignment = TextAnchor.MiddleCenter, // Center the title
            fontStyle = FontStyle.Bold
        };

        // Display the large header/title
        GUILayout.Label("Contact Directory", headerStyle);

        // Add an icon (this assumes you have an icon in your Resources folder)
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath); // Change to Texture2D
        if (icon != null)
        {
            GUILayout.Label(new GUIContent(" ", icon), GUILayout.Width(50), GUILayout.Height(50));
        }

        // Add some space after the header and icon
        GUILayout.Space(10);

        // Draw the default inspector fields (for the ContactDirectory and its contacts list)
        DrawDefaultInspector();

        // Load Contacts Button
        if (GUILayout.Button("Load Contacts"))
        {
            LoadContacts(contactDirectory);
        }

        // Optionally add custom buttons or functionality (e.g., actions related to the directory)
        if (GUILayout.Button("Save All Contacts to JSON"))
        {
            foreach (Contact contact in contactDirectory.contacts)
            {
                string path = EditorUtility.SaveFilePanel("Save Contact as JSON", "", contact.entityName + ".json", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    contact.SaveToJson(path);
                }
            }
        }
    }

    /// <summary>
    /// Sets the custom icon for the ContactDirectory ScriptableObject.
    /// </summary>
    private void SetCustomIcon()
    {
        // Load the custom icon from the resources path
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath); // Change to Texture2D

        // Only set the icon if it exists
        if (icon != null)
        {
            EditorGUIUtility.SetIconForObject(target, icon);
        }
        else
        {
            Debug.LogWarning("Custom icon not found at path: " + IconPath);
        }
    }

    /// <summary>
    /// Loads all contacts found in the same folder as the ContactDirectory.
    /// </summary>
    private void LoadContacts(ContactDirectory contactDirectory)
    {
        // Get the path to the current ContactDirectory asset
        string directoryPath = AssetDatabase.GetAssetPath(contactDirectory);
        string folderPath = Path.GetDirectoryName(directoryPath);

        // Find all contact assets in the same folder
        string[] guids = AssetDatabase.FindAssets("t:Contact", new[] { folderPath });

        // Clear the current contacts list
        contactDirectory.contacts.Clear();

        // Load and add each contact to the list
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Contact contact = AssetDatabase.LoadAssetAtPath<Contact>(path);
            if (contact != null)
            {
                contactDirectory.contacts.Add(contact);
            }
        }

        // Mark the ContactDirectory asset as dirty to save changes
        EditorUtility.SetDirty(contactDirectory);

        // Refresh the editor to reflect the changes
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
