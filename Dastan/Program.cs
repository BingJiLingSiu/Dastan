using System;
using System.Collections.Generic;
using System.Linq;

namespace Dastan
{
    class Program
    {
        static void Main(string[] args)
        {
            Dastan ThisGame = new Dastan(6, 6, 4);
            ThisGame.PlayGame();
            Console.WriteLine("Goodbye!");
            Console.ReadLine();
        }
    }

    class Dastan
    {
        protected List<Square> Board;
        protected int NoOfRows, NoOfColumns, MoveOptionOfferPosition;
        protected List<Player> Players = new List<Player>();
        protected List<string> MoveOptionOffer = new List<string>();
        protected Player CurrentPlayer;
        protected Random RGen = new Random();

        public Dastan(int R, int C, int NoOfPieces)
        {
            CreateCustomPlayers(); // task 1
            NoOfRows = R;
            NoOfColumns = C;
            MoveOptionOfferPosition = 0;
            CreateMoveOptionOffer();
            CreateBoard();
            CreatePieces(NoOfPieces);
            CreateMoveOptions(); // Q2 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_2
            CurrentPlayer = Players[0];
        }

        // task 1 - start
        private void CreateCustomPlayers()
        {
            Console.Write("Player one, enter your username: ");
            string playerone = Console.ReadLine();
            string playertwo;

            do
            {
                Console.Write("Player two, enter your username: ");
                playertwo = Console.ReadLine();
            } while(playerone.Equals(playertwo));

            Players.Add(new Player(playerone, 1));
            Players.Add(new Player(playertwo, -1));
        }
        // task 1 - end
        
