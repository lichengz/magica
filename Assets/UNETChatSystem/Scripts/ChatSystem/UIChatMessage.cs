using UnityEngine;
using UnityEngine.UI;

/*  keep the appropriate elements of a chat message in order
    this can get complex depending on how you want to structure your messages.
    for the purposes this was originally developed for, this is sufficient. 
    You can change the color (as done in ChatSystem.cs) and text of each message. 
    When you first get this, MessageText will always be white, and PlayerNameText will vary depending on the channel it was sent to.
*/
public class UIChatMessage : MonoBehaviour
{
    [SerializeField]
    public Text PlayerNameText;
    [SerializeField]
    public Text MessageText;
}
