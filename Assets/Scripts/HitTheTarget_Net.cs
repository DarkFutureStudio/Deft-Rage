using UnityEngine;
using UnityEngine.Networking;

public class HitTheTarget_Net : NetworkBehaviour
{
    public float m_attackRadius;        
    public Transform m_attackPos;       
    public LayerMask m_playerMask;
    public float m_startTimeBtwAttackS = 0.4f;
    public float m_startTimeBtwAttackU = 0.4f;
    public float m_hitForce = 5f;
    public float m_Damage = 2f;

    private Collider2D m_Collider;
    private bool m_sHit;
    private bool m_uHit;
    private float m_timeBtwAttackS;
    private float m_timeBtwAttackU;
    private PlayerController ClientScript;
    private Vector2 forceDir;
    private AnimController ac;

    private void Start()
    {
        ClientScript = GetComponent<PlayerController>();
        ac = GetComponent<AnimController>();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (m_timeBtwAttackS <= 0 && Input.GetButtonDown("Fire1"))
        {
            ac.HookAnimation();
            m_timeBtwAttackS = m_startTimeBtwAttackS;
        }
        else
            m_timeBtwAttackS -= Time.deltaTime;

        if (m_timeBtwAttackU <= 0 && Input.GetButtonDown("Fire2"))
        {
            ac.UppercutAnimation();
            m_timeBtwAttackU = m_startTimeBtwAttackU;
        }
        else
            m_timeBtwAttackU -= Time.deltaTime;
    }

    private bool CheckPlayer()
    {
        m_Collider = Physics2D.OverlapCircle(m_attackPos.position, m_attackRadius, m_playerMask);
        return m_Collider;
    }

    private void UCheckHit()
    {
        if (!isLocalPlayer)
            return;
        m_uHit = CheckPlayer();
    }

    private void FixedUpdate()
    {
        if (m_sHit)
        {
            StraitHit();
            m_sHit = false;
        }

        if (m_uHit)
        {
            UpperHit();
            m_uHit = false;
        }
    }

    private void SCheckHit()
    {
        if (!isLocalPlayer)
            return;
        m_sHit = CheckPlayer();
    }

    private void StraitHit()
    {
        if (ClientScript.m_facingRight)
            forceDir = new Vector2(m_hitForce, 0f);
        else
            forceDir = new Vector2(-m_hitForce, 0f);

        FinalHit();
    }

    private void UpperHit()
    {
        if (ClientScript.m_facingRight)
            forceDir = new Vector2(m_hitForce, 2 * m_hitForce);
        else
            forceDir = new Vector2(-m_hitForce, 2 * m_hitForce);

        FinalHit();
    }

    private void FinalHit()
    {
        GameObject obj = m_Collider.gameObject;

        Cmd_Hit(forceDir, obj);
        Cmd_Stamina(obj);
    }

    [Command]
    private void Cmd_Stamina(GameObject obj)
    {
        obj.GetComponent<Stamina>().StaminaBurner(m_Damage);
    }

    [Command]
    private void Cmd_Hit(Vector2 forcDir, GameObject obj)
    {
        float currentStamina = obj.GetComponent<Stamina>().CurrentStamina;
        obj.GetComponent<Physics>().Rpc_Hit(forcDir, currentStamina);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_attackPos.position, m_attackRadius);
    }
}
