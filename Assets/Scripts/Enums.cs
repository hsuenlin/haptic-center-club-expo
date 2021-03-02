public enum TrackerType 
{
        HC_Origin = 0,
        Player1 = 1,
        Player2 = 2,
        Player3 = 3,
        Shifty = 40,
        Shifty_Cartridge = 41,
        Panel = 50,
        Vive_Controller_Left = 60,
        Vive_Controller_Right = 61,
        Controller_Cartridge = 62,
        Gun = 70,
        Gun_Cartridge = 71,
        Shield = 80,
        Shield_Cartridge = 81,
    }

public enum DeviceName 
{
    Shifty = 40,
    Panel = 50,
    Controller = 60,
    Gun = 70,
    Shield = 80
}

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
public enum ServerCommand
{
    Welcome = 1,
    SendTrackerInfo = 2,
    SendDeviceStatus = 3,
    SendRequestIsSuccess = 4,
    SendDeviceReady = 5,
    SendControllerTriggerPress = 60,
    SendPanelInfo = 50,
    PlayerDisconnected = 100,

}
public enum ClientCommand
{
    SendWelcomeBack = 1,
    NotifyGameStateChange = 2,
    NotifyStageStateChange = 3,
    RequestDevicesStatus = 4,
    RequestDevice = 5
}