using UnityEngine;
using UnityEngine.Networking;

public class Physics : NetworkBehaviour
{
    private float m_StartStamina = 10f;

    [ClientRpc]
    public void Rpc_Hit(Vector2 forceDir, float currentStamina)
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

        forceDir *= m_StartStamina / currentStamina;

        rb.AddForce(forceDir, ForceMode2D.Impulse);   
    }
}
