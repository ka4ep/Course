using System;
using System.Collections.Generic;
using System.Text;

namespace XO
{
    /// <summary>
    /// A two-player game only
    /// </summary>
    class XO
    {
        static void Main(string[] args)
        {
            // Settings
            // Main loop :
            //      Draw grid
            //      Player(N)
            //          Input + Validate
            //      Check winning combination
            // 

            string strRead = "";
            int x = 0;
            int y = 0;
            int temp;
            GameEvent inputEvent;
            // Yes, a label!
        start_again:
            Game.IsStarted = false;
            Settings.Startup();
            Game.Start();
            do 
            {
                Player.NextPlayer();

                Visual.Render();
                Visual.RenderStatusBar(Viewport.Max_Y - 3, Game.Board);
                Visual.RenderStatusBar(Viewport.Max_Y - 2, "Current player is   " + Player.IdentifyPlayer(Player.CurrentPlayer));

                    Visual.RenderStatusBar(Viewport.Max_Y - 1, "Enter X coordinate or command (S)tart (E)xit (A)uto                            ");
                    strRead = UserInput.ReadCommand(out inputEvent);
                    if (!string.IsNullOrEmpty(strRead))
                    {
                        if (inputEvent == GameEvent.COORD)
                        {
                            x = int.Parse(strRead);
                        }
                        if (inputEvent == GameEvent.START) { goto start_again; }
                        if (inputEvent == GameEvent.EXIT) { goto exit_game; }
                    }
                    Visual.Render();
                    Visual.RenderStatusBar(Viewport.Max_Y - 3, Game.Board);
                    Visual.RenderStatusBar(Viewport.Max_Y - 2, "Current player is   " + Player.IdentifyPlayer(Player.CurrentPlayer));
                    Visual.RenderStatusBar(Viewport.Max_Y - 1, "Enter Y coordinate or command (S)tart (E)xit (A)uto                            ");
                    strRead = UserInput.ReadCommand(out inputEvent);
                    if (!string.IsNullOrEmpty(strRead))
                    {
                        if (inputEvent == GameEvent.COORD)
                        {
                            y = int.Parse(strRead);
                        }
                        if (inputEvent == GameEvent.START) { goto start_again; }
                        if (inputEvent == GameEvent.EXIT) { goto exit_game; }
                    }
                // process coordinates
                    Game.SetBoardMove(x-1, y-1, Player.CurrentPlayer);
            }
            while (Game.IsStarted);
        exit_game:
            Console.WriteLine("Game finished!");
            Console.ReadLine();
            
        }
    }

    public static class Settings
    {        
        public static void Startup()
        {
            SetViewport(0,0,79,24);
            Visual.CellWidth = 1;
            SetGridGlyphs('-', '|', '+', ' ');
            SetPlayerGlyphs('·', 'X', 'O');
            SetColors(ConsoleColor.DarkGreen, ConsoleColor.White, ConsoleColor.DarkYellow, ConsoleColor.Green, ConsoleColor.Yellow);
            // setup game defauts
            Player.DefinePlayers("Player1",PlayerType.Human, "Player2", PlayerType.Human);
        }
        private static void SetViewport(int Min_X, int Min_Y, int Max_X, int Max_Y)
        {
            Viewport.Min_X = Min_X < Max_X ? Min_X : Max_X;
            Viewport.Min_Y = Min_Y < Max_Y ? Min_Y : Max_Y;
            Viewport.Max_X = Min_X > Max_X ? Min_X : Max_X;
            Viewport.Max_Y = Min_Y > Max_Y ? Min_Y : Max_Y;
        }
        private static void SetGridGlyphs(char LineV, char LineH, char Cross, char Back)
        {
            GridGlyph.LineV = LineV;
            GridGlyph.LineH = LineH;
            GridGlyph.Cross = Cross;
            GridGlyph.Back = Back;
        }
        private static void SetPlayerGlyphs(char Spawn, char Player1, char Player2)
        {
            PlayerGlyph.Spawn = Spawn;
            PlayerGlyph.Player1 = Player1;
            PlayerGlyph.Player2 = Player2;
        }

        private static void SetColors(ConsoleColor GridColor, ConsoleColor TextIn, ConsoleColor TextOut, ConsoleColor Player1, ConsoleColor Player2)
        {
            Colors.GridColor = GridColor;
            Colors.Player1 = Player1;
            Colors.Player2 = Player2;
            Colors.TextOut = TextOut;
            Colors.TextIn = TextIn;
        }        
    }

    public static class Player
    {
               
