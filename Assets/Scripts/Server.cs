using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Server : NetworkBehaviour 
{

    private float speed = 0.04f;
    private static float startTime = 300f;

    private float timeLeft;

    private Text timer;

    public static Server instance;
    public bool gameOver;

    void OnEnable()
    {
        instance = this;
    }

    void Start()
    {
        timer = TextUI.instance.timer;
        timeLeft = startTime;
        timer.text = "5 : 00";
        gameOver = false;
    }

    void Update()
    {
        if (!isServer)
            return;

        if (NetworkServer.connections.Count == 2)
        {
            if (timeLeft > 0f)
            {
                transform.Translate(Vector2.up * speed * Time.deltaTime);
                timeLeft -= Time.deltaTime;
                Rpc_showTimer(timeLeft);
            }
            else
            {
                timer.text = "00:00";
                gameOver = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameObject obj = col.gameObject;
            Cmd_Die(obj);
        }
    }

    [ClientRpc]
    private void Rpc_showTimer(float timeLeft)
    {
        int minute = Mathf.FloorToInt(timeLeft / 60);
        string seconds = (timeLeft % 60).ToString("00");

        timer.text = minute + " : " + seconds;
    }
        
    [Command]
    private void Cmd_Die(GameObject obj)
    {
        obj.GetComponent<Stamina>().CurrentStamina = 10.0f;
        obj.GetComponent<Stamina>().Rpc_OnDeath();
    }
        
}
