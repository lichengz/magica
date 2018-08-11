using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

//this class should be attached to the root object for chat system since it will manage fade in/out of visibility
public class ChatSystem : NetworkBehaviour
{
    public InputField userNameInputField;

    [SerializeField]
    private SyncListChatMessage chatMessages = new SyncListChatMessage();

    private List<UIChatMessage> messagesOnUI;

    private NetworkClient networkClient;
    //network channel in which to send messages across
    private const short messageChannel = 100;

    [SerializeField]
    private UIChatMessage chatMessagePrefab;
    //probably you will  want to adjust this based on expected chat message load. You could even modify it so we have max_messages per channel (in and MMO setting perhaps?)
    [SerializeField]
    private int MAX_MESSAGES = 5;
    [SerializeField]
    private const float DELAY_BEFORE_HIDING_CHAT = 10f; //in seconds
    [SerializeField]
    private bool openChatOnNewMessageReceived = true;
    //play with this value to adjust fade speed
    private float FADE_SPEED = 5f;
    //sometimes it magically triggers before the expected time, allowing small lieniency value will make it close properly
    private const float LIENIENCY = 0.5f;

    //You will probably have another class to manage these, but for simplicity of the demo's sake I have them here:
    public const int UNSET_TEAM = 0; // unlikely case, failsafe, should be used for "all chat"
                                     //public const int SPECTATOR_INDEX = 1; // the channel index for spectators. perhaps you want to disallow spectators from all chatting if they can see everyone.
                                     //public const int TERRORIST_INDEX = 2; // the channel index for terrorists
                                     //public const int COUNTER_TERRORIST_INDEX = 3; // the channel index for counter-terrorists

    //in version 1.1 we have replaced the above (commented lines) with a more robust way of referencing and creating ChatChannels
    [SerializeField]
    private List<ChatChannel> chatChannels;

    private ContentSizeFitter contentPanel;
    private CanvasGroup canvasGroup;
    private ChatIdentifier chatPanelIdentifier;

    private bool lerpAlphaOfChat;
    private float timeLastChatEntryHappened;

    private float targetAlpha;

    private uint channelToSend;

    //it's magic! not really. This is used to generate hex values for the color specified in the Editor. There's no simple Color.toHex()
    private const string hexValues = "0123456789ABCDEF";

    //private List<PlayerController> cachedPlayers;

    void Start()
    {
        networkClient = NetworkManager.singleton.client;
        chatPanelIdentifier = GameObject.FindObjectOfType<ChatIdentifier>();
        contentPanel = chatPanelIdentifier.GetComponentInChildren<ContentSizeFitter>();
        canvasGroup = GetComponent<CanvasGroup>();
        NetworkServer.RegisterHandler(messageChannel, ReceivedMessage);
        chatMessages.Callback += OnChatMessagesUpdated;
        messagesOnUI = new List<UIChatMessage>();

        ForceCloseChat();
        //uncomment this only if you are trying to maintain local cache of players. Doing this requires a bit of extra work and creating a custom 
        //NetworkManager class. If you do not want to do this, the chat system will still work.
        //cachedPlayers = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>());
    }

    private void OnChatMessagesUpdated(SyncListStruct<ChatEntry>.Operation op, int itemIndex)
    {
        Debug.Log("Operation: " + op.ToString() + " index: " + itemIndex);
        if (SyncListStruct<ChatEntry>.Operation.OP_ADD.ToString().Equals(op.ToString()))
        {
            Debug.Log("updating and creating prefab.");
            //swap the two lines below if you are going to maintain a local cache of players to prevent searching entire scene for them.
            //uint playerTeam = cachedPlayers.Find(player => player.isLocalPlayer).teamIndex;
            uint playerTeam;
            try
            {
                playerTeam = new List<NaokoController>(GameObject.FindObjectsOfType<NaokoController>()).Find(player => player.isLocalPlayer).teamIndex;

            }
            catch
            {
                playerTeam = new List<YuriController>(GameObject.FindObjectsOfType<YuriController>()).Find(player => player.isLocalPlayer).teamIndex;
            }

            //if you are in the wrong channel, do not create text prefab for that message
            if (chatMessages[itemIndex].Channel == UNSET_TEAM || chatMessages[itemIndex].Channel == playerTeam)
            {
                CreatePrefabAndAddToScreen(chatMessages[itemIndex]);
                if (openChatOnNewMessageReceived)
                {
                    OpenChat(false);
                }
                timeLastChatEntryHappened = Time.time;
                Invoke("TryToHideChat", DELAY_BEFORE_HIDING_CHAT);
            }
            //and if you are feeling adventurous, you may think about even removing it from the list.
        }
        //last condition should only happen when user is first connecting to a game in progress, and they have not filled up their queue
        else if (SyncListString.Operation.OP_REMOVEAT.ToString().Equals(op.ToString()) && !isServer && messagesOnUI.Count > MAX_MESSAGES)
        {
            Debug.Log("Destroying message: " + itemIndex);
            Destroy(messagesOnUI[itemIndex]);
            messagesOnUI.RemoveAt(itemIndex);
        }
    }

