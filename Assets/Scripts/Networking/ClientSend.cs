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

    public static void NotifyGameStateChange(int state) {
        using(Packet _packet = new Packet((int)ClientCommand.NotifyGameStateChange)) {
            _packet.Write(state);
            SendTCPData(_packet);
        }
    }

    public static void NotifyStageStateChange(int state)
    {
        using (Packet _packet = new Packet((int)ClientCommand.NotifyStageStateChange))
        {
            _packet.Write(state);
            SendTCPData(_packet);
        }
    }

    public static void RequestDevicesStatus() {
        using (Packet _packet = new Packet((int)ClientCommand.RequestDevicesStatus))
        {
            SendTCPData(_packet);
        }
    }

    public static void RequestDevice(int device) {
        using (Packet _packet = new Packet((int)ClientCommand.RequestDevice))
        {
            _packet.Write(device);
            SendTCPData(_packet);
        }
    }

    #endregion
}