        private void DisplayBoard()
        {
            Console.Write(Environment.NewLine + "   ");
            for (int Column = 1; Column <= NoOfColumns; Column++)
            {
                Console.Write(Column.ToString() + "  ");
            }
            Console.Write(Environment.NewLine + "  ");
            for (int Count = 1; Count <= NoOfColumns; Count++)
            {
                Console.Write("---");
            }
            Console.WriteLine("-");
            for (int Row = 1; Row <= NoOfRows; Row++)
            {
                Console.Write(Row.ToString() + " ");
                for (int Column = 1; Column <= NoOfColumns; Column++)
                {
                    int Index = GetIndexOfSquare(Row * 10 + Column);
                    Console.Write("|" + Board[Index].GetSymbol());
                    Piece PieceInSquare = Board[Index].GetPieceInSquare();
                    if (PieceInSquare == null)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write(PieceInSquare.GetSymbol());
                    }
                }
                Console.WriteLine("|");
            }
            Console.Write("  -");
            for (int Column = 1; Column <= NoOfColumns; Column++)
            {
                Console.Write("---");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        
        private void DisplayBoardMoveOptions(int Choice, int StartSquareReference)
        {
            Console.Write(Environment.NewLine + "   ");
            for (int Column = 1; Column <= NoOfColumns; Column++)
            {
                Console.Write(Column.ToString() + "  ");
            }
            Console.Write(Environment.NewLine + "  ");
            for (int Count = 1; Count <= NoOfColumns; Count++)
            {
                Console.Write("---");
            }
            Console.WriteLine("-");
            for (int Row = 1; Row <= NoOfRows; Row++)
            {
                Console.Write(Row.ToString() + " ");
                for (int Column = 1; Column <= NoOfColumns; Column++)
                {
                    int Index = GetIndexOfSquare(Row * 10 + Column);
                    Console.Write("|" + Board[Index].GetSymbol());
                    Piece PieceInSquare = Board[Index].GetPieceInSquare();
                    // Q6 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Show_where_the_piece_can_move
                    if (CheckSquareIsValid(Row * 10 + Column, false) &&
                        CurrentPlayer.CheckPlayerMove(Choice, StartSquareReference, Row * 10 + Column))
                    {
                        Console.Write("^");
                    }
                    // Q6 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Show_where_the_piece_can_move
                    else if (PieceInSquare == null)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write(PieceInSquare.GetSymbol());
                    }
                }
                Console.WriteLine("|");
            }
            Console.Write("  -");
            for (int Column = 1; Column <= NoOfColumns; Column++)
            {
                Console.Write("---");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private void DisplayState()
        {
            DisplayBoard();
            Console.WriteLine("Move option offer: " + MoveOptionOffer[MoveOptionOfferPosition]);
            Console.WriteLine();
            Console.WriteLine(CurrentPlayer.GetPlayerStateAsString());
            Console.WriteLine("Turn: " + CurrentPlayer.GetName());
            Console.WriteLine();
        }

        private int GetIndexOfSquare(int SquareReference)
        {
            int Row = SquareReference / 10;
            int Col = SquareReference % 10;
            return (Row - 1) * NoOfColumns + (Col - 1);
        }

        private bool CheckSquareInBounds(int SquareReference)
        {
            int Row = SquareReference / 10;
            int Col = SquareReference % 10;
            if (Row < 1 || Row > NoOfRows)
            {
                return false;
            }
            else if (Col < 1 || Col > NoOfColumns)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CheckSquareIsValid(int SquareReference, bool StartSquare)
        {
            if (!CheckSquareInBounds(SquareReference))
            {
                return false;
            }
            Piece PieceInSquare = Board[GetIndexOfSquare(SquareReference)].GetPieceInSquare();
            if (PieceInSquare == null)
            {
                if (StartSquare)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()))
            {
                if (StartSquare)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (StartSquare)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private bool CheckIfGameOver()
        {
            bool Player1HasMirza = false;
            bool Player2HasMirza = false;
            foreach (var S in Board)
            {
                Piece PieceInSquare = S.GetPieceInSquare();
                if (PieceInSquare != null)
                {
                    if (S.ContainsKotla() && PieceInSquare.GetTypeOfPiece() == "mirza" && !PieceInSquare.GetBelongsTo().SameAs(S.GetBelongsTo()))
                    {
                        return true;
                    }
                    else if (PieceInSquare.GetTypeOfPiece() == "mirza" && PieceInSquare.GetBelongsTo().SameAs(Players[0]))
                    {
                        Player1HasMirza = true;
                    }
                    else if (PieceInSquare.GetTypeOfPiece() == "mirza" && PieceInSquare.GetBelongsTo().SameAs(Players[1]))
                    {
                        Player2HasMirza = true;
                    }
                }
            }
            return !(Player1HasMirza && Player2HasMirza);
        }

        private int GetSquareReference(string Description)
        {
            int SelectedSquare;
            Console.Write("Enter the square " + Description + " (row number followed by column number): ");
            SelectedSquare = Convert.ToInt32(Console.ReadLine());
            return SelectedSquare;
        }

        private void UseMoveOptionOffer()
        {
            int ReplaceChoice;
            Console.Write("Choose the move option from your queue to replace (1 to 5): ");
            ReplaceChoice = Convert.ToInt32(Console.ReadLine());
            CurrentPlayer.UpdateMoveOptionQueueWithOffer(ReplaceChoice - 1, CreateMoveOption(MoveOptionOffer[MoveOptionOfferPosition], CurrentPlayer.GetDirection()));
            CurrentPlayer.ChangeScore(-(10 - (ReplaceChoice * 2)));
            MoveOptionOfferPosition = RGen.Next(0, 5);
        }

        private int GetPointsForOccupancyByPlayer(Player CurrentPlayer)
        {
            int ScoreAdjustment = 0;
            foreach (var S in Board)
            {
                ScoreAdjustment += (S.GetPointsForOccupancy(CurrentPlayer));
            }
            return ScoreAdjustment;
        }

        private void UpdatePlayerScore(int PointsForPieceCapture)
        {
            CurrentPlayer.ChangeScore(GetPointsForOccupancyByPlayer(CurrentPlayer) + PointsForPieceCapture);
        }

        private int CalculatePieceCapturePoints(int FinishSquareReference)
        {
            if (Board[GetIndexOfSquare(FinishSquareReference)].GetPieceInSquare() != null)
            {
                return Board[GetIndexOfSquare(FinishSquareReference)].GetPieceInSquare().GetPointsIfCaptured();
            }
            return 0;
        }

        public void PlayGame()
        {
            bool GameOver = false;
            while (!GameOver)
            {
                DisplayState();
                bool SquareIsValid = false;
                int Choice;
                do
                {
                    // Q4 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_4
                    List<string> Extras = new List<string>();
                    Extras.Add("or 9 to take the offer");
                    if (CurrentPlayer.GetSpaceJumpState()) Extras.Add("or 8 for space jump");
                    Console.Write($"Choose move option to use from queue (1 to 3) {String.Join(" ", Extras.ToArray())}: ");
                    // Q4 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_4
                    Choice = Convert.ToInt32(Console.ReadLine());
                    if (Choice == 9)
                    {
                        UseMoveOptionOffer();
                        DisplayState();
                    }
                }
                while (!(Choice != 1 || Choice != 2 || Choice != 3 || Choice != 8)); // Q4 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_4
                int StartSquareReference = 0;
                while (!SquareIsValid)
                {
                    StartSquareReference = GetSquareReference("containing the piece to move");
                    SquareIsValid = CheckSquareIsValid(StartSquareReference, true);
                }
                int FinishSquareReference = 0;
                if (Choice != 8) // Q4 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_4
                {
                    SquareIsValid = false;
                    DisplayBoardMoveOptions(Choice, StartSquareReference); // Q6 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Show_where_the_piece_can_move
                    // Q3 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_3
                    bool MoveLegal = false;
                    while (!(SquareIsValid && MoveLegal))
                    {
                        FinishSquareReference = GetSquareReference("to move to");
                        SquareIsValid = CheckSquareIsValid(FinishSquareReference, false);

                        MoveLegal = CurrentPlayer.CheckPlayerMove(Choice, StartSquareReference, FinishSquareReference);
                    
                        if (!MoveLegal)
                            Console.WriteLine("Cannot move to that square with this move");
                    
                        if (!SquareIsValid)
                            Console.WriteLine("Invalid position");
                    }
                    int PointsForPieceCapture = CalculatePieceCapturePoints(FinishSquareReference);
                    CurrentPlayer.ChangeScore(-(Choice + (2 * (Choice - 1))));
                    CurrentPlayer.UpdateQueueAfterMove(Choice);
                    UpdateBoard(StartSquareReference, FinishSquareReference);
                    UpdatePlayerScore(PointsForPieceCapture);
                    Console.WriteLine("New score: " + CurrentPlayer.GetScore() + Environment.NewLine);
                    // Q3 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_3
                }
                // Q4 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_4
                else
                {
                    do
                    {
                        FinishSquareReference = RGen.Next(1, NoOfRows) * 10 + RGen.Next(1, NoOfColumns);
                        SquareIsValid = CheckSquareIsValid(FinishSquareReference, false);
                    } while (FinishSquareReference == StartSquareReference && !SquareIsValid);
                    int PointsForPieceCapture = CalculatePieceCapturePoints(FinishSquareReference);
                    CurrentPlayer.ChangeScore(-(Choice + (2 * (Choice - 1))));
                    UpdateBoard(StartSquareReference, FinishSquareReference);
                    UpdatePlayerScore(PointsForPieceCapture);
                    Console.WriteLine("New score: " + CurrentPlayer.GetScore() + Environment.NewLine);
                    
                    CurrentPlayer.UseSpaceJump();
                }
                // Q4 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_4
                if (CurrentPlayer.SameAs(Players[0]))
                {
                    CurrentPlayer = Players[1];
                }
                else
                {
                    CurrentPlayer = Players[0];
                }
                GameOver = CheckIfGameOver();
            }
            DisplayState();
            DisplayFinalResult();
        }

        private void UpdateBoard(int StartSquareReference, int FinishSquareReference)
        {
            Board[GetIndexOfSquare(FinishSquareReference)].SetPiece(Board[GetIndexOfSquare(StartSquareReference)].RemovePiece());
        }

        private void DisplayFinalResult()
        {
            if (Players[0].GetScore() == Players[1].GetScore())
            {
                Console.WriteLine("Draw!");
            }
            else if (Players[0].GetScore() > Players[1].GetScore())
            {
                Console.WriteLine(Players[0].GetName() + " is the winner!");
            }
            else
            {
                Console.WriteLine(Players[1].GetName() + " is the winner!");
            }
        }

        private void CreateBoard()
        {
            Square S;
            Board = new List<Square>();
            for (int Row = 1; Row <= NoOfRows; Row++)
            {
                for (int Column = 1; Column <= NoOfColumns; Column++)
                {
                    if (Row == 1 && Column == Math.Round((float)NoOfColumns / 2)) // eq4
                    {
                        S = new Kotla(Players[0], "K");
                    }
                    else if (Row == NoOfRows && Column == NoOfColumns / 2 + 1)
                    {
                        S = new Kotla(Players[1], "k");
                    }
                    else
                    {
                        S = new Square();
                    }
                    Board.Add(S);
                }
            }
        }

        private void CreatePieces(int NoOfPieces)
        {
            Piece CurrentPiece;
            for (int Count = 1; Count <= NoOfPieces; Count++)
            {
                CurrentPiece = new Piece("piece", Players[0], 1, "!");
                Board[GetIndexOfSquare(2 * 10 + Count + 1)].SetPiece(CurrentPiece);
            }
            CurrentPiece = new Piece("mirza", Players[0], 5, "1");
            Board[GetIndexOfSquare(10 + (int)Math.Round((float)NoOfColumns / 2))].SetPiece(CurrentPiece); // eq4
            for (int Count = 1; Count <= NoOfPieces; Count++)
            {
                CurrentPiece = new Piece("piece", Players[1], 1, "\"");
                Board[GetIndexOfSquare((NoOfRows - 1) * 10 + Count + 1)].SetPiece(CurrentPiece);
            }
            CurrentPiece = new Piece("mirza", Players[1], 5, "2");
            Board[GetIndexOfSquare(NoOfRows * 10 + (NoOfColumns / 2 + 1))].SetPiece(CurrentPiece);
        }

        private void CreateMoveOptionOffer()
        {
            MoveOptionOffer.Add("sarukh"); // task 3
            MoveOptionOffer.Add("tibblecross"); // Q1 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_1
            MoveOptionOffer.Add("rook"); // Q2 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_2
            MoveOptionOffer.Add("faris"); // task 2
            MoveOptionOffer.Add("jazair");
            MoveOptionOffer.Add("chowkidar");
            MoveOptionOffer.Add("cuirassier");
            MoveOptionOffer.Add("ryott");
            MoveOptionOffer.Add("faujdar");
        }
        // task 2 start
        private MoveOption CreateFarisMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("faris");
            Move NewMove = new Move(2 * Direction, 1);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(2 * Direction, -1);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-2 * Direction, 1);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-2 * Direction, -1);
            NewMoveOption.AddToPossibleMoves(NewMove);

            NewMove = new Move(1, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1, -2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1, -2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }
        // task 2 end
        // task 3 start
        private MoveOption CreateSarukhMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("sarukh");
            Move NewMove = new Move(0, 1);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -1);
            NewMoveOption.AddToPossibleMoves(NewMove);
            
            NewMove = new Move(1 * Direction, 1);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, -1);
            NewMoveOption.AddToPossibleMoves(NewMove);
            