    private void ReceivedMessage(NetworkMessage message)
    {
        ChatMessage chatMessage = message.ReadMessage<ChatMessage>();

        chatMessages.Add(chatMessage.entry);

        //since we only get 1 message at a time, removing the 0 index = the oldest message. 
        //if you have max messages per channel...requires a tad of work on your end :(...  
        //you should filter chatMessages per channel and see if any exceed the limit, and remove the oldest from that channel.
        if (chatMessages.Count > MAX_MESSAGES)
        {
            chatMessages.RemoveAt(0);
            Destroy(messagesOnUI[0]);
            messagesOnUI.RemoveAt(0);
        }
    }

    //Uncomment this only if you are going to maintain a local cache of players and are using a custom NetworkManager. See below for more information
    //[Command]
    //public void CmdOnPlayerConnectOrDisconnect()
    //{
    //    cachedPlayers = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>());
    //}

    /* If you want to maintain the player list within this ChatSystem (which is much better in terms of performance)
     * you will need to create a new NetworkManager class, for example:
      public class DefaultNetworkManager : NetworkManager
      {
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            //we want to do the normal stuff first so our player should be initialized
            base.OnServerAddPlayer(conn, playerControllerId);
            chatSystem.CmdOnPlayerConnectOrDisconnect();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            //we do want to do our stuff before the standard things so we still have the player object
            chatSystem.CmdOnPlayerConnectOrDisconnect();
            base.OnClientDisconnect(conn);
        }
     }

     * Then in the editor, set this script as the NetworkManager script.
     * After you have done that, uncomment the lines relating the cachedPlayers in this script
     * and it should be working.
     */

    private void CreatePrefabAndAddToScreen(ChatEntry message)
    {
        UIChatMessage newMessage = Instantiate(chatMessagePrefab);
        newMessage.GetComponent<RectTransform>().SetParent(contentPanel.GetComponent<RectTransform>(), false);

        newMessage.MessageText.text = "<color=\"" + GetHexValueForColor(chatChannels.Find(channel => channel.Channel == message.Channel).color) + "\">" + "(" + chatChannels.Find(channel => channel.Channel == message.Channel).Name + ") " + message.SenderName + ":</color> ";
        newMessage.MessageText.color = Color.white;
        newMessage.MessageText.text += message.Message;
        messagesOnUI.Add(newMessage);

        //this will try to hide the chat after 10 seconds. If a new message comes in, the timeLastChatEntryHappened will be updated so still we should have DELAY_BEFORE_HIDING_CHAT seconds before it hides
        Debug.Log("Going to invoke TryToHideChat in " + DELAY_BEFORE_HIDING_CHAT + " seconds. @(" + (Time.time + DELAY_BEFORE_HIDING_CHAT) + ")");
        Invoke("TryToHideChat", DELAY_BEFORE_HIDING_CHAT);
        timeLastChatEntryHappened = Time.time;

        //frequently the last message is not properly scrolled into view due to some internals of Unity UI, putting a brief delay ensures proper scrolling
        Invoke("ScrollToBottom", 0.15f);
    }

