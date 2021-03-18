using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    /// <summary>Lets the server know that the welcome message was received.</summary>
    public static void SendWelcomeBack()
    {
        using (Packet _packet = new Packet((int)ClientCommand.SendWelcomeBack))
        {
            _packet.Write("Hello from Player 1");
            SendTCPData(_packet);
        }
    }

    public static void NotifyServerStateChange(ServerState nextState)
    {
        using (Packet _packet = new Packet((int)ClientCommand.NotifyServerStateChange))
        {
            _packet.Write((int)nextState);
            SendTCPData(_packet);
        }
    }

    public static void RequestDevice(Device device) {
        using (Packet _packet = new Packet((int)ClientCommand.RequestDevice))
        {
            switch(device) {
                case Device.CONTROLLER:
                     _packet.Write(60);
                    break;
                case Device.SHIFTY:
                    _packet.Write(40);
                    break;
                case Device.PANEL:
                    _packet.Write(50);
                    break;
                default:
                    Debug.Log("Request strange device");
                    break;
            }
            SendTCPData(_packet);
        }
    }

    #endregion
}
