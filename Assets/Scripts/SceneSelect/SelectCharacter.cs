using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacter : MonoBehaviour
{
    public Sprite originFace;
    public Sprite ActivateFace;

    public Sprite confirmFace;


    public Sprite selectP1;
    public Sprite selectP2;

    public GameObject faceObj;
    public GameObject selectorObj;

    enum ESELECT_TYPE
    {
        NONE,
        P1,
        P2,
    }

    ESELECT_TYPE curSelectType = ESELECT_TYPE.NONE;

    public bool IsConfirmed { get; private set; } = false;

    public void Awake()
    {
        SelectPlayer(0);
    }

    public void SelectPlayer(int _pNo)
    {
        var nextState = ESELECT_TYPE.NONE;
        if (_pNo == 1)
            nextState = ESELECT_TYPE.P1;
        else if (_pNo == 2)
            nextState = ESELECT_TYPE.P2;
        else
            nextState = ESELECT_TYPE.NONE;

        selectorObj.SetActive(false);

        switch (nextState)
        {

            case ESELECT_TYPE.NONE:
                {
                    faceObj.GetComponent<SpriteRenderer>().sprite = originFace;
                }
                break;
            case ESELECT_TYPE.P1:
                {
                    faceObj.GetComponent<SpriteRenderer>().sprite = ActivateFace;
                    selectorObj.GetComponent<SpriteRenderer>().sprite = selectP1;
                    selectorObj.SetActive(true);
                }
                break;
            case ESELECT_TYPE.P2:
                {
                    faceObj.GetComponent<SpriteRenderer>().sprite = ActivateFace;
                    selectorObj.GetComponent<SpriteRenderer>().sprite = selectP2;
                    selectorObj.SetActive(true);
                }
                break;
            default:
                break;
        }
    }

    public void ConfirmCharacter(int _pid)
    {
        faceObj.GetComponent<SpriteRenderer>().sprite = confirmFace;
        IsConfirmed = true;
    }
}
