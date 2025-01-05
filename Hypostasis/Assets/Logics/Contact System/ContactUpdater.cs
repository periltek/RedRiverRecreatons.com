using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

public class ContactUpdater : MonoBehaviour
{
    public ContactDirectory contactDirectory; // Local contact directory

    private const string GitHubBaseURL = "https://raw.githubusercontent.com/your-repo/your-branch/contacts/"; // Replace with your repo
    private const string ContactListURL = GitHubBaseURL + "contact_list.json"; // JSON file listing all available contacts

    void Start()
    {
        StartCoroutine(FetchContactUpdates());
    }

    IEnumerator FetchContactUpdates()
    {
        Debug.Log("Fetching updates...");

        // Fetch the contact list from GitHub
        UnityWebRequest listRequest = UnityWebRequest.Get(ContactListURL);
        yield return listRequest.SendWebRequest();

        if (listRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to fetch contact list: {listRequest.error}");
            yield break;
        }

        // Parse the list of contacts
        List<ContactMetadata> remoteContacts = JsonUtility.FromJson<ContactListWrapper>(listRequest.downloadHandler.text).contacts;

        // Process each remote contact
        foreach (var remoteContact in remoteContacts)
        {
            yield return StartCoroutine(UpdateOrAddContact(remoteContact));
        }

        // Remove stale contacts
        RemoveStaleContacts(remoteContacts);

        Debug.Log("Contact update completed!");
    }

    IEnumerator UpdateOrAddContact(ContactMetadata remoteContact)
    {
        // Check if the contact already exists locally
        Contact localContact = contactDirectory.contacts.FirstOrDefault(c => c.entityName == remoteContact.entityName);

        if (localContact != null)
        {
            // Compare dates to see if an update is needed
            if (string.Compare(localContact.HardCodedDate, remoteContact.hardCodedDate) < 0)
            {
                Debug.Log($"Updating contact: {remoteContact.entityName}");
                yield return StartCoroutine(DownloadAndReplaceContact(remoteContact));
            }
        }
        else
        {
            // Add new contact
            Debug.Log($"Adding new contact: {remoteContact.entityName}");
            yield return StartCoroutine(DownloadAndAddContact(remoteContact));
        }
    }

    IEnumerator DownloadAndReplaceContact(ContactMetadata remoteContact)
    {
        UnityWebRequest assetRequest = UnityWebRequest.Get(remoteContact.sourceLink);
        yield return assetRequest.SendWebRequest();

        if (assetRequest.result == UnityWebRequest.Result.Success)
        {
            // Deserialize and replace the contact
            string json = assetRequest.downloadHandler.text;
            Contact updatedContact = JsonUtility.FromJson<Contact>(json);

            int index = contactDirectory.contacts.FindIndex(c => c.entityName == updatedContact.entityName);
            contactDirectory.contacts[index] = updatedContact;
        }
        else
        {
            Debug.LogError($"Failed to download contact: {remoteContact.entityName} - {assetRequest.error}");
        }
    }

    IEnumerator DownloadAndAddContact(ContactMetadata remoteContact)
    {
        UnityWebRequest assetRequest = UnityWebRequest.Get(remoteContact.sourceLink);
        yield return assetRequest.SendWebRequest();

        if (assetRequest.result == UnityWebRequest.Result.Success)
        {
            // Deserialize and add the contact
            string json = assetRequest.downloadHandler.text;
            Contact newContact = JsonUtility.FromJson<Contact>(json);
            contactDirectory.contacts.Add(newContact);
        }
        else
        {
            Debug.LogError($"Failed to download contact: {remoteContact.entityName} - {assetRequest.error}");
        }
    }

    void RemoveStaleContacts(List<ContactMetadata> remoteContacts)
    {
        var remoteNames = new HashSet<string>(remoteContacts.Select(c => c.entityName));
        contactDirectory.contacts.RemoveAll(c => !remoteNames.Contains(c.entityName));

        Debug.Log("Removed stale contacts.");
    }
}

[System.Serializable]
public class ContactMetadata
{
    public string entityName;
    public string hardCodedDate;
    public string sourceLink;
}

[System.Serializable]
public class ContactListWrapper
{
    public List<ContactMetadata> contacts;
}
