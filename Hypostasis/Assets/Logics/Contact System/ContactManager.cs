using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ContactManager : MonoBehaviour
{
    public ContactDirectory contactDirectory;
    public UltimateMobileQuickbar mobileQuickbar; // Reference to the Quickbar system
    public Sprite[] buttonInfoIcons; // Array of icons for Quickbar buttons
    public InputField searchInputField; // Input field for search
    public Dropdown contactsDropdown; // Dropdown for contact names
    public Dropdown categoryDropdown; // Dropdown for category filtering
    public Text displayText; // Display for contact details

    public Button callButton;   // Call button
    public Button emailButton;  // Email button
    public Button locationButton; // Location button
    public Button websiteButton; // Website button
    public Button gpsButton; // GPS button

    [Header("Dropdown Customization")]
    public Color contactsDropdownBackgroundColor = Color.white; // Background color for contacts dropdown
    public Sprite contactsDropdownBackgroundImage; // Background image for contacts dropdown
    public Color categoryDropdownBackgroundColor = Color.white; // Background color for category dropdown
    public Sprite categoryDropdownBackgroundImage; // Background image for category dropdown

    private List<Contact> filteredContacts; // Temporary storage for filtered contacts
    private Contact currentContact; // Currently selected contact

    void Start()
    {
        filteredContacts = new List<Contact>(contactDirectory.contacts);

        CustomizeDropdownBackground(contactsDropdown, contactsDropdownBackgroundColor, contactsDropdownBackgroundImage);
        CustomizeDropdownBackground(categoryDropdown, categoryDropdownBackgroundColor, categoryDropdownBackgroundImage);

        PopulateContactsDropdown(filteredContacts);
        PopulateCategoryDropdown();

        // Assign listeners
        if (searchInputField != null)
            searchInputField.onValueChanged.AddListener(SearchContacts);

        if (categoryDropdown != null)
            categoryDropdown.onValueChanged.AddListener(FilterByCategoryDropdown);

        if (contactsDropdown != null)
            contactsDropdown.onValueChanged.AddListener(ShowSelectedContactDetails);

        ShowSelectedContactDetails(0); // Clear Quickbar and reset display for no contact selected

    }

    private void CustomizeDropdownBackground(Dropdown dropdown, Color backgroundColor, Sprite backgroundImage)
    {
        if (dropdown == null || dropdown.template == null) return;

        var dropdownTemplate = dropdown.template.GetComponent<Image>();

        if (backgroundImage != null)
        {
            dropdownTemplate.sprite = backgroundImage;
            dropdownTemplate.color = Color.white; // Reset color if using an image
        }
        else
        {
            dropdownTemplate.sprite = null; // Clear the sprite if using color
            dropdownTemplate.color = backgroundColor;
        }
    }

    private void PopulateContactsDropdown(List<Contact> contacts)
    {
        if (contactsDropdown == null) return;

        contactsDropdown.ClearOptions();
        var contactNames = contacts.Select(c => c.entityName).ToList();
        contactNames.Insert(0, "View Available Matches"); // Placeholder option
        contactsDropdown.AddOptions(contactNames);

        filteredContacts = contacts; // Update the filtered list
        contactsDropdown.value = 0; // Reset dropdown to "Select Contact"
        contactsDropdown.RefreshShownValue(); // Refresh dropdown to reflect changes
    }

    private void PopulateCategoryDropdown()
    {
        if (categoryDropdown == null) return;

        categoryDropdown.ClearOptions();
        var primaryCategories = System.Enum.GetNames(typeof(Contact.PrimaryCategory)).ToList();
        primaryCategories.Insert(0, "Filter By Category"); // Add "All" option
        categoryDropdown.AddOptions(primaryCategories);
    }

    private void SearchContacts(string searchKeyword)
    {
        // Convert the keyword to lowercase for case-insensitive comparison
        searchKeyword = searchKeyword.ToLower();

        var matchedContacts = contactDirectory.contacts
            .Where(c =>
                c.entityName.ToLower().Contains(searchKeyword) ||
                c.location.ToLower().Contains(searchKeyword) ||
                c.phoneNumber.ToLower().Contains(searchKeyword) ||
                c.email.ToLower().Contains(searchKeyword) ||
                c.website.ToLower().Contains(searchKeyword) ||
                c.gpsCoordinates.ToString().ToLower().Contains(searchKeyword) ||
                c.primaryCategory.ToString().ToLower().Contains(searchKeyword) ||
                c.secondaryCategories.Any(category => category.ToLower().Contains(searchKeyword))
            )
            .ToList();

        // Apply category filtering after search
        FilterByCategory(matchedContacts);
    }

    private void FilterByCategoryDropdown(int dropdownIndex)
    {
        if (dropdownIndex == 0) // "All" selected
        {
            // Reset to unfiltered contact list
            PopulateContactsDropdown(contactDirectory.contacts);
            return;
        }

        var selectedCategory = (Contact.PrimaryCategory)(dropdownIndex - 1); // Adjust for "All"

        var filteredByCategory = contactDirectory.contacts
            .Where(c => c.primaryCategory == selectedCategory ||
                        c.secondaryCategories.Contains(selectedCategory.ToString()))
            .ToList();

        PopulateContactsDropdown(filteredByCategory);
    }

    private void FilterByCategory(List<Contact> contacts)
    {
        int categoryIndex = categoryDropdown != null ? categoryDropdown.value : 0;

        if (categoryIndex == 0) // "All" selected
        {
            PopulateContactsDropdown(contacts); // Reset to all contacts
            return;
        }

        var selectedCategory = (Contact.PrimaryCategory)(categoryIndex - 1); // Adjust for "All"

        var filteredContacts = contacts
            .Where(c => c.primaryCategory == selectedCategory ||
                        c.secondaryCategories.Contains(selectedCategory.ToString()))
            .ToList();

        PopulateContactsDropdown(filteredContacts);
    }

    public void ShowSelectedContactDetails(int dropdownIndex)
    {
        if (mobileQuickbar == null)
        {
            Debug.LogWarning("Mobile Quickbar is not assigned!");
            return;
        }

        // Clear the Quickbar using the existing method
        mobileQuickbar.ClearQuickbar();

        if (dropdownIndex == 0) // "Select a Contact" selected
        {
            displayText.text = string.Empty;
            currentContact = null;
            return;
        }

        if (dropdownIndex > filteredContacts.Count) return; // Ignore invalid selection

        currentContact = filteredContacts[dropdownIndex - 1]; // Adjust for placeholder
        displayText.text = FormatContactDetails(currentContact);

        // Add phone functionality to the Quickbar
        if (!string.IsNullOrEmpty(currentContact.phoneNumber))
        {
            var callButtonInfo = new UltimateMobileQuickbarButtonInfo
            {
                id = 1,
                key = "Call",
                icon = callButton.image.sprite
            };
            mobileQuickbar.RegisterToQuickbar(LaunchCall, callButtonInfo, true);
            callButton.gameObject.SetActive(false); // Hide original button
        }

        // Add email functionality to the Quickbar
        if (!string.IsNullOrEmpty(currentContact.email))
        {
            var emailButtonInfo = new UltimateMobileQuickbarButtonInfo
            {
                id = 2,
                key = "Email",
                icon = emailButton.image.sprite
            };
            mobileQuickbar.RegisterToQuickbar(LaunchEmail, emailButtonInfo, true);
            emailButton.gameObject.SetActive(false); // Hide original button
        }

        // Add GPS functionality to the Quickbar
        if (currentContact.gpsCoordinates != Vector2.zero)
        {
            var gpsButtonInfo = new UltimateMobileQuickbarButtonInfo
            {
                id = 3,
                key = "GPS",
                icon = gpsButton.image.sprite
            };
            mobileQuickbar.RegisterToQuickbar(LaunchNavigation, gpsButtonInfo, true);
            gpsButton.gameObject.SetActive(false); // Hide original button
        }

        // Add website functionality to the Quickbar
        if (!string.IsNullOrEmpty(currentContact.website))
        {
            var websiteButtonInfo = new UltimateMobileQuickbarButtonInfo
            {
                id = 4,
                key = "Website",
                icon = websiteButton.image.sprite
            };
            mobileQuickbar.RegisterToQuickbar(OpenWebsite, websiteButtonInfo, true);
            websiteButton.gameObject.SetActive(false); // Hide original button
        }
    }


    private string FormatContactDetails(Contact contact)
    {
        return $"Entity: {contact.entityName}\n" +
               $"Location: {contact.location}\n" +
               $"Phone: {contact.phoneNumber}\n" +
               $"Email: {contact.email}\n" +
               $"Website: {contact.website}\n" +
               $"GPS: {contact.gpsCoordinates}\n" +
               $"Primary Category: {contact.primaryCategory}\n" +
               $"Secondary Categories: {string.Join(", ", contact.secondaryCategories)}";
    }

    private void LaunchCall()
    {
        if (currentContact != null && !string.IsNullOrEmpty(currentContact.phoneNumber))
        {
            Application.OpenURL($"tel:{currentContact.phoneNumber}");
        }
    }

    private void LaunchEmail()
    {
        if (currentContact != null && !string.IsNullOrEmpty(currentContact.email))
        {
            string subject = UnityWebRequest.EscapeURL("Red Mobile User Inquiry").Replace("+", "%20");
            string emailURL = $"mailto:{currentContact.email}?subject={subject}";
            Application.OpenURL(emailURL);
        }
    }

    private void LaunchNavigation()
    {
        if (currentContact != null && currentContact.gpsCoordinates != Vector2.zero)
        {
            string mapURL = $"https://www.google.com/maps?q={currentContact.gpsCoordinates.x},{currentContact.gpsCoordinates.y}";
            Application.OpenURL(mapURL);
        }
    }

    private void OpenWebsite()
    {
        if (currentContact != null && !string.IsNullOrEmpty(currentContact.website))
        {
            Application.OpenURL(currentContact.website);
        }
    }
}