            NewMove = new Move(2 * Direction, 0);
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }
        // task 3 end
        // Q2 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_2
        private MoveOption CreateRookMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("rook");
            for (int row = 1; row < NoOfRows; ++row)
            {
                int Index = Direction < 0 ? (row - 1) * 10 : (NoOfRows - row - 2) * 10;
                if (Board[Index].GetPieceInSquare() != null)
                {
                    break;
                }
                Move NewMove = new Move(row * Direction, 0);
                NewMoveOption.AddToPossibleMoves(NewMove);
            }
            return NewMoveOption;
        }
        // Q2 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_2
        // Q1 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_1
        private MoveOption CreateTibbleCrossMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("tibblecross");
            Move NewMove = new Move(2 * Direction, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(2 * Direction, -2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-2 * Direction, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-2 * Direction, -2 * Direction);
            return NewMoveOption;
        }
        // Q1 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_1

        private MoveOption CreateRyottMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("ryott");
            Move NewMove = new Move(0, 1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, 0);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, 0);
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateFaujdarMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("faujdar");
            Move NewMove = new Move(0, -1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, 1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateJazairMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("jazair");
            Move NewMove = new Move(2 * Direction, 0);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(2 * Direction, -2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(2 * Direction, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, -1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, 1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateCuirassierMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("cuirassier");
            Move NewMove = new Move(1 * Direction, 0);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(2 * Direction, 0);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, -2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateChowkidarMoveOption(int Direction)
        {
            MoveOption NewMoveOption = new MoveOption("chowkidar");
            Move NewMove = new Move(1 * Direction, 1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(1 * Direction, -1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, 1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(-1 * Direction, -1 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, 2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            NewMove = new Move(0, -2 * Direction);
            NewMoveOption.AddToPossibleMoves(NewMove);
            return NewMoveOption;
        }

        private MoveOption CreateMoveOption(string Name, int Direction)
        {
            if (Name == "chowkidar")
            {
                return CreateChowkidarMoveOption(Direction);
            }
            else if (Name == "ryott")
            {
                return CreateRyottMoveOption(Direction);
            }
            else if (Name == "faujdar")
            {
                return CreateFaujdarMoveOption(Direction);
            }
            else if (Name == "jazair")
            {
                return CreateJazairMoveOption(Direction);
            }
            // task 2 start
            else if (Name == "faris")
            {
                return CreateFarisMoveOption(Direction);
            }
            // task 2 end
            // Q1 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_1
            else if (Name == "tibblecross")
            {
                return CreateTibbleCrossMoveOption(Direction);
            }
            // Q1 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_1
            // Q2 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_2
            else if (Name == "rook")
            {
                return CreateRookMoveOption(Direction);
            }
            // Q2 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_2
            // task 3 start
            else if (Name == "sarukh")
            {
                return CreateSarukhMoveOption(Direction);
            }
            // task 3 end
            else
            {
                return CreateCuirassierMoveOption(Direction);
            }
        }

        private void CreateMoveOptions()
        {
            Players[0].AddToMoveOptionQueue(CreateMoveOption("tibblecross", 1)); // Q1 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_1
            Players[0].AddToMoveOptionQueue(CreateMoveOption("rook", 1)); // Q2 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_2
            Players[0].AddToMoveOptionQueue(CreateMoveOption("faris", 1)); // task 2
            Players[0].AddToMoveOptionQueue(CreateMoveOption("ryott", 1));
            Players[0].AddToMoveOptionQueue(CreateMoveOption("sarukh", 1)); // task 3
            Players[0].AddToMoveOptionQueue(CreateMoveOption("chowkidar", 1));
            Players[0].AddToMoveOptionQueue(CreateMoveOption("cuirassier", 1));
            Players[0].AddToMoveOptionQueue(CreateMoveOption("faujdar", 1));
            Players[0].AddToMoveOptionQueue(CreateMoveOption("jazair", 1));
            Players[0].ShuffleQueue();

            Players[1].AddToMoveOptionQueue(CreateMoveOption("tibblecross", -1)); // Q1 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_1
            Players[1].AddToMoveOptionQueue(CreateMoveOption("rook", -1)); // Q2 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_2
            Players[1].AddToMoveOptionQueue(CreateMoveOption("faris", -1)); // task 2
            Players[1].AddToMoveOptionQueue(CreateMoveOption("ryott", -1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("sarukh", -1)); // task 3
            Players[1].AddToMoveOptionQueue(CreateMoveOption("chowkidar", -1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("jazair", -1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("faujdar", -1));
            Players[1].AddToMoveOptionQueue(CreateMoveOption("cuirassier", -1));
            Players[1].ShuffleQueue();
        }
    }

    class Piece
    {
        protected string TypeOfPiece, Symbol;
        protected int PointsIfCaptured;
        protected Player BelongsTo;

        public Piece(string T, Player B, int P, string S)
        {
            TypeOfPiece = T;
            BelongsTo = B;
            PointsIfCaptured = P;
            Symbol = S;
        }

        public string GetSymbol()
        {
            return Symbol;
        }

        public string GetTypeOfPiece()
        {
            return TypeOfPiece;
        }

        public Player GetBelongsTo()
        {
            return BelongsTo;
        }

        public int GetPointsIfCaptured()
        {
            return PointsIfCaptured;
        }
    }

    class Square
    {
        protected string Symbol;
        protected Piece PieceInSquare;
        protected Player BelongsTo;

        public Square()
        {
            PieceInSquare = null;
            BelongsTo = null;
            Symbol = " ";
        }

        public virtual void SetPiece(Piece P)
        {
            PieceInSquare = P;
        }

        public virtual Piece RemovePiece()
        {
            Piece PieceToReturn = PieceInSquare;
            PieceInSquare = null;
            return PieceToReturn;
        }

        public virtual Piece GetPieceInSquare()
        {
            return PieceInSquare;
        }

        public virtual string GetSymbol()
        {
            return Symbol;
        }

        public virtual int GetPointsForOccupancy(Player CurrentPlayer)
        {
            return 0;
        }

        public virtual Player GetBelongsTo()
        {
            return BelongsTo;
        }

        public virtual bool ContainsKotla()
        {
            if (Symbol == "K" || Symbol == "k")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class Kotla : Square
    {
        public Kotla(Player P, string S) : base()
        {
            BelongsTo = P;
            Symbol = S;
        }

        public override int GetPointsForOccupancy(Player CurrentPlayer)
        {
            if (PieceInSquare == null)
            {
                return 0;
            }
            else if (BelongsTo.SameAs(CurrentPlayer))
            {
                if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) && (PieceInSquare.GetTypeOfPiece() == "piece" || PieceInSquare.GetTypeOfPiece() == "mirza"))
                {
                    return 5;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (CurrentPlayer.SameAs(PieceInSquare.GetBelongsTo()) && (PieceInSquare.GetTypeOfPiece() == "piece" || PieceInSquare.GetTypeOfPiece() == "mirza"))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    class MoveOption
    {
        protected string Name;
        protected List<Move> PossibleMoves;

        public MoveOption(string N)
        {
            Name = N;
            PossibleMoves = new List<Move>();
        }

        public void AddToPossibleMoves(Move M)
        {
            PossibleMoves.Add(M);
        }

        public string GetName()
        {
            return Name;
        }

        public bool CheckIfThereIsAMoveToSquare(int StartSquareReference, int FinishSquareReference)
        {
            int StartRow = StartSquareReference / 10;
            int StartColumn = StartSquareReference % 10;
            int FinishRow = FinishSquareReference / 10;
            int FinishColumn = FinishSquareReference % 10;
            foreach (var M in PossibleMoves)
            {
                if (StartRow + M.GetRowChange() == FinishRow && StartColumn + M.GetColumnChange() == FinishColumn)
                {
                    return true;
                }
            }
            return false;
        }
    }

    class Move
    {
        protected int RowChange, ColumnChange;

        public Move(int R, int C)
        {
            RowChange = R;
            ColumnChange = C;
        }

        public int GetRowChange()
        {
            return RowChange;
        }

        public int GetColumnChange()
        {
            return ColumnChange;
        }
    }

    class MoveOptionQueue
    {
        private List<MoveOption> Queue = new List<MoveOption>();
        Random rng = new Random(); // Q7 - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Random_start_queue

        // Q7 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Random_start_queue
        public void Shuffle()
        {
            // Shuffle based on Fisher-Yates shuffle (https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle)
            int n = Queue.Count;
            while (n > 1)
            {
                n--;
                int R = rng.Next(n + 1);
                (Queue[R], Queue[n]) = (Queue[n], Queue[R]);
            }
        }
        // Q7 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Random_start_queue
        
        public string GetQueueAsString()
        {
            string QueueAsString = "";
            int Count = 1;
            foreach (var M in Queue)
            {
                QueueAsString += Count.ToString() + ". " + M.GetName() + "   ";
                Count += 1;
            }
            return QueueAsString;
        }

        public void Add(MoveOption NewMoveOption)
        {
            Queue.Add(NewMoveOption);
        }

        public void Replace(int Position, MoveOption NewMoveOption)
        {
            Queue[Position] = NewMoveOption;
        }

        public void MoveItemToBack(int Position)
        {
            MoveOption Temp = Queue[Position];
            Queue.RemoveAt(Position);
            Queue.Add(Temp);
        }

        public MoveOption GetMoveOptionInPosition(int Pos)
        {
            return Queue[Pos];
        }
    }

    class Player
    {
        private string Name;
        private int Direction, Score;
        private MoveOptionQueue Queue = new MoveOptionQueue();
        private bool hasSpaceJump = true; 

        public Player(string N, int D)
        {
            Score = 100;
            Name = N;
            Direction = D;
        }

        // Q7 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Random_start_queue
        public void ShuffleQueue()
        {
            Queue.Shuffle();
        }
        // Q7 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Random_start_queue

        public bool SameAs(Player APlayer)
        {
            if (APlayer == null)
            {
                return false;
            }
            else if (APlayer.GetName() == Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Q4 start - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_4
        public bool GetSpaceJumpState()
        {
            return hasSpaceJump;
        }

        public void UseSpaceJump()
        {
            hasSpaceJump = false;
        }
        // Q4 end - https://en.wikibooks.org/wiki/A-level_Computing/AQA/Paper_1/Skeleton_program/2023#Question_4

        public string GetPlayerStateAsString()
        {
            return Name + Environment.NewLine + "Score: " + Score.ToString() + Environment.NewLine + "Move option queue: " + Queue.GetQueueAsString() + Environment.NewLine;
        }

        public void AddToMoveOptionQueue(MoveOption NewMoveOption)
        {
            Queue.Add(NewMoveOption);
        }

        public void UpdateQueueAfterMove(int Position)
        {
            Queue.MoveItemToBack(Position - 1);
        }

        public void UpdateMoveOptionQueueWithOffer(int Position, MoveOption NewMoveOption)
        {
            Queue.Replace(Position, NewMoveOption);
        }

        public int GetScore()
        {
            return Score;
        }

        public string GetName()
        {
            return Name;
        }

        public int GetDirection()
        {
            return Direction;
        }

        public void ChangeScore(int Amount)
        {
            Score += Amount;
        }

        public bool CheckPlayerMove(int Pos, int StartSquareReference, int FinishSquareReference)
        {
            MoveOption Temp = Queue.GetMoveOptionInPosition(Pos - 1);
            return Temp.CheckIfThereIsAMoveToSquare(StartSquareReference, FinishSquareReference);
        }
    }
}

