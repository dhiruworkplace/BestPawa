using System;
using System.Collections.Generic;
using UnityEngine;

public class LootStarShop : MonoBehaviour
{
    public GameObject noCoinPanel;
    public GameObject[] balls;
    private List<int> prices = new List<int>() { 0, 1000, 2000, 3000, 4000, 5000 };

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("p0", 1);
        PlayerPrefs.Save();

        CheckAllBalls();
    }

    private void CheckAllBalls()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            GameObject m = balls[i];
            if (PlayerPrefs.GetInt("p" + i, 0) == 1)
            {
                m.transform.GetChild(2).gameObject.SetActive(false);
                m.transform.GetChild(3).gameObject.SetActive(true);
                m.transform.GetChild(4).gameObject.SetActive(false);
                m.transform.GetChild(0).gameObject.SetActive(false);
            }
            if (i.Equals(GameManager.selectedCube))
            {
                m.transform.GetChild(0).gameObject.SetActive(true);
                m.transform.GetChild(4).gameObject.SetActive(true);
                ProgressManager.Instance.OnCharacterSelected?.Invoke(GameManager.selectedCube);
            }
        }
    }

    public void SelectBall(int index)
    {
        if (PlayerPrefs.GetInt("p" + index, 0) == 1)
        {
            //if (!Container.selectedBg.Equals(index))
            {
                GameManager.selectedCube = index;
                CheckAllBalls();
            }
        }
        else
        {
            if (ProgressManager.Instance.Progress.stars >= prices[index])
            {
                ProgressManager.Instance.Progress.stars -= prices[index];
                ProgressManager.Instance.SaveProgress();
                GameUIManager.Instance.UpdateStarsUI();
                balls[index].transform.GetChild(2).gameObject.SetActive(false);
                balls[index].transform.GetChild(3).gameObject.SetActive(true);
                balls[index].transform.GetChild(4).gameObject.SetActive(false);
                balls[index].transform.GetChild(0).gameObject.SetActive(false);
                PlayerPrefs.SetInt("p" + index, 1);
                PlayerPrefs.Save();
            }
            else
            {
                noCoinPanel.SetActive(true);
            }
        }
        SoundManager.Instance.PlayButtonClick();
    }
}