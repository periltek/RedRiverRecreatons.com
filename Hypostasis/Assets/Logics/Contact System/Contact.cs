    using UnityEngine;
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using UnityEditor;

    /// <summary>
    /// ScriptableObject for storing contact information related to the Red River Gorge.
    /// Provides functionality to save the contact to a JSON file and sync it with a folder.
    /// </summary>
    [CreateAssetMenu(fileName = "Contact", menuName = "The Red/Contact")]
    public class Contact : ScriptableObject
    {   

        #region Contact Information

        [Header("Basic Information")]
        [Tooltip("Name of the entity, business, or individual")]
        public string entityName;

        [Tooltip("A brief description or summary of the contact")]
        [TextArea(3, 5)]
        public string summary;

        [Tooltip("Physical address or location name of the contact")]
        public string location;

        [Tooltip("Phone number of the contact")]
        public string phoneNumber;

        [Tooltip("Email address of the contact")]
        public string email;

        [Tooltip("Website URL for the contact")]
        public string website;

        [Tooltip("GPS coordinates in Latitude, Longitude format")]
        public Vector2 gpsCoordinates;

        [Tooltip("General area name or description")]
        public string generalArea;
        
    #endregion

    #region Auto-Generated Fields

    [Header("Auto-Generated Fields")]
        [SerializeField, HideInInspector]
        private string hardCodedDate; // Immutable creation timestamp (internally set)

        [Tooltip("Public getter for accessing the timestamp")]
        public string HardCodedDate => hardCodedDate;

        [Tooltip("GitHub reference for future updates")]
        public string sourceLink;

        [Tooltip("Enable debugging to print the creation date in the console")]
        public bool debug = false;

        #endregion

        #region Categories

        [Header("Categories")]
        [Tooltip("Primary category of the contact")]
        public PrimaryCategory primaryCategory;

        [Tooltip("Secondary categories associated with the contact")]
        public string[] secondaryCategories;

        #endregion

        #region Enum Definitions

        /// <summary>
        /// Enum representing primary categories for the contact.
        /// </summary>
        public enum PrimaryCategory
        {
            Outdoors,
            Tours,
            Attractions,
            Lodging,
            Camping,
            Dining,
            Shopping,
            ForestRecs,
            WaterRecs,
            AutoRecs,
            ArielRecs,
            Services,
            Hiking,
            Community,
            ClimbingRecs,
            OffroadRecs,
            PointsOfInterest,
            Waterfalls,
            HistoricLocations,
            ScenicViews,
            Trails,
            ArtsAndCrafts,
        }

        #endregion

        #region Editor and File Handling

        /// <summary>
        /// Automatically sets the creation date in Eastern Time when the ScriptableObject is created.
        /// </summary>
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(hardCodedDate))
            {
                TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternTimeZone);
                hardCodedDate = localTime.ToString("MMddyyyyHHmm");
            }
        }

        /// <summary>
        /// Logs the creation date in the console (with parsing) when a value in the Inspector changes.
        /// </summary>
        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(hardCodedDate) && debug)
            {
                try
                {
                    DateTime parsedDate = DateTime.ParseExact(hardCodedDate, "MMddyyyyHHmm", null);
                    TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeToUtc(parsedDate).ToLocalTime();
                    Debug.Log($"Contact '{entityName}' created on: {localTime:MMMM dd, yyyy - hh:mm tt} (Eastern Time)");
                }
                catch (FormatException)
                {
                    Debug.LogWarning($"Unable to parse the hardCodedDate for contact '{entityName}': {hardCodedDate}");
                }
            }
        }

        /// <summary>
        /// Save the contact data as a JSON file.
        /// </summary>
        public void SaveToJson(string filePath)
        {
            try
            {
                string json = JsonUtility.ToJson(this, true); // Convert the object to a JSON string
                File.WriteAllText(filePath, json, Encoding.UTF8); // Write the JSON data to a file
                Debug.Log($"Contact '{entityName}' saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving contact to JSON: {ex.Message}");
            }
        }

        /// <summary>
        /// Sync the contact to a folder, overwriting if a contact with the same name exists.
        /// </summary>
        public void SyncToFolder(string syncFolderPath)
        {
            try
            {
                if (!Directory.Exists(syncFolderPath))
                {
                    Directory.CreateDirectory(syncFolderPath); // Create the folder if it doesn't exist
                }

                string filePath = Path.Combine(syncFolderPath, $"{entityName}.json");

                // Save the contact to the sync folder (overwrites if the contact already exists)
                SaveToJson(filePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error syncing contact to folder: {ex.Message}");
            }
        }

        #endregion

        #region Secondary Categories Dropdown

        // This dictionary holds possible secondary categories available for selection
        private static readonly List<string> allSecondaryCategories = new List<string>
        {
            "Camping", "Hiking", "Fishing", "Wildlife Watching", "Rock Climbing", "Caving", "Water Sports", "Picnicking"
        };

        /// <summary>
        /// Returns a list of available secondary categories to be used in the Inspector dropdown.
        /// </summary>
        public static List<string> GetSecondaryCategoryOptions()
        {
            return allSecondaryCategories;
        }

        #endregion
    }
