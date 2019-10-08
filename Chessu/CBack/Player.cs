// ;

namespace CBack
{
    public abstract class Player
    {
        public PieceColor PlayerColor { get; set; }

        protected Player(PieceColor _color)
        {
            PlayerColor = _color;
        }
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(PieceColor _color)
            : base(_color)
        { }
    }

    public class ComputerPlayer : Player
    {
        private ComputerPlayer(PieceColor _color)
            : base(_color)
        { }
    }
}
