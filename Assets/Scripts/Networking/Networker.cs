using CoreNet.Jobs;
using CoreNet.Networking;
using CoreNet.Protocols;
using CoreNet.Sockets;
using MmoCore.Enums;
using MmoCore.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

//one connection 
public class Networker : CoreNetwork, IDisposable
{
    //ex : lobby connect, battle connect etc....
    public static Dictionary<string, Networker> nameToDic { get; private set; } = new Dictionary<string, Networker>();
    public static Networker CreateNetworker(string _name, Action _shutDownAct = null)
    {
        if (nameToDic.ContainsKey(_name))
        {
            //todo : logging
            return default(Networker);
        }
        var newWorker = new Networker();
        nameToDic[_name] = newWorker;
        newWorker.shutdownAct = _shutDownAct;
        return newWorker;
    }
    private bool disposed = false;

    protected virtual void Dispose(bool _isDisposing)
    {
        //close session
        if (_isDisposing)
        {
        }
    }

    public void Dispose()
    {
        if (disposed)
            return;
        Dispose(true);
        disposed = true;
    }

    private Dictionary<string, Worker> nameToWorker = new Dictionary<string, Worker>();
    private CancellationTokenSource cts = new CancellationTokenSource();
    public CoreSession mSession { get; private set; }

    private string ipStr;
    private int port;

    private Networker()
    {

    }

    private void ReadyWorkers()
    {
        nameToWorker["recv"] = new Worker("recv");
        nameToWorker["recv"].PushJob(new JobOnce(DateTime.UtcNow, async () => {
            while (cts.IsCancellationRequested == false)
            {
                var packet = await mSession.OnRecvTAP();
                packageQ.Push(new Package(mSession, packet));
            }
        }));

        long hbTickDelta = TimeSpan.FromMilliseconds(CoreSession.hbDelayMilliSec).Ticks;

        nameToWorker["hb"] = new Worker("hb");
        nameToWorker["hb"].PushJob(new JobNormal(DateTime.UtcNow, DateTime.MaxValue, hbTickDelta, () => {
            if (mSession.Sock.Sock.Connected == false)
                return;
            var hbPacket = new HBNoti();
            hbPacket.SerWrite();
            mSession.OnSendTAP(hbPacket);
        }));
    }

    private void ReadyTranslate()
    {
        MmoCore.Protocols.MmoTranslate.Init();
    }

    public void SetConnToServer(string _Ip, int _port)
    {
        ipStr = _Ip;
        port = _port;
    }

    public override void ReadyToStart()
    {
        ReadyTranslate();
        ReadyWorkers();
    }

    public void StartConnect(Action _cb)
    {
        Start();
        _cb?.Invoke();
    }

    public override void Start()
    {
        try
        {
            CoreTCP tcpSock = new CoreTCP();
            var ep = new IPEndPoint(IPAddress.Parse(ipStr), port);
            tcpSock.Sock.Connect(ep);
            mSession = new CoreSession(-1, tcpSock);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Exception : Connect socket");
            throw e;
        }

        foreach (var w in nameToWorker)
        {
            UnityEngine.Debug.Log($"{w.Key} worker is start");
            w.Value.WorkStart();
        }
    }

    protected override void Analizer_Ans(CoreSession _s, Packet _p)
    {
        throw new NotImplementedException("");
    }

    protected override void Analizer_Noti(CoreSession _s, Packet _p)
    {
        throw new NotImplementedException("");
    }

    protected override void Analizer_Req(CoreSession _s, Packet _p)
    {
        throw new NotImplementedException("");
    }

    protected override void Analizer_Test(CoreSession _s, Packet _p)
    {
        throw new NotImplementedException("");
    }
}
