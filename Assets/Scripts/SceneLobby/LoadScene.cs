using CoreNet.Protocols;
using MmoCore.Protocols;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CoreNet.Protocols.Packet;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Translate.Init();
        MmoTranslate.Init();

        PACKET_TYPE pt = PACKET_TYPE.ANS;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
