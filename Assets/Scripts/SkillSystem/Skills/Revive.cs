using UnityEngine;

public class Revive : Skill
{
    public float ReviveRange;
    public float ReviveHealthMulti;
    public BuffObject ReviveCoolDownBuff;

    public PlayerController ReviveTarget;

    private GameManager gameManager;
    private StatsTracker statsTracker;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        statsTracker = StatsTracker.Instance;
    }

    public override void Shoot()
    {
        if (!BuffObject.isStackable)
        {
            if (BuffObject.HasBuff(Character.ActiveBuffObjects))
            {
                return;
            }
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }

        var tempPlayer = Character.GetComponent<PlayerController>();
        if (tempPlayer == null)
        {
            return;
        }

        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            if (gameManager.Players[i] == tempPlayer)
            {
                continue;
            }

            if (!gameManager.Players[i].isDeadTrigger)
            {
                continue;
            }

            ReviveTarget = gameManager.Players[i];

            if (Vector3.Distance(Character.transform.position, gameManager.Players[i].transform.position) <= ReviveRange)
            {
                Character.AddBuff(BuffObject, 1, Character);
                SpawnBuff();

                var tempChargeBarBuff = curBuff.GetComponent<ChargeBarBuff>();
                if (tempChargeBarBuff != null)
                {
                    tempChargeBarBuff.ReviveTarget = ReviveTarget;
                }

                Character.canWalk = false;
                Character.canUseRightStick = false;
                Character.canUseSkills = false;

                break;
            }
        }

    }

    public void OnComplete()
    {
        var melee = ReviveTarget.GetComponentInChildren<ReviveSelf_Melee>();
        if (melee != null)
            melee.Deactivate();
        var shooter = ReviveTarget.GetComponentInChildren<ReviveSelf_Shooter>();
        if (shooter != null)
            shooter.Deactivate();
        ReviveTarget.GetHealth(ReviveTarget.MaxHealth * ReviveHealthMulti);

        Character.AddBuff(ReviveCoolDownBuff, 1, Character);

        var PlayerController = Character.GetComponent<PlayerController>();
        if (PlayerController != null)
        {
            statsTracker.RevivedTeamMate[PlayerController.InternalPlayerNumber]++;
        }
    }
}
