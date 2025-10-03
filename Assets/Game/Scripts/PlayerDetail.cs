using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetail : MonoBehaviour
{
    public Image panel;
    public Image medal;
    public TextMeshProUGUI rank;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI points;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetData(int _rank, string name, int _points)
    {
        rank.text = _rank.ToString("00");
        playerName.text = name;
        points.text = _points.ToString();
    }
}