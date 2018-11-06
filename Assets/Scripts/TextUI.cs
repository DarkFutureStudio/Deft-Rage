using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    public Text MyText;
    public Text player1;
    public Text player2;
    public Text timer;

    public static TextUI instance;

    private void Awake()
    {
        instance = this;
    }
}
