using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetConnObj : MonoBehaviour
{
    public string connName;

    public Networker netWorker { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrWhiteSpace(connName))
            connName = "default";
        netWorker = Networker.CreateNetworker(connName);
        if (netWorker == default(Networker))
            netWorker = Networker.nameToDic[connName];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
