using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager
{
    public int chosenCharacter = 0;
    public GameObject[] characters;
    public Transform[] startPos;
    //subclass for sending network messages
    public class NetworkMessage : MessageBase
    {
        public int chosenClass;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();
        int selectedClass = message.chosenClass;

        GameObject player;

        if (startPos != null)
        {
            player = Instantiate(characters[selectedClass], startPos[selectedClass].position, startPos[selectedClass].rotation) as GameObject;
        }
        else
        {
            player = Instantiate(characters[selectedClass], Vector3.zero, Quaternion.identity) as GameObject;
        }

        // Assign tag to each player
        int tmp = selectedClass + 1;
        player.tag = "Player" + tmp.ToString();
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        NetworkMessage test = new NetworkMessage();
        test.chosenClass = chosenCharacter;
        ClientScene.AddPlayer(conn, 0, test);
    }


    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //base.OnClientSceneChanged(conn);
    }

    public void btn1()
    {
        chosenCharacter = 0;
    }

    public void btn2()
    {
        chosenCharacter = 1;
    }

    //Customized UNET UI
    public void StartUpHost()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
        GameObject.Find("WelcomePanel").SetActive(false);
    }
    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        NetworkManager.singleton.StartClient();
        GameObject.Find("WelcomePanel").SetActive(false);
    }
    void SetIPAddress()
    {
        string ip = GameObject.Find("HostIP").transform.Find("Text").GetComponent<Text>().text;
        if (ip != "")
        {
            NetworkManager.singleton.networkAddress = ip;
        }
        else
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }
    }
    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }
}