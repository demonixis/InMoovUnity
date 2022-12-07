namespace Demonixis.InMoov.ComputerVision
{
    public class ComputerVisionService: RobotService
    {
        public override RobotServices Type => RobotServices.ComputerVision;

        public override void SetPaused(bool paused)
        {
        }
    }
}