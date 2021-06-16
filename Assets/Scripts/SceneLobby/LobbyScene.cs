using CoreNet.Networking;
using CoreNet.Protocols;
using MmoCore.Packets;
using System;
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
    private Dictionary<MmoCorePacket.PACKET_TYPE, Action<CoreSession, MmoCorePacket>> pTypeAct = new Dictionary<Packet.PACKET_TYPE, Action<CoreSession, MmoCorePacket>>();

    public GameObject StartObj;
    public GameObject ConnObj;

#if TESTING
    bool isTest = true;
#else
    bool isTest = false;
#endif

    public void Start()
    {
        pTypeAct[MmoCorePacket.PACKET_TYPE.ANS] = Dispatch_Ans;
        pTypeAct[MmoCorePacket.PACKET_TYPE.REQ] = Dispatch_Req;
        pTypeAct[MmoCorePacket.PACKET_TYPE.NOTI] = Dispatch_Noti;
        pTypeAct[MmoCorePacket.PACKET_TYPE.TEST] = Dispatch_Test;
    }


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
        
        //exception check?
        pTypeAct[mp.pType](session, mp);
    }

    private void Dispatch_Req(CoreSession _s, MmoCorePacket _mp)
    {
        switch (_mp.cType)
        {
            default:
                break;
        }
    }
    private void Dispatch_Ans(CoreSession _s, MmoCorePacket _mp)
    {
        switch (_mp.cType)
        {
            case MmoCore.Enums.CONTENT_TYPE.WELCOME:
                Debug.Log("recv welcome");
                ConnObj.SetActive(false);
                StartObj.SetActive(true);
                break;
            default:
                break;
        }
    }
    private void Dispatch_Noti(CoreSession _s, MmoCorePacket _mp)
    {
        switch (_mp.cType)
        {
            case MmoCore.Enums.CONTENT_TYPE.CHAT:
                break;
            default:
                break;
        }
    }
    private void Dispatch_Test(CoreSession _s, MmoCorePacket _mp)
    {
        switch (_mp.cType)
        {
            case MmoCore.Enums.CONTENT_TYPE.TEST:
                break;
            default:
                break;
        }
    }
}
