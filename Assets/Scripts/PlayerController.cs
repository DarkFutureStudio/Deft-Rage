using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
   
    public float speed = 10.0f;
    public GameObject groundPrefab;
    public Transform groundSpawn;
    public float deathTime;
    [HideInInspector] public bool m_facingRight = false;

    public float groundCooldown = 2.5f;

    private Rigidbody2D rb;
    private Transform m_mainCamera;
    private Vector3 m_cameraOffset;

    public float JumpForce = 1000f;
    public float x;

    public Transform groundCheck;
    public LayerMask WhatIsGround;
    private bool doubleJump = false;
    public float GroundRadius = 1.5f;
    public bool grounded = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, GroundRadius);
    }

    void Start()
    {
        if (isLocalPlayer)
            rb = GetComponent<Rigidbody2D>();

        m_mainCamera = Camera.main.transform;
        m_cameraOffset = new Vector3(0f, 0f, -10f);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        grounded = Physics2D.OverlapCircle(groundCheck.position, GroundRadius, WhatIsGround);
        if (grounded)
            doubleJump = false;

        x = Input.GetAxis("Horizontal");
        Vector2 direction = new Vector2(x, 0.0f);
        transform.Translate(direction * speed * Time.deltaTime);

        if ((grounded || !doubleJump) && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(new Vector2(0, JumpForce));

            if (!doubleJump && !grounded)
                doubleJump = true;
        }

        if (x > 0 && !m_facingRight)
            LocalFlip();
        else if (x < 0 && m_facingRight)
            LocalFlip();

        if (groundCooldown <= 0 && Input.GetButtonDown("Instanciate"))
        {
            Cmd_secondChance();
            groundCooldown = 2.5f;
        }
        else
        {
            groundCooldown -= Time.deltaTime;
        }

        MoveCamera();
    }

    [Command]
    void Cmd_secondChance()
    {
        GameObject ground = (GameObject)Instantiate(groundPrefab, groundSpawn.position, groundSpawn.rotation);
        NetworkServer.Spawn(ground);
        Destroy(ground, deathTime);
    }

    private void LocalFlip()
    {
        m_facingRight = !m_facingRight;

        CmdFlip();
    }

    [Command]
    public void CmdFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        RpcFlip(scale);
    }

    [ClientRpc]
    public void RpcFlip(Vector3 scale)
    {
        transform.localScale = scale;
    }

    private void MoveCamera()
    {
        m_mainCamera.position = transform.position;

        m_mainCamera.Translate(m_cameraOffset);
    }
}