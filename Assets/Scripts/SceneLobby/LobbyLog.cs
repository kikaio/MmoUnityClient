using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLog : MonoBehaviour
{
    private enum LobbyState
    {
        NONE,
        READY_LOBBY_START,
        STARTING_LOBBY_SERVER,
        STARTED_LOBBY_SERVER,
        TRY_CONN_TO_LOBBY,
        CONN_TO_LOBBY,
        START_SELECT_SCENE,
        EXCEPT,
        END,
    }

    private Dictionary<LobbyState, string> stateToStr = new Dictionary<LobbyState, string>();


    private LobbyState preState = LobbyState.NONE;
    private LobbyState lobbyState = LobbyState.NONE;
    public InputField logInput;

    // Start is called before the first frame update
    // Update is called once per frame

    public void Start()
    {
        stateToStr[LobbyState.READY_LOBBY_START] = "Wait Start Server or Conn to other";
        stateToStr[LobbyState.STARTING_LOBBY_SERVER] = "now, starting lobbyserver to localhost";
        stateToStr[LobbyState.STARTED_LOBBY_SERVER] = "lobby server process run complete";
        stateToStr[LobbyState.TRY_CONN_TO_LOBBY] = "Try connect to lobbyserver";
        stateToStr[LobbyState.CONN_TO_LOBBY] = "Connect complete, press statrt or waiting";
        stateToStr[LobbyState.START_SELECT_SCENE] = "Scene change to selectScene";
        stateToStr[LobbyState.EXCEPT] = "Something throw except";

        SwitchState(LobbyState.READY_LOBBY_START);
    }

    public void Update()
    {
        if (preState == lobbyState)
            return;
        logInput.text = stateToStr[lobbyState];
        preState = lobbyState;
    }

    private void SetLoggingMsg(string _msg)
    {
        logInput.text = _msg;
    }

    private void SwitchState(LobbyState _nextState)
    {
        if (_nextState == lobbyState)
            return;
        preState = lobbyState;
        lobbyState = _nextState;
    }

    public void TryStartLobbyServer()
    {
        SwitchState(LobbyState.STARTING_LOBBY_SERVER);
    }

    public void ServerStartComplete()
    {
        SwitchState(LobbyState.STARTED_LOBBY_SERVER);
    }
    public void TryConnToLobbyServer()
    {
        SwitchState(LobbyState.TRY_CONN_TO_LOBBY);
    }

    public void ConnToLobyyComplete()
    {
        SwitchState(LobbyState.CONN_TO_LOBBY);
    }
}
