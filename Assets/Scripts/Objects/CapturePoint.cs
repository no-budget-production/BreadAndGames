using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    public RectTransform CaptureProgressBar;
    public Collider TriggerCollider;

    public string ValidTag;

    public bool Captured;
    public int MinCapturingPlayersRequired;
    public int CurrentCapturingPlayers;
    public float CurrentProgress;
    public float MaxProgress;
    public float StepIncreaseProgress;
    public float StepDecreaseProgress;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ValidTag)
        {
            CurrentCapturingPlayers++;
            if (CurrentCapturingPlayers >= MinCapturingPlayersRequired)
            {
                Captured = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == ValidTag)
        {
            CurrentCapturingPlayers--;
            if (CurrentCapturingPlayers <= MinCapturingPlayersRequired)
            {
                Captured = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Captured)
        {
            IncreaseProgress();
        }
        else
        {
            DecreaseProgress();
        }
    }

    void CheckCaptureRequirement()
    {
        if (CurrentCapturingPlayers >= MinCapturingPlayersRequired)
        {
            Captured = true;
            return;
        }
        else
        {
            Captured = false;
        }
    }

    void IncreaseProgress()
    {
        if ((CurrentProgress + StepIncreaseProgress) >= MaxProgress)
        {
            if (CurrentProgress == MaxProgress)
            {
                return;
            }
            CurrentProgress = MaxProgress;
            OnChangeProgress(CurrentProgress);
        }
        else
        {
            CurrentProgress += Mathf.Clamp(StepIncreaseProgress, 0, StepIncreaseProgress);
            OnChangeProgress(CurrentProgress);
        }
    }

    void DecreaseProgress()
    {
        if ((CurrentProgress - StepDecreaseProgress) <= 0)
        {
            if (CurrentProgress == 0)
            {
                return;
            }
            CurrentProgress = 0;
            OnChangeProgress(CurrentProgress);
        }
        else
        {
            CurrentProgress -= Mathf.Clamp(StepDecreaseProgress, 0, StepDecreaseProgress);
            OnChangeProgress(CurrentProgress);
        }
    }

    void OnChangeProgress(float currentProgress)
    {
        CaptureProgressBar.sizeDelta = new Vector2(currentProgress / MaxProgress * 100, CaptureProgressBar.sizeDelta.y);
    }
}