        public static void DefinePlayers(string PlayerName1,PlayerType Player1_Type, string PlayerName2, PlayerType Player2_Type)
        {
            Player.Player1.Name = PlayerName1;
            Player.Player1.Type = Player1_Type;
            Player.Player2.Name = PlayerName2;
            Player.Player2.Type = Player2_Type;
        }
        public static int CurrentPlayer = 0;
        public static void NextPlayer()
        {
            switch (CurrentPlayer)
            {
                case 2 : CurrentPlayer = 1; break;
                case 1 : CurrentPlayer = 2; break;
                default: CurrentPlayer = 1; break;
            } 
        }
        public static string IdentifyPlayer(int PlayerIndex)
        {
                if (PlayerIndex == 1) return Player1.Name + " (" + Enum.GetName(typeof(PlayerType), Player1.Type) + ")";
                if (PlayerIndex == 2) return Player2.Name + " (" + Enum.GetName(typeof(PlayerType), Player2.Type) + ")";
                return "Unknown player";
        }
        public static class Player1
        {
            public static PlayerType Type = PlayerType.Human;
            public static string Name = "Player 1";
        }
        public static class Player2
        {
            public static string Name = "Player 2";
            public static PlayerType Type = PlayerType.Human;
        }

        public static void Process_AI_Player(int Accuracy)
        {
        }
        public static void Process_Human_Player(int Coord_X, int Coord_Y)
        {
        }        
    }

    public enum PlayerType
    {
        Human,
        AI
    }

    public static class Game
    {
        public static int CellCount = 6;
        public static int CellToWin = 3;
        public static int MaxPlayers = 2;
        public static string Board = "";
        public static bool IsStarted = false;
        public static void ResetTheBoard()
        {
            if (IsStarted)            
                return;
            Board = FillChar(PlayerGlyph.Spawn,CellCount*CellCount);
        }
        private static string FillChar(char Symbol, int count)
        {
            string result = "";            
            for (int i = 0; i < count; i++)
            {
                result += Symbol;
            }
            return result;
        }
        public static void Start()
        {
            // checks
            ResetTheBoard();
            IsStarted = true;
        }
        public static void Finish()
        {
            IsStarted = false;
        }
        public static void SetBoardMove(int X, int Y, int PlayerIndex)
        {
            int BoardLen = Board.Length;
            int pos = Y * CellCount + X;
            string temp = "";
            if (pos > 0)
            {
                temp = Board.Substring(0, pos);
            }
            temp += PlayerIndex == 1 ? PlayerGlyph.Player1 : PlayerGlyph.Player2;
            if (pos < BoardLen-1)
            {
                temp += Board.Substring(pos+1, BoardLen-pos-1);
            }
            Board = temp;            
        }
    }

    public static class Viewport
    {
        public static int Min_X = 1;
        public static int Min_Y = 1;
        public static int Max_X = 80;
        public static int Max_Y = 25;
        public static int Fits_X(int CellWidth)
        {
            int result;
            int ActualWidth = Max_X - Min_X + 1;
            CellWidth = Math.Abs(CellWidth);
            result = (CellWidth == 0) || (CellWidth == 1) ? (ActualWidth - 1) / 3 : ((ActualWidth - 1) / (CellWidth + 1) < 3) ? 0 : (ActualWidth - 1) / (CellWidth + 1);
            return result;
        }
        public static int Fits_Y(int CellHeight)
        {
            int result;
            int ActualHeight = Max_Y - Min_Y + 1;
            CellHeight = Math.Abs(CellHeight);
            result = (CellHeight == 0) || (CellHeight == 1) ? (ActualHeight - 1) / 3 : ((ActualHeight - 1) / (CellHeight + 1) < 3) ? 0 : (ActualHeight - 1) / (CellHeight + 1);
            return result;
        }
        public static int Fits_Best(int CellSize)
        {
            int Fit_X = Fits_X(CellSize);
            int Fit_Y = Fits_Y(CellSize);
            return Fit_Y < Fit_X ? Fit_Y : Fit_X;
        }
    }

    public static class GridGlyph
    {
        public static char LineV = '-';
        public static char LineH = '|';
        public static char Cross = '+';
        public static char Back  = ' ';
    }
    public static class PlayerGlyph
    {
        public static char Spawn = '·';
        public static char Player1 = 'X';
        public static char Player2 = 'O';
    }

    public static class Colors
    {
        public static ConsoleColor GridColor = ConsoleColor.DarkGreen;
        public static ConsoleColor Player1 = ConsoleColor.Green;
        public static ConsoleColor Player2 = ConsoleColor.Yellow;
        public static ConsoleColor TextOut = ConsoleColor.DarkYellow;
        public static ConsoleColor TextIn = ConsoleColor.White;
    }

    public static class Visual
    {
        public static int CellWidth = 3;        
        private static void ValidateCellWidthAndCount(ref int Width, ref int Count)
        {
            // normally, when width = 3 count should be 5 or less...
        }

        public static void Render()
        {
            Console.Clear();
            MoveTo(Viewport.Min_X, Viewport.Min_Y);
            DrawGrid();
            DrawGameCells(Game.Board, Game.CellCount, CellWidth);
        }

