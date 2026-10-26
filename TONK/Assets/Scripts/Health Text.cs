using UnityEngine;
using TMPro;

public class HealthText : MonoBehaviour
{
    public string agent;
    public AgentMovement tank;
    public PlayerMovement player;
    public TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        if(tank != null)
        {
            text.text = (agent + ": " + tank.health);
        }
        else
        {
            text.text = (agent + ": " + player.health);
        }
    }
}
