using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerInDungeon : MonoBehaviour
{
    public Player player;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerHealth;

    public Color goodHealth, mediumHealth, badHealth;

    public void SetUp(Player _player)
    {
        player = _player;
        playerName.text = _player.NickName;
        ToggleReady(false);
    }
    public void ToggleReady(bool toggle)
    {
        if (toggle)
        {
            playerHealth.color = goodHealth;
            playerHealth.text = "Ready";
        }
        else
        {
            playerHealth.color = badHealth;
            playerHealth.text = "Unready";
        }
    }
    public void ShowHealth(float currentHealth, float maxHealth)
    {
        float healthPercentage = (currentHealth / maxHealth) * 100;
        if(healthPercentage >= 50)
        {
            playerHealth.color = goodHealth;
            playerHealth.text = currentHealth.ToString();
        }
        else if (healthPercentage >= 20)
        {
            playerHealth.color = mediumHealth;
            playerHealth.text = currentHealth.ToString();
        }
        else
        {
            playerHealth.color = badHealth;
            playerHealth.text = currentHealth.ToString();
        }
    }
}
