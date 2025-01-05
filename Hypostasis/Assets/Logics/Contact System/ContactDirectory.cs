using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContactDirectory", menuName = "The Red/Contact Directory")]
public class ContactDirectory : ScriptableObject
{
    public List<Contact> contacts = new List<Contact>();
}