    private void ScrollToBottom()
    {
        chatPanelIdentifier.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    //some magic to convert the rgb to hex value we can use in RichText (eliminates need for secondary text element for player name)
    private string GetHexValueForColor(Color color)
    {
        string hexValue = "#";
        float redFloat = color.r * 255f;
        float greenFloat = color.g * 255f;
        float blueFloat = color.b * 255f;

        hexValue += GetHex(Mathf.FloorToInt(redFloat / 16)) + GetHex(Mathf.FloorToInt(redFloat) % 16) + GetHex(Mathf.FloorToInt(greenFloat / 16)) + GetHex(Mathf.FloorToInt(greenFloat) % 16) + GetHex(Mathf.FloorToInt(blueFloat / 16)) + GetHex(Mathf.FloorToInt(blueFloat) % 16);

        return hexValue;
    }

    //helper for the above
    private string GetHex(int value)
    {
        return hexValues[value].ToString();
    }

    public void UpdateChatMessages()
    {
        //again this is a performance improvement, see above commented section for usage
        //PlayerController playerController = cachedPlayers.Find(playerControllerId => player.isLocalPlayer);
        PlayerHP playerController = new List<PlayerHP>(GameObject.FindObjectsOfType<PlayerHP>()).Find(player => player.isLocalPlayer);
        //perhaps send playerController to ReactivatePlayerAndDeselectInputField() so you don't have to do the above search again.
        ReactivatePlayerAndDeselectInputField();
        if (chatPanelIdentifier.InputField.text != "")
        {
            string localPlayerName = "Player";
            if (playerController != null)
            {
                localPlayerName = playerController.name;
            }
            if (userNameInputField.text != "")
            {
                localPlayerName = userNameInputField.text;
            }

            ChatEntry entryToSend = new ChatEntry();

            entryToSend.Channel = channelToSend;
            entryToSend.Message = chatPanelIdentifier.InputField.text;
            entryToSend.SenderName = localPlayerName;

            networkClient.Send(messageChannel, new ChatMessage(entryToSend));

            chatPanelIdentifier.InputField.text = "";
        }

        //this will try to hide the chat after DELAY_BEFORE_HIDING_CHAT seconds. If a new message comes in, the timeLastChatEntryHappened will be updated so still we should have DELAY_BEFORE_HIDING_CHAT seconds before it hides
        Invoke("TryToHideChat", DELAY_BEFORE_HIDING_CHAT);
        timeLastChatEntryHappened = Time.time;

        // resume Player movement
        try
        {
            if (playerController.GetComponent<YuriController>() != null)
            {
                playerController.GetComponent<YuriController>().movementDisable = false;
            }
            if (playerController.GetComponent<NaokoController>() != null)
            {
                playerController.GetComponent<NaokoController>().movementDisable = false;
            }
        }
        catch
        {

        }
    }

    private void ReactivatePlayerAndDeselectInputField()
    {
        chatPanelIdentifier.InputField.DeactivateInputField();
        //perhaps you would like to re-activate player's ability to control here. Should be the inverse of what OpenChat() does
    }

    private void TryToHideChat()
    {
        if (Time.time >= ((timeLastChatEntryHappened + DELAY_BEFORE_HIDING_CHAT) - LIENIENCY) && !chatPanelIdentifier.InputField.isFocused)
        {
            ForceCloseChat();
        }
    }

    public void ForceCloseChat()
    {
        lerpAlphaOfChat = true;
        targetAlpha = 0;
        EventSystem.current.SetSelectedGameObject(null);

        ReactivatePlayerAndDeselectInputField();
    }


    //legacy way to open chat. By specifying a channel (see OpenChat(bool, int)) the user is greeted with a message indicating where the message will be sent.
    public void OpenChat(bool focusInputField)
    {
        chatPanelIdentifier.InputField.placeholder.GetComponent<Text>().text = "Enter message...";
        lerpAlphaOfChat = true;
        targetAlpha = 1;
        if (focusInputField)
        {
            chatPanelIdentifier.InputField.ActivateInputField();
            chatPanelIdentifier.InputField.Select();
        }

        //perhaps disable your player's ability to move (keyboard input)?
    }

    //This is now the preferred way to open the chat. Specify the channel (a valid one) and we will notify the user which channel name their message will go to.
    public void OpenChat(bool focusInputField, uint channel)
    {
        //note that with 5.3 there is some undesirable behavior here. The message is updated, but placeholders disappear before user starts typing.
        //This was fixed in 5.4. However the chat system is still working fine on 5.3 so I didn't want to force a version upgrade on anyone currently on 5.3. 
        //This feature is quirky until you upgrade to 5.4.
        chatPanelIdentifier.InputField.placeholder.GetComponent<Text>().text = "Enter message (" + chatChannels.Find(chatChannel => chatChannel.Channel == channel).Name + ")...";
        chatPanelIdentifier.InputField.ForceLabelUpdate();

        channelToSend = channel;

        lerpAlphaOfChat = true;
        targetAlpha = 1;
        if (focusInputField)
        {
            chatPanelIdentifier.InputField.ActivateInputField();
            chatPanelIdentifier.InputField.Select();
            //perhaps disable your player's ability to move (keyboard input)?
        }
    }

    private void Update()
    {
        if (lerpAlphaOfChat)
        {
            //for instantaneous visibility, you can just set canvasGroup.alpha = targetAlpha
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * FADE_SPEED);

            if (canvasGroup.alpha < 0.01f || canvasGroup.alpha > 0.99f)
            {
                lerpAlphaOfChat = false;
            }
        }

        //Disable movement when inputting
        if (chatPanelIdentifier.InputField.isFocused)
        {
            PlayerHP playerController = new List<PlayerHP>(GameObject.FindObjectsOfType<PlayerHP>()).Find(player => player.isLocalPlayer);
            try
            {
                if (playerController.GetComponent<YuriController>() != null)
                {
                    playerController.GetComponent<YuriController>().movementDisable = true;
                }
                if (playerController.GetComponent<NaokoController>() != null)
                {
                    playerController.GetComponent<NaokoController>().movementDisable = true;
                }
            }
            catch
            {

            }
        }
    }


