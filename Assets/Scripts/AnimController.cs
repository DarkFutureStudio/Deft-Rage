using UnityEngine;
using UnityEngine.Networking;

public class AnimController : NetworkBehaviour {

    //Animator component of the player
    private Animator m_anim;                                     
    //This variable should assign to the input float of the player ( Example: float x = input.getaxis )
    private float x;                                         
    //Rigidbody component of the player
    private Rigidbody2D m_rigidbody;                         
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
    
    //Do the settings in the fixedUpdate for smoothing
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        m_anim.SetBool("Ground", pc.grounded);

        Move(pc.x, m_rigidbody.velocity.y);                 

    }

    //Sets the value of the parameters in the animator
    private void Move(float x, float velocity)              
    {
        m_anim.SetFloat("Speed", Mathf.Abs(x));
        m_anim.SetFloat("vSpeed", velocity);
    }

    //This function should call when the uppercut move done
    public void UppercutAnimation()
    {
        Cmd_Trigger("Hit");                                       
    }

    //This function should call when the hook move done
    public void HookAnimation()
    {
        Cmd_Trigger("Punch");                                     
    }

    //Request the server politely for telling other clients about the state of animator
    [Command]
    void Cmd_Trigger(string param)
    {
        Rpc_Trigger(param);                                       
    }

    //Telling other clients about the state of animator
    [ClientRpc]
    void Rpc_Trigger(string param)
    {
        Animator anim = this.gameObject.GetComponent<Animator>(); 
        anim.SetTrigger(param);
    }
}
