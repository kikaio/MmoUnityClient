using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
//for p2p?
public class NetClient
{
    public static NetClient Inst { get; private set; }

    public static void Init(string _ip, int _port)
    {
        Inst = new NetClient();
        Inst.ServerIP = _ip;
        Inst.ServerPort = _port;
    }

    //todo : CoreNet 참조 추가할 것.

    public string ServerIP { get; private set; }
    public int ServerPort { get; private set; }

    private NetClient()
    {
    }

}
