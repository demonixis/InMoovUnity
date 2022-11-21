namespace Demonixis.InMoov.ComputerVision
{
    public class DepthMappingService : RobotService
    {
        public override RobotServices Type => RobotServices.ComputerVision;
        
        public override void SetPaused(bool paused)
        {
        }
    }
}