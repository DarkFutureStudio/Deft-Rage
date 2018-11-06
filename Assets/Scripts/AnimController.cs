using UnityEngine;
using UnityEngine.Networking;

public class AnimController : NetworkBehaviour {

    private Animator m_anim;                                     //Animator component of the player
    private float x;                                            //This variable should assign to the input float of the player ( Example: float x = input.getaxis )
    private Rigidbody2D m_rigidbody;                            //Rigidbody component of the player
    private PlayerController pc;

    void Start()
    {
        if (isLocalPlayer)
        {
            m_rigidbody = GetComponent<Rigidbody2D>();
            m_anim = GetComponent<Animator>();
            pc = this.gameObject.GetComponent<PlayerController>();
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        m_anim.SetBool("Ground", pc.grounded);

        Move(pc.x, m_rigidbody.velocity.y);                                 //Do the settings in the fixedUpdate for smoothing

    }

    private void Move(float x, float velocity)               //Sets the value of the parameters in the animator
    {
        m_anim.SetFloat("Speed", Mathf.Abs(x));
        m_anim.SetFloat("vSpeed", velocity);
    }

    public void UppercutAnimation()
    {
        Cmd_Trigger("Hit");                                         //This function should call when the uppercut move done
    }

    public void HookAnimation()
    {
        Cmd_Trigger("Punch");                                           //This function should call when the hook move done
    }

    [Command]
    void Cmd_Trigger(string param)
    {
        Rpc_Trigger(param);                                         //Request the server politely for telling other clients about the state of animator
    }

    [ClientRpc]
    void Rpc_Trigger(string param)
    {
        Animator anim = this.gameObject.GetComponent<Animator>();                   //Telling other clients about the state of animator
        anim.SetTrigger(param);
    }
}
