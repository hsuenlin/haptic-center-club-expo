using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    public enum GameState
    {
        NONE = 0,
        INIT = 1,
        STORY = 2,
        BRANCH = 3,
        TENNIS_CLUB = 4,
        SHOOTING_CLUB = 5,
        MUSIC_CLUB = 6
    }

    public enum StageState
    {
        NONE = 0,
        DISTRIBUTE_DEVICE = 1,
        WAITING_DEVICE_READY = 2,
        STAGE_START = 3,
        STAGE_END = 4
    }
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
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientCommand.SendWelcomeBack))
        {
            _packet.Write(Client.instance.myId);
            //_packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    #endregion
}
