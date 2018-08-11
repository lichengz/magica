using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private ChatSystem chatSystem;
    private ChatIdentifier chatIdentifier;
    private TeamIdentifier teamIdentifier;

    private InputField nameSetter;

    //you can play with this in the editor to see the different channels by bringing up "team chat" by pressing "U"
    [SerializeField]
    public uint teamIndex;

    private const uint SPECTATOR_INDEX = 1; // the channel index for spectators. perhaps you want to disallow spectators from all chatting if they can see everyone.
    private const uint COUNTER_TERRORIST_INDEX = 2; // the channel index for counter-terrorists
    private const uint TERRORIST_INDEX = 3; // the channel index for terrorists

    void Start()
    {
        if (!isLocalPlayer)
        {
            this.enabled = false;
        }
        chatSystem = GameObject.FindObjectOfType<ChatSystem>();
        chatIdentifier = GameObject.FindObjectOfType<ChatIdentifier>();
        teamIdentifier = GameObject.FindObjectOfType<TeamIdentifier>();
        nameSetter = GameObject.Find("Player Name Setter").GetComponent<InputField>();
        nameSetter.onEndEdit.AddListener(value =>
        {
            name = value;
        });
    }

    void Update()
    {
        // we don't want to do extra stuff like refocus chat if we are typing a message. But it's weird if they have to close chat and reopen it 
        // if they sent a message already and it hasn't re-hidden yet.
        if (Input.GetKeyUp(KeyCode.Y) && (!ChatSystemIsOpen() || chatIdentifier.InputField.text == ""))
        {
            chatSystem.OpenChat(true, 0);
        }
        else if (Input.GetKeyUp(KeyCode.U) && (!ChatSystemIsOpen() || chatIdentifier.InputField.text == ""))
        {
            chatSystem.OpenChat(true, teamIndex);
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            chatSystem.ForceCloseChat();
        }

        if (isLocalPlayer)
        {
            switch (teamIndex)
            {
                case ChatSystem.UNSET_TEAM:
                    teamIdentifier.textComponent.text = "No Team";
                    break;
                case COUNTER_TERRORIST_INDEX:
                    teamIdentifier.textComponent.text = "CT";
                    break;
                case TERRORIST_INDEX:
                    teamIdentifier.textComponent.text = "Terrorist";
                    break;
                case SPECTATOR_INDEX:
                    teamIdentifier.textComponent.text = "Spectator";
                    break;
                default:
                    teamIdentifier.textComponent.text = "No Team";
                    break;
            }
        }
    }

    private bool ChatSystemIsOpen()
    {
        return chatSystem.GetComponent<CanvasGroup>().alpha > 0.01f;
    }
}
