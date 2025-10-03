using UnityEngine;

public class MyLetter : MonoBehaviour
{
    public SpriteRenderer letter;

    private void OnTriggerEnter(Collider other)
    {
        GameManager.letters.Add(letter.sprite.name.ToUpper());
        Destroy(gameObject);
    }
}