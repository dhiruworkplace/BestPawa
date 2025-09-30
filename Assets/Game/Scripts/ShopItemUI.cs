using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    // public TMP_Text nameText;
    public Image iconImage, gemImage;
    public TMP_Text priceText;
    public Button actionButton;
    public TMP_Text actionButtonText;

    private int itemIndex;
    private UnityAction<int> onAction;

    /// <summary>
    /// Sets up the shop item display.
    /// </summary>
    public void Setup(string characterName, Sprite icon, int price, bool unlocked, bool selected, int index, UnityAction<int> actionCallback)
    {
        // nameText.text = characterName;
        iconImage.sprite = icon;
        itemIndex = index;
        onAction = actionCallback;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onAction?.Invoke(itemIndex));

        if (!unlocked)
        {
            priceText.text = price.ToString();
            actionButtonText.text = "Buy";
            actionButton.interactable = true;
            gemImage.enabled = true;
        }
        else
        {
            priceText.text = "Unlocked";
            gemImage.enabled = false;

            if (selected)
            {
                actionButtonText.text = "Selected";
                actionButton.interactable = false;
            }
            else
            {
                actionButtonText.text = "Select";
                actionButton.interactable = true;
            }
        }
    }
}
