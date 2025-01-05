using UnityEngine;
using UnityEngine.UI;

public class IconLoader : MonoBehaviour
{
    public Sprite icon;
    public Image image;


    private void OnEnable()
    {
        if (icon != null)
        {
            image.sprite = icon;
        }
    }
}
