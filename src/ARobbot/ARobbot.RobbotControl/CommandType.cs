namespace ARobbot.RobbotControl
{
    public enum CommandType
    {
        StartMotorCommand = (byte)'M',
        EndCommand = (byte)';',
        All = (byte)'A',
        Stop = (byte)'S',
        ForwardDirection = (byte)'F',
        BackwardDirection = (byte)'B',
        RPM = (byte)'R'
    }
}
