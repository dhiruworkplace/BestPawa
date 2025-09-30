using UnityEngine;
using UnityEngine.UI;

public class UserAgreementPanel : MonoBehaviour
{
    [Header("Checkbox Buttons")]
    public Button checkBox1Button;
    public Button checkBox2Button;

    [Header("Checkbox Sprites")]
    public Sprite uncheckedSprite;
    public Sprite checkedSprite;

    [Header("Start Button")]
    public Button startButton;

    [Header("Panels")]
    public GameObject userAgreementPanel; // whole panel for agreement
    public GameObject privacyPolicyPanel;
    public GameObject termsPanel;

    [Header("Open Panel Buttons")]
    public Button privacyButton;
    public Button termsButton;

    private bool isCheckBox1Checked = false;
    private bool isCheckBox2Checked = false;

    private const string AGREEMENT_KEY = "UserAgreementAccepted";

    private void Start()
    {
        // Check if already accepted
        if (PlayerPrefs.GetInt(AGREEMENT_KEY, 0) == 1)
        {
            userAgreementPanel.SetActive(false);
            return;
        }

        // If not accepted, show agreement panel
        userAgreementPanel.SetActive(true);

        UpdateCheckboxVisuals();
        UpdateStartButton();

        // Add listeners
        checkBox1Button.onClick.AddListener(() => ToggleCheckbox(1));
        checkBox2Button.onClick.AddListener(() => ToggleCheckbox(2));

        privacyButton.onClick.AddListener(() => OpenPanel(privacyPolicyPanel));
        termsButton.onClick.AddListener(() => OpenPanel(termsPanel));

        startButton.onClick.AddListener(OnStartClicked);
    }

    private void ToggleCheckbox(int id)
    {
        if (id == 1)
            isCheckBox1Checked = !isCheckBox1Checked;
        else if (id == 2)
            isCheckBox2Checked = !isCheckBox2Checked;

        UpdateCheckboxVisuals();
        UpdateStartButton();
    }

    private void UpdateCheckboxVisuals()
    {
        checkBox1Button.image.sprite = isCheckBox1Checked ? checkedSprite : uncheckedSprite;
        checkBox2Button.image.sprite = isCheckBox2Checked ? checkedSprite : uncheckedSprite;
    }

    private void UpdateStartButton()
    {
        startButton.interactable = (isCheckBox1Checked && isCheckBox2Checked);
    }

    private void OpenPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);
    }

    private void OnStartClicked()
    {
        Debug.Log("User agreement accepted. Game starting...");

        // Save agreement so panel won't show again
        PlayerPrefs.SetInt(AGREEMENT_KEY, 1);
        PlayerPrefs.Save();

        // Hide agreement panel
        userAgreementPanel.SetActive(false);

        // Continue game logic (e.g. load main menu)
    }
}
