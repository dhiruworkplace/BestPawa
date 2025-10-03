using System;
using UnityEngine;

public class DailyRewardPage : MonoBehaviour
{
    public DayPanel[] dayPanels;

    // Start is called before the first frame update
    void OnEnable()
    {
        SetData();
        CheckForReward();
    }

    private void SetData()
    {
        for (int i = 0; i < dayPanels.Length; i++)
        {
            dayPanels[i].rewardAmount = (i + 1) * 1000;
            dayPanels[i].dayText.text = "Day " + (i + 1).ToString("00");
            dayPanels[i].rewardText.text = ((i + 1) * 1000).ToString();
        }
    }

    private void CheckForReward()
    {
        if (!PlayerPrefs.HasKey("rewardTime"))
        {
            dayPanels[0].collectBtn.SetActive(true);
            dayPanels[0].collectedBtn.SetActive(false);
            dayPanels[0].lockBtn.SetActive(false);

            dayPanels[0].statusImg[2].gameObject.SetActive(false);
            dayPanels[0].statusImg[1].gameObject.SetActive(true);
            dayPanels[0].statusImg[0].gameObject.SetActive(false);
        }
        else
        {
            int inx = PlayerPrefs.GetInt("dayValue", 0);
            for (int i = 0; i < inx; i++)
            {
                dayPanels[i].collectBtn.SetActive(false);
                dayPanels[i].collectedBtn.SetActive(true);
                dayPanels[i].lockBtn.SetActive(false);

                dayPanels[i].statusImg[2].gameObject.SetActive(false);
                dayPanels[i].statusImg[1].gameObject.SetActive(false);
                dayPanels[i].statusImg[0].gameObject.SetActive(true);
            }

            string stime = PlayerPrefs.GetString("rewardTime");
            DateTime prevTime = DateTime.Parse(stime);
            TimeSpan ts = DateTime.Now - prevTime;

            if (ts.TotalHours >= 24 && ts.TotalHours < 48)
            {
                dayPanels[inx].collectBtn.SetActive(true);
                dayPanels[inx].collectedBtn.SetActive(false);
                dayPanels[inx].lockBtn.SetActive(false);

                dayPanels[inx].statusImg[2].gameObject.SetActive(false);
                dayPanels[inx].statusImg[1].gameObject.SetActive(true);
                dayPanels[inx].statusImg[0].gameObject.SetActive(false);
            }
            else if (ts.TotalHours > 48)
            {
                PlayerPrefs.DeleteKey("rewardTime");
                PlayerPrefs.DeleteKey("dayValue");
                PlayerPrefs.Save();
                ResetAll();
                CheckForReward();
            }
        }
    }

    private void ResetAll()
    {
        for (int i = 0; i < dayPanels.Length; i++)
        {
            dayPanels[i].collectBtn.SetActive(false);
            dayPanels[i].collectedBtn.SetActive(false);
            dayPanels[i].lockBtn.SetActive(true);

            dayPanels[i].statusImg[2].gameObject.SetActive(true);
            dayPanels[i].statusImg[1].gameObject.SetActive(false);
            dayPanels[i].statusImg[0].gameObject.SetActive(false);
        }
    }

    public void Claim(int inx)
    {
        DayPanel dayPanel = dayPanels[inx];
        if (!dayPanel.lockBtn.activeSelf && !dayPanel.collectedBtn.activeSelf)
        {
            dayPanel.collectBtn.SetActive(false);
            dayPanel.collectedBtn.SetActive(true);

            dayPanels[inx].statusImg[2].gameObject.SetActive(false);
            dayPanels[inx].statusImg[1].gameObject.SetActive(false);
            dayPanels[inx].statusImg[0].gameObject.SetActive(true);

            ProgressManager.Instance.Progress.stars += 1000 * (inx + 1);
            ProgressManager.Instance.SaveProgress();
            GameUIManager.Instance.UpdateStarsUI();

            ProgressManager.Instance.StartTimer();
            PlayerPrefs.SetInt("dayValue", (inx + 1));
            PlayerPrefs.Save();
        }
        SoundManager.Instance.PlayButtonClick();
    }
}