    //allows easier editor modifications of Chat Channels
    [Serializable]
    public struct ChatChannel
    {
        public string Name;
        public Color color;
        public uint Channel;
    }

    //if you for some reason need more data transferred, you will probably need to do some reading, maybe here is a good starting point: http://docs.unity3d.com/Manual/UNetStateSync.html
    /*
        note that you should probably limit both:
            message length (capped to 150 in demo, by InputField) and 
            sender name
        or you will end up sending a ton of data at once if someone sends a really long message, or sets a really long name
    */
    private struct ChatEntry
    {
        public string Message;
        public uint Channel;
        public string SenderName;
    }

    //unless you know what you are doing, I would avoid touching this
    private class ChatMessage : MessageBase
    {
        public ChatEntry entry;

        public ChatMessage(ChatEntry entry)
        {
            this.entry = entry;
        }

        public ChatMessage()
        {
            entry = new ChatEntry();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32(entry.Channel);
            writer.Write(entry.Message);
            writer.Write(entry.SenderName);
        }

        public override void Deserialize(NetworkReader reader)
        {
            entry.Channel = reader.ReadPackedUInt32();
            entry.Message = reader.ReadString();
            entry.SenderName = reader.ReadString();
        }

    }

    //you can't directly just do SyncListStruct<YourClass>
    private class SyncListChatMessage : SyncListStruct<ChatEntry> { }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ChatSystem.ChatChannel))]
public class ChatChannelEditor : Editor
{
    SerializedProperty Name;
    SerializedProperty color;
    SerializedProperty Channel;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        Name = serializedObject.FindProperty("Name");
        color = serializedObject.FindProperty("color");
        Channel = serializedObject.FindProperty("Channel");
    }

    void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        this.name = Name.ToString();
        Rect channelRect = new Rect(position.x, position.y, position.width - 90, position.height);
        Rect nameRect = new Rect(position.x + 35, position.y, 30, position.height);
        Rect colorRect = new Rect(position.x + 90, position.y, 50, position.height);

        EditorGUI.PropertyField(channelRect, Channel, GUIContent.none);
        EditorGUI.PropertyField(nameRect, Name, GUIContent.none);
        EditorGUI.PropertyField(colorRect, color, GUIContent.none);

        EditorGUI.EndProperty();
    }
}

#endif