        public static void DrawGrid()
        {
            int CellsFit = Viewport.Fits_Best(Visual.CellWidth);
            if ((CellWidth > 0) && (CellsFit >= Game.CellCount))
            {
                for (int y = 0; y <=  (CellWidth + 1) * Game.CellCount; y++)
                {
                    if (y % (CellWidth+1) == 0)
                    {
                        DrawGridLineV(CellWidth, Game.CellCount, Colors.GridColor);
                    }
                    else
                    {
                        DrawGridLineH(CellWidth, Game.CellCount, Colors.GridColor);
                    }
                    Console.WriteLine();
                }
            }
        }
        private static void DrawGridLineV(int CellWidth, int CellsCount, ConsoleColor GridColor)
        {
            Console.ForegroundColor = GridColor;
            for (int x = 0; x <= (CellWidth + 1) * CellsCount; x++)
            {
                if (x % (CellWidth + 1) == 0) { PutChar(GridGlyph.Cross, x + Viewport.Min_X, Console.CursorTop); }
                else { PutChar(GridGlyph.LineV, x + Viewport.Min_X, Console.CursorTop); }
            }
        }
        private static void DrawGridLineH(int CellWidth, int CellsCount, ConsoleColor GridColor)
        {
            Console.ForegroundColor = GridColor;
            for (int x = 0; x <= (CellWidth + 1) * CellsCount; x++)
            {
                if (x % (CellWidth + 1) == 0) { PutChar(GridGlyph.LineH, x + Viewport.Min_X, Console.CursorTop); }
                else { PutChar(GridGlyph.Back, x + Viewport.Min_X, Console.CursorTop); }
            }
        }

        public static void DrawGameCells(string Cells, int CellCount, int CellWidth)
        {
            for (int y = 0; y < CellCount; y++)
            {
                for (int x = 0; x < CellCount; x++)
                {
                    // (Width+1)*(x+1)-(Width+1)/2
                    DrawGameCell( (CellWidth+1)*(x+1)-(CellWidth+1)/2 , (CellWidth + 1)*(y+1) - (CellWidth+1)/2, Cells[(y * Game.CellCount) + x]);
                }
            }
        }
        private static void DrawGameCell(int X, int Y, char Symbol)
        {
            if (Symbol == PlayerGlyph.Spawn) { Console.ForegroundColor = Colors.GridColor; PutChar(Symbol, X, Y); }
            if (Symbol == PlayerGlyph.Player1) { Console.ForegroundColor = Colors.Player1; PutChar(Symbol, X, Y); }
            if (Symbol == PlayerGlyph.Player2) { Console.ForegroundColor = Colors.Player2; PutChar(Symbol, X, Y); }
        }

        public static void MoveTo(int X, int Y)
        {
            Console.CursorLeft = X;
            Console.CursorTop = Y;
        }
        public static void PutChar(char Symbol)
        {
            Console.Write(Symbol);
        }
        public static void PutChar(char Symbol, int X, int Y)
        {
            MoveTo(X, Y);            
            Console.Write(Symbol);
        }

        public static void RenderStatusBar(int Y_Pos, string Status)
        {
            MoveTo(0, Y_Pos);
            Console.ForegroundColor = Colors.TextOut;
            Console.WriteLine(Status);
        }

    }

    public enum GameEvent
    {
        NONE,
        START,
        EXIT,
        AUTO,
        COORD
    }

    /// <summary>
    /// Not a great way to implement this....
    /// </summary>
    public static class UserInput
    {
        public static string ReadCommand(out GameEvent Event)
        {
            //GameEvent Event;
            string input;
            input = Console.ReadLine();
            Event = ValidateCommand(ref input);            
            if (Event == GameEvent.COORD)
            {                
                return input;
            }
            if (Event == GameEvent.NONE)
            {
                return "";
            }
            else
            {
                return Event.ToString();
            }
        }

        public static GameEvent ValidateCommand(ref string input)
        {
            // leave only <space>, 0-9, S, E, A
            string AllowedChars = " 0123456789SEA";
            string result = "";
            char last = ' ';
            input = input.ToUpper();
            if ((input.Length == 0) || (string.IsNullOrEmpty(input)))
            {
                return GameEvent.NONE;
            }
            for (int i = 0; i < input.Length ; i++)
            {
                if (AllowedChars.Contains(input[i].ToString()))
                {
                    last = input[i];
                    // check for commandchars, then break
                    switch (last)
                    {
                        case 'S': return GameEvent.START;
                        case 'E': return GameEvent.EXIT; 
                        case 'A': return GameEvent.AUTO; 
                    }
                    result += last;                    
                }
                else
                {
                    if (last != ' ') 
                        result += ' ';
                }
            }
            // Trim, look for commands, then for coords
            input = result;
            // ------ temporary ------
            return GameEvent.COORD;
        }
    }



}
