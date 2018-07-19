namespace Assets.Scripts.player
{
    public enum MoveStatus
    {
        DONE,
        ILLEGAL_MOVE,
        LEAVES_PLAYER_IN_CHECK
    }
    public static class MoveStatusMethods
    {
        public static bool isDone(this MoveStatus status)
        {
            bool moveStatus;
            switch(status)
            {
                case MoveStatus.DONE:
                    moveStatus = true;
                    break;
                case MoveStatus.ILLEGAL_MOVE:
                    moveStatus = false;
                    break;
                case MoveStatus.LEAVES_PLAYER_IN_CHECK:
                    moveStatus = false;
                    break;
                default:
                    moveStatus = false;
                    break;
            }
            return moveStatus;
        }
    }
}