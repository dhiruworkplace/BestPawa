using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Data")]
    public List<CharacterData> characters;   // ScriptableObjects
    public GameObject shopItemPrefab;        // Prefab with ShopItemUI
    public Transform shopContent;            // Scroll View content
    public TMP_Text starsText;               // Stars display UI

    [Header("Navigation")]
    public Button backButton; // Assign in Inspector

    void Start()
    {
        PopulateShop();
        UpdateStarsUI();

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    #region Shop UI
    void PopulateShop()
    {
        // Clear existing items
        foreach (Transform child in shopContent)
            Destroy(child.gameObject);

        var progress = ProgressManager.Instance.Progress;

        for (int i = 0; i < characters.Count; i++)
        {
            var charData = characters[i];
            var itemObj = Instantiate(shopItemPrefab, shopContent);
            var shopItemUI = itemObj.GetComponent<ShopItemUI>();

            bool unlocked = i < progress.unlockedCharacters.Length && progress.unlockedCharacters[i];
            bool selected = progress.selectedCharacterIndex == i;

            shopItemUI.Setup(
                charData.characterName,
                charData.characterSprite,
                charData.price,
                unlocked,
                selected,
                i,
                OnShopItemAction
            );
        }
    }
    #endregion

    #region Buy / Select
    void OnShopItemAction(int index)
    {
        var charData = characters[index];
        var progress = ProgressManager.Instance.Progress;

        if (!progress.unlockedCharacters[index])
        {
            TryBuyCharacter(index, charData);
        }
        else
        {
            SelectCharacter(index);
        }
    }

    void TryBuyCharacter(int index, CharacterData charData)
    {

        // int coins = PlayerPrefs.GetInt("coins", 0);
        var progress = ProgressManager.Instance.Progress;

        if (progress.stars >= charData.price)
        {
            progress.stars -= charData.price;
            progress.unlockedCharacters[index] = true;
            progress.selectedCharacterIndex = index; // Auto-select
            ProgressManager.Instance.SelectCharacter(index);
            ProgressManager.Instance.SaveProgress();
            // PlayerPrefs.SetInt("coins", coins);
            // PlayerPrefs.Save();
            // starsText.text = PlayerPrefs.GetInt("coins", 0).ToString();
            starsText.text = progress.stars.ToString();
            PopulateShop();
            UpdateStarsUI();
            SoundManager.Instance.PlayButtonClick();
        }
        else
        {
            // Debug.Log("Not enough stars to buy " + charData.characterName);
        }
    }

    void SelectCharacter(int index)
    {
        SoundManager.Instance.PlayButtonClick();
        ProgressManager.Instance.SelectCharacter(index);
        PopulateShop();
    }
    #endregion

    #region Stars
    public void AddStars(int amount)
    {
        ProgressManager.Instance.Progress.stars += amount;
        ProgressManager.Instance.SaveProgress();
        UpdateStarsUI();
    }

    void UpdateStarsUI()
    {
        // starsText.text = PlayerPrefs.GetInt("coins",0).ToString();
        starsText.text = ProgressManager.Instance.Progress.stars.ToString();
    }
    #endregion

    #region Navigation
    public void OnBackButton()
    {
        SoundManager.Instance.PlayButtonClick();
        gameObject.SetActive(false);
    }
    #endregion
}
