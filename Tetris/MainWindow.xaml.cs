//Author: Rúben Ribeiro AKA Sephyre
//Date  : 20 Jan 2019

using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private const int Gamespeed = 700;// millisecond

        // List for add 2 sound wav
        readonly List<SoundPlayer> _soundList=new List<SoundPlayer>();

        readonly DispatcherTimer _timer;
        Random _shapeRandom;
        private int _rowCount;
        private int _columnCount;
        private int _leftPos;
        private int _downPos;
        private int _currentShapeWidth;
        private int _currentShapeHeigth;
        private int _currentShapeNumber;
        private int _nextShapeNumber;
        private readonly int _tetrisGridColumn;
        private readonly int _tetrisGridRow;
        private int _rotation;
        private bool _gameActive;
        private bool _nextShapeDrawed;
        private int[,] _currentShape;
        private bool _isRotated;
        private bool _bottomCollided;
        private bool _leftCollided;
        private bool _rightCollided;
        private bool _isGameOver;
        private int _gameSpeed;
        private readonly int _levelScale = 60;// every 60 second increase level by 1 until 10
        private double _gameSpeedCounter;
        private int _gameLevel=1;
        private int _gameScore;

        private static readonly Color OShapeColor = Colors.GreenYellow;
        private static readonly Color IShapeColor = Colors.Red;
        private static readonly Color TShapeColor = Colors.Gold;
        private static readonly Color SShapeColor = Colors.Violet;
        private static readonly Color ZShapeColor = Colors.DeepSkyBlue;
        private static readonly Color JShapeColor = Colors.Cyan;
        private static readonly Color LShapeColor = Colors.LightSeaGreen;
        List<int> _currentShapeRow;
        List<int> _currentShapeColumn;

        
        // Color for shape Shape
        readonly Color[] _shapeColor = {  OShapeColor,IShapeColor,
                                TShapeColor,SShapeColor,
                                ZShapeColor,JShapeColor,
                                LShapeColor
                             };
        // ---------
        readonly string[] _arrayShapes = { "","OShape" , "IShape0",
                                        "TShape0","SShape0",
                                        "ZShape0","JShape0",
                                        "LShape0"
                                   };

        #region Array of Shapes shape 

        // arrays of Shape shape
        //---- O Shape------------
        public int[,] OShape = new int[2, 2] { { 1, 1 },  // * *
                                                    { 1, 1 }}; // * *

        //---- I Shape------------
        public int[,] IShape0 = new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 0, 0 } };// * * * *

        public int[,] IShape90 = new int[4, 2] {{ 1,0 },   // *  
                                                       { 1,0 },  // *
                                                       { 1,0 },  // *
                                                       { 1,0 }}; // *
        //---- T Shape------------
        public int[,] TShape0 = new int[2, 3] {{0,1,0},    //    * 
                                                     {1,1,1}};   //  * * *

        public int[,] TShape90 = new int[3, 2] {{1,0},     //  * 
                                                      {1,1},     //  * *
                                                      {1,0}};    //  *  

        public int[,] TShape180 = new int[2, 3] {{1,1,1},  // * * *
                                                       {0,1,0}}; //   * 

        public int[,] TShape270 = new int[3, 2] {{0,1},    //   * 
                                                       {1,1},    // * *
                                                       {0,1}};   //   *  
        //---- S Shape------------
        public int[,] SShape0 = new int[2, 3] {{0,1,1},    //   * *
                                                     {1,1,0}};   // * *

        public int[,] SShape90 = new int[3, 2] {{1,0},     // *
                                                      {1,1},     // * *
                                                      {0,1}};    //   *
        //---- Z Shape------------
        public int[,] ZShape0 = new int[2, 3] {{1,1,0},    // * *
                                                     {0,1,1}};   //   * *

        public int[,] ZShape90 = new int[3, 2] {{0,1},     //   *
                                                      {1,1},     // * *
                                                      {1,0}};    // *
        //---- J Shape------------
        public int[,] JShape0 = new int[2, 3] {{1,0,0},    // * 
                                                     {1,1,1}};   // * * *

        public int[,] JShape90 = new int[3, 2] {{1,1},     // * * 
                                                      {1,0},     // *
                                                      {1,0}};    // * 

        public int[,] JShape180 = new int[2, 3] {{1,1,1},  // * * * 
                                                       {0,0,1}}; //     *

        public int[,] JShape270 = new int[3, 2] {{0,1},    //   * 
                                                       {0,1},    //   *
                                                       {1,1 }};  // * *

        //---- L Shape------------
        public int[,] LShape0 = new int[2, 3] {{0,0,1},    //     * 
                                                     {1,1,1}};   // * * *

        public int[,] LShape90 = new int[3, 2] {{1,0},     // *  
                                                      {1,0},     // *
                                                      {1,1}};    // * *

        public int[,] LShape180 = new int[2, 3] {{1,1,1},  // * * * 
                                                       {1,0,0}}; // *

        public int[,] LShape270 = new int[3, 2] {{1,1},    // * * 
                                                       {0,1},    //   *
                                                       {0,1 }};  //   *

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            _gameSpeed = Gamespeed;  
            
            //created event for key press
            KeyDown += MainWindow_KeyDown;

            // init timer
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, _gameSpeed)
            };

            // 700 millisecond
            _timer.Tick += Timer_Tick;
            _tetrisGridColumn = tetrisGrid.ColumnDefinitions.Count;
            _tetrisGridRow = tetrisGrid.RowDefinitions.Count;
            _shapeRandom = new Random();
            _currentShapeNumber = _shapeRandom.Next(1, 8);
            _nextShapeNumber = _shapeRandom.Next(1, 8);
            nextTxt.Visibility = levelTxt.Visibility= GameOverTxt.Visibility = Visibility.Collapsed;

            // Add the 2 wav sound in list
            _soundList.Add(new SoundPlayer(Properties.Resources.collided));
            _soundList.Add(new SoundPlayer(Properties.Resources.deleteLine));
        }
            
        // Key event method for moving shape down,rigth,left and rotation
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (!_timer.IsEnabled) { return; }
            switch (e.Key.ToString())
            {
                case "Up":
                    _rotation += 90;
                    if (_rotation > 270) { _rotation = 0; }
                    ShapeRotation(_rotation);
                    break;
                case "Down":
                    _downPos++;
                    break;
                case "Right":
                    // Check if collided
                    TetroCollided(); 
                    if (!_rightCollided) { _leftPos++; }
                    _rightCollided = false;
                    break;
                case "Left":
                    // Check if collided
                    TetroCollided(); 
                    if (!_leftCollided) { _leftPos--;}
                    _leftCollided = false;
                    break;
            }
            MoveShape();
        }

        // Rotation Shapes 
        private void ShapeRotation(int rotation)
        {
            // Check if collided
            if (RotationCollided(_rotation))
            {
                _rotation -= 90;
                return;
            }

            if (_arrayShapes[_currentShapeNumber].IndexOf("I", StringComparison.Ordinal) == 0)
            {              
                if (rotation > 90) { rotation = _rotation = 0; }
                _currentShape = GetVariableByString("IShape" + rotation);
            }
            else if (_arrayShapes[_currentShapeNumber].IndexOf("T", StringComparison.Ordinal) == 0)
            {
                _currentShape = GetVariableByString("TShape" + rotation);
            }
            else if (_arrayShapes[_currentShapeNumber].IndexOf("S", StringComparison.Ordinal) == 0)
            {
                if (rotation > 90) { rotation = _rotation = 0; }
                _currentShape = GetVariableByString("SShape" + rotation);
            }
            else if (_arrayShapes[_currentShapeNumber].IndexOf("Z", StringComparison.Ordinal) == 0)
            {
                if (rotation > 90) { rotation = _rotation = 0; }
                _currentShape = GetVariableByString("ZShape" + rotation);
            }
            else if (_arrayShapes[_currentShapeNumber].IndexOf("J", StringComparison.Ordinal) == 0)
            {
                _currentShape = GetVariableByString("JShape" + rotation);
            }
            else if (_arrayShapes[_currentShapeNumber].IndexOf("L", StringComparison.Ordinal) == 0)
            {
                _currentShape = GetVariableByString("LShape" + rotation);
            }
            else if (_arrayShapes[_currentShapeNumber].IndexOf("O", StringComparison.Ordinal) == 0) // Do not rotate this
            {
                return;
            }
           
            _isRotated = true;
            AddShape(_currentShapeNumber, _leftPos, _downPos);
        }

       
        // Timer tick method for moving shape down
        private void Timer_Tick(object sender, EventArgs e)
        {
            _downPos++;
            MoveShape();
            if (_gameSpeedCounter >= _levelScale)
            {          
                if (_gameSpeed >= 50)
                {
                    _gameSpeed -= 50;
                    _gameLevel++;
                    levelTxt.Text = "Level: " + _gameLevel;
                }
                else { _gameSpeed = 50; }
                _timer.Stop();
                _timer.Interval = new TimeSpan(0, 0, 0, 0, _gameSpeed);
                _timer.Start();
                _gameSpeedCounter = 0;
            }
            _gameSpeedCounter += (_gameSpeed/1000f);
          
        }

       
        // Button start stop clicked method
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
          
            if(_isGameOver)
            {
                tetrisGrid.Children.Clear();
                nextShapeCanvas.Children.Clear();
                GameOverTxt.Visibility = Visibility.Collapsed;
                _isGameOver = false;
            }
            if(!_timer.IsEnabled)
            {
                if (!_gameActive) { scoreTxt.Text = "0"; _leftPos = 3; AddShape(_currentShapeNumber,_leftPos); }
                nextTxt.Visibility = levelTxt.Visibility = Visibility.Visible;
                levelTxt.Text = "Level: " + _gameLevel;
                _timer.Start();
                startStopBtn.Content = "Stop Game";
                _gameActive = true;
            }
            else
            {
                _timer.Stop();
                startStopBtn.Content = "Start Game";
            }
        }
                      
        // Add new shape Shape in grid
        private void AddShape(int shapeNumber,int left=0,int down=0)
        {           
            // Remove previous position of Shape
            RemoveShape();
            _currentShapeRow = new List<int>();
            _currentShapeColumn = new List<int>();
            if (!_isRotated)
            {
                _currentShape = null;
                _currentShape = GetVariableByString(_arrayShapes[shapeNumber] );
            }
            var firstDim = _currentShape.GetLength(0);
            var secondDim = _currentShape.GetLength(1);
            _currentShapeWidth = secondDim;
            _currentShapeHeigth = firstDim;

            // This is only for I Shape
            if (_currentShape == IShape90)
            {
             _currentShapeWidth = 1;
            }
            else if (_currentShape == IShape0) { _currentShapeHeigth = 1; }
            //------------------------------------
            for (var row=0;row < firstDim;row++)
            {
                for (var column=0; column < secondDim; column++)
                {
                    var bit = _currentShape[row, column];
                    if (bit == 1 )
                    {
                        var square=GetBasicSquare(_shapeColor[shapeNumber - 1]);
                        tetrisGrid.Children.Add(square);
                        square.Name = "moving_" + Grid.GetRow(square)+"_"+Grid.GetColumn(square);
                        if ( down >= tetrisGrid.RowDefinitions.Count- _currentShapeHeigth)
                        {
                           down = tetrisGrid.RowDefinitions.Count - _currentShapeHeigth;
                        }
                        Grid.SetRow(square, _rowCount + down);
                        Grid.SetColumn(square, _columnCount + left);
                        _currentShapeRow.Add(_rowCount + down);
                        _currentShapeColumn.Add(_columnCount + left);
                     
                    }
                    _columnCount++;
                }
                _columnCount = 0;
                _rowCount++;
            }
            _columnCount = 0;
            _rowCount = 0;
            if (!_nextShapeDrawed)
            {
                DrawNextShape(_nextShapeNumber);
            }
        }

        // Add new shape in new location
        private void MoveShape()
        {
            _leftCollided = false;
            _rightCollided = false;

            // Check if collided
            TetroCollided(); 
            if (_leftPos > (_tetrisGridColumn - _currentShapeWidth))
            {
                _leftPos = (_tetrisGridColumn - _currentShapeWidth);             
            }
            else if (_leftPos < 0) { _leftPos = 0; }

            if (_bottomCollided) 
            {           
                ShapeStop();
                return;
            }
            AddShape(_currentShapeNumber, _leftPos, _downPos);
        }

        // Check collided if rotated Shape 
        private bool RotationCollided(int rotation)
        {
           if (   CheckCollided(0, _currentShapeWidth - 1))   { return true; }//Bottom collision 

           if (CheckCollided(0, - (_currentShapeWidth - 1)) ) { return true; }// Top collision

           if (CheckCollided(0, -1)) { return true; }// Top collision

           if (CheckCollided(-1, _currentShapeWidth - 1)){ return true; }// Left collision

           if (CheckCollided(1, _currentShapeWidth - 1)) { return true; }// Right collision
           return false;
        }
        
        // Check if collided in sides , bottom and other shapes 
        private void TetroCollided()
        {
            _bottomCollided = CheckCollided(0, 1);
            _leftCollided = CheckCollided(-1, 0);
            _rightCollided = CheckCollided(1, 0);
        }

        //Check collided
        private bool CheckCollided(int leftRightOffset, int bottomOffset)
        {
            var squareRow = 0;
            var squareColumn = 0;
            for (var i = 0; i <= 3; i++)
            {
                squareRow = _currentShapeRow[i];
                squareColumn = _currentShapeColumn[i];
                try
                {
                    var movingSquare = (Rectangle)tetrisGrid.Children
                        .Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == squareRow + bottomOffset && Grid.GetColumn(e) == squareColumn+leftRightOffset);
                    if (movingSquare != null)
                    {
                        if (movingSquare.Name.IndexOf("arrived") == 0)
                        {
                           return true;
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return _downPos > (_tetrisGridRow - _currentShapeHeigth);
        }
       
        // Draw next shape Shape in nextShapeCanvas 
        private void DrawNextShape(int shapeNumber)
        {
            nextShapeCanvas.Children.Clear();
            var nextShapeShape = GetVariableByString(_arrayShapes[shapeNumber]);
            var firstDim = nextShapeShape.GetLength(0);
            var secondDim = nextShapeShape.GetLength(1);
            var x = 0;
            var y = 0;
            for (var row = 0; row < firstDim; row++)
            {                
                for (var column = 0; column < secondDim; column++)
                {
                    var bit = nextShapeShape[row, column];
                    if (bit == 1)
                    {
                        var square = GetBasicSquare(_shapeColor[shapeNumber-1]);
                        nextShapeCanvas.Children.Add(square);
                        Canvas.SetLeft(square, x);
                        Canvas.SetTop(square, y);
                    }
                    x += 25;
                }
                x = 0;
                y += 25;    
            }
            _nextShapeDrawed = true;
        }


        // This method called when shape it arrives at the bottom or collided
       private void ShapeStop()
       {
           _timer.Stop();
            PlaySound(0);
            // Game over condition
            if (_downPos <= 2)
            {                          
               GameOver();
               return;
            }
            
            var index = 0;
            while (index < tetrisGrid.Children.Count)
            {
                var element = tetrisGrid.Children[index];
                if (element is Rectangle square)
                {
                    if (square.Name.IndexOf("moving_", StringComparison.Ordinal) == 0)
                    {
                        // Replace the name of squares arrived Shape
                        var newName= square.Name.Replace("moving_", "arrived_");
                        square.Name=newName;
                    }
                }
                index++;
            }
            // Check if line  is complete  and descend down the other shapes
            CheckComplete();
            Reset();
            _timer.Start();
         
        }
        // Method for check if complete line
        private void CheckComplete()
        {
            var gridRow = tetrisGrid.RowDefinitions.Count;
            var gridColumn = tetrisGrid.ColumnDefinitions.Count;
            for (var row = gridRow; row >= 0; row--)
            {
                var squareCount = 0;
                for (var column = gridColumn; column >= 0; column--)
                {
                    var square = (Rectangle) tetrisGrid.Children
                        .Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == column);
                    if (square == null) continue;
                    if (square.Name.IndexOf("arrived", StringComparison.Ordinal) == 0)
                    {
                        squareCount++;
                    }
                }

                // If squareCount == gridColumn this means tha the line is completed and must to be delete
                if (squareCount != gridColumn) continue;

                PlaySound(1);
                DeleteLine(row);
                scoreTxt.Text = AddScore().ToString();
                CheckComplete();
            }
        }
       
        // Delete complete square line
        private void DeleteLine(int row)
        {
            // Delete complete line
            for(var i=0;i<tetrisGrid.ColumnDefinitions.Count;i++)
            {
                try
                {
                    var square = (Rectangle)tetrisGrid.Children
                        .Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == i);
                    tetrisGrid.Children.Remove(square);
                }
                catch
                {
                    // ignored
                }
            }
            // Move down the rest shape
            foreach (UIElement element in tetrisGrid.Children)
            {
                var square = (Rectangle)element;
                if(square.Name.IndexOf("arrived", StringComparison.Ordinal)==0 && Grid.GetRow(square)<=row)
                {
                  Grid.SetRow(square, Grid.GetRow(square) + 1);
                }
            }
        }
        // Get the score
        private int AddScore()
        {
            _gameScore += 50 * _gameLevel;
            return _gameScore;
        }

        // Some reset
        private void Reset()
        {
            _downPos = 0;
            _leftPos = 3;
            _isRotated = false;
            _rotation = 0;
            _currentShapeNumber = _nextShapeNumber;
            if (!_isGameOver) { AddShape(_currentShapeNumber, _leftPos); }
            _nextShapeDrawed = false;
            _shapeRandom = new Random();
            _nextShapeNumber = _shapeRandom.Next(1, 8);
            _bottomCollided = false;
            _leftCollided = false;
            _rightCollided = false;
        }
        // The game over reset
        private void GameOver()
        {         
          _isGameOver = true;
          Reset();
          startStopBtn.Content = "Start Game";
          GameOverTxt.Visibility = Visibility.Visible;
          _rowCount = 0;
          _columnCount = 0;
          _leftPos = 0;
          _gameSpeedCounter = 0;
          _gameSpeed = Gamespeed;
          _gameLevel = 1;
          _gameActive = false;
          _gameScore = 0;
          _nextShapeDrawed = false;
          _currentShape = null;
          _currentShapeNumber = _shapeRandom.Next(1, 8);
          _nextShapeNumber = _shapeRandom.Next(1, 8);
          _timer.Interval = new TimeSpan(0, 0, 0, 0, _gameSpeed);
         
        }
        

        // Remove shape from grid   
        private void RemoveShape()
        {
            var index = 0; 
            while (index<tetrisGrid.Children.Count)
            {             
                var element =  tetrisGrid.Children[index];
                if (element is Rectangle square)
                {
                    if (square.Name.IndexOf("moving_", StringComparison.Ordinal) == 0)
                    {
                        tetrisGrid.Children.Remove(square);
                        index = -1;
                    } 
                } 
                index++;  
            }
          
        }

        // Created the basic square for tetris shape
        private Rectangle GetBasicSquare(Color rectColor)
        {
            var rectangle = new Rectangle
            {
                Width = 25,
                Height = 25,
                StrokeThickness = 1,
                Stroke = Brushes.White,
                Fill = GetGradientColor(rectColor)
            };
            return rectangle;
        }

        // Get the gradient color for basic square
        private static LinearGradientBrush GetGradientColor( Color clr)
        {
            var gradientColor = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1.5)
            };
            var black = new GradientStop
            {
                Color = Colors.Black,
                Offset = -1.5
            };
            gradientColor.GradientStops.Add(black);
            var other = new GradientStop
            {
                Color = clr,
                Offset = 0.70
            };
            gradientColor.GradientStops.Add(other);
            return gradientColor;
        }

        // Access variable by string name
        private int[,] GetVariableByString(string variable)
        {
            return (int[,])GetType().GetField(variable).GetValue(this);
        }
        // Play sound. index=0 is for collided.wav and index=1 for deleteLine.wav
        private void PlaySound(int index)
        {
         _soundList[index].Play();
        }

    
    }

    
}
