using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementInRoom : MonoBehaviour
{
    [SerializeField] private Button ToLeftBtn;
    [SerializeField] private Button ToRightBtn;

    [SerializeField] private Canvas Left;
    [SerializeField] private Canvas Middle;
    [SerializeField] private Canvas Right;

    private List<Canvas> roomParts = new List<Canvas>();
    private int activeRoomPart;

    private void Start()
    {
        roomParts.Add(Left);
        roomParts.Add(Middle);
        roomParts.Add(Right);

        activeRoomPart = 1;
        SetVisibleCanvas();

        ToLeftBtn.onClick.AddListener(OnLeftButtonClick);
        ToRightBtn.onClick.AddListener(OnRightButtonClick);
    }

    private void OnLeftButtonClick()
    {
        if (activeRoomPart > 0)
            activeRoomPart--;
        SetVisibleCanvas();
    }

    private void OnRightButtonClick()
    {
        if (activeRoomPart < roomParts.Count - 1)
            activeRoomPart++;
        SetVisibleCanvas();
    }

    private void SetVisibleCanvas()
    {
        for (int i = 0; i < roomParts.Count; i++)
            roomParts[i].gameObject.SetActive(i == activeRoomPart);

        if (activeRoomPart == 0) ToLeftBtn.gameObject.SetActive(false);
        else ToLeftBtn.gameObject.SetActive(true);

        if (activeRoomPart == roomParts.Count - 1) ToRightBtn.gameObject.SetActive(false);
        else ToRightBtn.gameObject.SetActive(true);
    }
}
