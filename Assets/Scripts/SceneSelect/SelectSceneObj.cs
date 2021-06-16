using CoreNet.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSceneObj : MonoBehaviour
{
    static readonly int Character_max = 4;
    public List<GameObject> CharacterList;

    private string sceneName = "SelectScene";
    private int curPlayerCnt = 0;

    //todo : server 한테 input 받기.
    private int[] cArr = new int[Character_max];

    private Networker networker;
    private CoreSession lobbySession;

    public SelectSceneObj()
        : this(2)
    {
    }

    public SelectSceneObj(int _playerCnt = 1)
    {
        curPlayerCnt = _playerCnt;
    }

    public void Start()
    {
        for (int idx = 0, no = 1; idx < curPlayerCnt; ++idx, ++no)
        {
            SetPlayerCharacter(idx, no);
        }
        if (Networker.nameToDic.TryGetValue(sceneName, out networker) == false)
        {
            UnityEngine.Debug.Log("lobbyNetworker is not set");
        }
    }

    private int GetPlayerCharacterIdx(int _pNo)
    {
        for (int idx = 0; idx < Character_max; ++idx)
        {
            if (cArr[idx] == _pNo)
                return idx;
        }
        return 0;
    }

    private void SetPlayerCharacter(int _idx, int _pNo)
    {
        cArr[_idx] = _pNo;
        var character = CharacterList[_idx].GetComponent<SelectCharacter>();
        character.SelectPlayer(_pNo);
    }

    public void SelectLeft(int _pNo)
    {
        int curIdx = GetPlayerCharacterIdx(_pNo);
        SetPlayerCharacter(curIdx, 0);
        curIdx--;
        if (curIdx < 0)
            curIdx = Character_max - 1;
        while (cArr[curIdx] != 0)
            curIdx--;
        SetPlayerCharacter(curIdx, _pNo);
    }

    public void SelectRight(int _pNo)
    {
        int curIdx = GetPlayerCharacterIdx(_pNo);
        SetPlayerCharacter(curIdx, 0);
        curIdx++;
        if (curIdx >= Character_max)
            curIdx = 0;
        while (cArr[curIdx] != 0)
            curIdx++;
        SetPlayerCharacter(curIdx, _pNo);
    }

}
