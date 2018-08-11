using UnityEngine;
using UnityEngine.UI;

//this class is for demo purposes, it's just for the demo
public class TeamIdentifier : MonoBehaviour
{
    public Text textComponent;

    void Start()
    {
        textComponent = GetComponent<Text>();
    }
}
