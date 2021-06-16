using CoreNet.Protocols;
using MmoCore.Packets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{

    public InputField ConnToIpTxt;

    public LobbyLog lobbyLog;

    private CancellationTokenSource cst = new CancellationTokenSource();
    private string serverProcessName = "server.exe";
    private string folderName = "Debug";

    private bool isWelcomed = false;

    private Networker lobbyNetworker;
    private AsyncOperation selectSceneOp;
#if TESTING
    bool isTest = true;
#else
    bool isTest = false;
#endif

    private IEnumerator StartConn()
    {
        if (string.IsNullOrWhiteSpace(ConnToIpTxt.text) && isTest == false)
            yield return null;
        string serverIP = ConnToIpTxt.text;
        string testServerIP = "127.0.0.1";
        int serverPort = 30000;

        if (isTest)
            serverIP = testServerIP;
        NetClient.Init(serverIP, serverPort);
        UnityEngine.Debug.Log($"ClickConnectBtn");

        lobbyNetworker = Networker.CreateNetworker("Lobby", () => {
            UnityEngine.Debug.Log("LobbyNetworker shutdowned");
        });

        lobbyNetworker.SetConnToServer(NetClient.Inst.ServerIP, NetClient.Inst.ServerPort);
        lobbyNetworker.ReadyToStart();
        lobbyNetworker.Start();

        lobbyLog.ConnToLobyyComplete();
        StartCoroutine(SendHelloPacket());

    }

    public void ClickConnectBtn()
    {
        StartCoroutine(StartConn());
    }

    // send hello packet to lobbyserver while recv welcome packet.
    private IEnumerator SendHelloPacket()
    {
        float deltaSec = 0.3f;
        while (isWelcomed == false)
        {
            HelloReq hello = new HelloReq();
            hello.SerWrite();
            lobbyNetworker.mSession.OnSendTAP(hello);
            yield return new WaitForSeconds(deltaSec);
            UnityEngine.Debug.Log("hello send");
        }
    }


    private void ChangeToSelectScene()
    {

        string sceneName = "SelectScene";
        SceneManager.LoadScene(sceneName);
        return;
    }


    private IEnumerator AsyncLoadSelectScene()
    {
        string sceneName = "SelectScene";
        selectSceneOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        selectSceneOp.allowSceneActivation = false;
        while (selectSceneOp.isDone == false)
            yield return null;
        UnityEngine.Debug.Log($"Scene Load Complete :{sceneName}");
    }

    public void Update()
    {
        if (lobbyNetworker == null)
            return;
        var pkg = lobbyNetworker.packageQ.pop();
        if (pkg == null)
        {
            lobbyNetworker.packageQ.Swap();
            return;
        }
        PkgDispatcher(pkg);
    }

    private void PkgDispatcher(Package _pkg)
    {
        var packet = _pkg.Packet;
        var session = _pkg.session;

        packet.ReadPacketType();
        MmoCorePacket mp = new MmoCorePacket(packet);
        switch (mp.cType)
        {
            case MmoCore.Enums.CONTENT_TYPE.TEST:
                break;
            case MmoCore.Enums.CONTENT_TYPE.WELCOME:
                {
                    UnityEngine.Debug.Log("recv welcome packet");
                    //todo : scene change
                    isWelcomed = true;
                    ChangeToSelectScene();
                }
                break;
            default:
                break;
        }
    }
}
