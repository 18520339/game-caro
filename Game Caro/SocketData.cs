using System;
using System.Drawing;

namespace Game_Caro
{
    [Serializable]
    class SocketData
    {
        private int command;
        private Point point;
        private string message;

        public int Command { get => command; set => command = value; }
        public Point Point { get => point; set => point = value; }
        public string Message { get => message; set => message = value; }

        public SocketData(int command, string message, Point point)
        {
            this.Command = command;
            this.Message = message;
            this.Point = point;
        }
    }

    public enum SocketCommand
    {
        SEND_POINT,
        SEND_MESSAGE,
        NEW_GAME,
        UNDO,
        REDO,
        END_GAME,
        TIME_OUT,
        QUIT
    }
}
