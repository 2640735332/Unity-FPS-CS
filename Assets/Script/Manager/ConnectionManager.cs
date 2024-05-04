using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectManager
{
    private static ConnectManager instance;
    public static ConnectManager Instance => instance;

    public ConnectManager()
    {
        if (instance == null)
            instance = this;
    }
    
    public void OnCennectionApprovalCallback(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        var clientID = req.ClientNetworkId;
        var payload = req.Payload;

        res.Approved = true;
        res.Position = Vector3.zero;
        res.Rotation = Quaternion.identity;
        res.Reason = "";
        res.PlayerPrefabHash = null;
        res.Pending = false;
        Debug.Log($"[ConnectionManager] OnCennectionApprovalCallback: clientID={clientID}, approved={res.Approved}, reason={res.Reason}");
    }

    public void OnConnectEvent(NetworkManager networkMgr, ConnectionEventData eventData)
    {
        Debug.Log($"[ConnectionManager] OnConnectEvent: eventData={eventData}");
    }
}
