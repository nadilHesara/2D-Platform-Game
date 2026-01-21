using UnityEngine;
using UnityEngine.EventSystems;

public class UI_JumpButton : MonoBehaviour, IPointerDownHandler
{
    private Player player;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player == null || !player.isActiveAndEnabled) return;
        player.JumpButton();
    }

    public void UpdatePlayerRef(Player newplayer)=> player = newplayer;
}
