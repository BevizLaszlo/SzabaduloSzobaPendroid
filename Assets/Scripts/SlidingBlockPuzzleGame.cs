using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlidingBlockPuzzleGame : MonoBehaviour
{
    [SerializeField] private Canvas GridPlayground;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;
    [SerializeField] private Button button5;
    [SerializeField] private Button button6;
    [SerializeField] private Button button7;
    [SerializeField] private Button button8;
    [SerializeField] private Button buttonEmpty;
    [SerializeField] private Button ShuffleButton;
    [SerializeField] private TextMeshProUGUI youWonText;

    List<Button> buttons;

    private void Start()
    {
        buttons = new List<Button> { button1, button2, button3, button4, button5, button6, button7, button8, buttonEmpty };
        AddButtonListeners();
        Shuffle();
        ShuffleButton.onClick.AddListener(() =>
        {
            if (!GameData.Lv3Solved)
            {
                Shuffle();
                GameData.sanity--;
            }
        });
    }

    public void AddButtonListeners()
    {
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => MoveButton(btn));
        }
    }

    private void Shuffle()
    {
        do
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                int randomIndex = Random.Range(0, buttons.Count);
                Button temp = buttons[randomIndex];
                buttons[randomIndex] = buttons[i];
                buttons[i] = temp;
            }
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].transform.SetSiblingIndex(i);
            }
        }
        while (!IsSolveable());
    }

    private bool IsSolveable()
    {
        List<int> buttonIndices = new List<int>();

        foreach (Button btn in buttons)
        {
            if (btn != buttonEmpty)
            {
                buttonIndices.Add(int.Parse(btn.name));
            }
        }

        int inversions = 0;
        for (int i = 0; i < buttonIndices.Count; i++)
        {
            for (int j = i + 1; j < buttonIndices.Count; j++)
            {
                if (buttonIndices[i] > buttonIndices[j])
                {
                    inversions++;
                }
            }
        }

        return inversions % 2 == 0;
    }




    private void MoveButton(Button btn)
    {
        if(!GameData.Lv3Solved)
        {
            int btnIndex = btn.transform.GetSiblingIndex();
            int emptyIndex = buttonEmpty.transform.GetSiblingIndex();

            if (btnIndex + 1 == emptyIndex && emptyIndex % 3 != 0 ||
                btnIndex - 1 == emptyIndex && btnIndex % 3 != 0 ||
                btnIndex + 3 == emptyIndex ||
                btnIndex - 3 == emptyIndex)
            {
                btn.transform.SetSiblingIndex(emptyIndex);
                buttonEmpty.transform.SetSiblingIndex(btnIndex);
            }

            if (IsSolved())
            {
                youWonText.text = "Szeretnék Gratulálni! Az akadályokat állta, a teszt végeredménye: Megfelel a beilleszkedésre! Az ajtót kinyitom!";
                GameData.Lv3Solved = true;
            }
        }
    }

    private bool IsSolved()
    {
        List<Button> btnsInOrder = new List<Button> { button1, button2, button3, button4, button5, button6, button7, button8, buttonEmpty };

        for (int i = 0; i < btnsInOrder.Count; i++)
        {
            if (btnsInOrder[i].transform.GetSiblingIndex() != i) return false;
        }
        return true;
    }


}
