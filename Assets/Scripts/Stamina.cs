using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Stamina : NetworkBehaviour
{
    public float m_StartStamina = 10f;
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullStaminaColor = Color.green;
    public Color m_ZeroStaminaColor = Color.red;

    [SyncVar(hook = "SetStaminaUI")]
    private float m_CurrentStamina;

    private bool m_Dead;
    private Text wonText;
    
    private Text player1;
    private Text player2;

    public int p1Score;
    public int p2Score;

    public float CurrentStamina
    {
        get 
        {
            return m_CurrentStamina; 
        }
        set 
        {
            m_CurrentStamina = value; 
        }
    }

    private void Start()
    {
        wonText = TextUI.instance.MyText;
        player1 = TextUI.instance.player1;
        player2 = TextUI.instance.player2;

        player1.text = p1Score.ToString();
        player2.text = p2Score.ToString();
    }

    private void OnEnable()
    {
        m_CurrentStamina = m_StartStamina;

        p1Score = 0;
        p2Score = 0;

        SetStaminaUI(m_CurrentStamina);
    }

    void Update()
    {
        if (Server.instance.gameOver == true)
        {
            Rpc_decideTheWinner();
        }
    }

    public void StaminaBurner(float amount)
    {
        if (!isServer)
            return;

        m_CurrentStamina -= amount;

        if (m_CurrentStamina <= 0)
        {
            m_CurrentStamina = m_StartStamina;
            Rpc_OnDeath();
        }
    }

    private void SetStaminaUI(float currentStamina)
    {
        m_Slider.value = currentStamina;

        m_FillImage.color = Color.Lerp(m_ZeroStaminaColor, m_FullStaminaColor, currentStamina / m_StartStamina);
    }

    [ClientRpc]
    public void Rpc_OnDeath()
    {
        if (isLocalPlayer)
        {
            p2Score++;
            player2.text = p2Score.ToString();
            transform.position = SpawnScript.instance.Spawn.position;
        }
        else
        {
            p1Score++;
            player1.text = p1Score.ToString();
        }            
    }

    [ClientRpc]
    public void Rpc_decideTheWinner()
    {
        if (isLocalPlayer)
        {
            int p1 = int.Parse(player1.text);
            int p2 = int.Parse(player2.text);

            if (p1 > p2)
            {
                wonText.text = "You Win!";
            }
            else if (p2 > p1)
            {
                wonText.text = "You Lose!";
            }
            else
            {
                wonText.text = "Even!";
            }
        }
    }
}