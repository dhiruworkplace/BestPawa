using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Shop/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
    public int price;
    public bool unlockedByDefault;

    public Material material; // assign per character in Inspector
}
