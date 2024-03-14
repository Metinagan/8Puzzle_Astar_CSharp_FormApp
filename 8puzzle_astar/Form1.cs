using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _8puzzle_astar
{
    public partial class Form1 : Form
    {
        static List<int[,]> mainList = new List<int[,]>();
        static List<int[]> neighborsList = new List<int[]>();
        static List<int[,]> posList = new List<int[,]>();

        static int[,] startPos = new int[,] { 
                                            { 1, 2, 3 }, 
                                            { 0, 4, 5 },    
                                            { 7, 8, 6 } };
        
        static int[,] endPos = new int[,] { { 1, 2, 3 }, 
                                            { 4, 5, 6 }, 
                                            { 7, 8, 0 } };



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mainList.Add(startPos);
            PrintMatrix(startPos);
        }
        static int Hamming(int[,] matrix)
        {
            int hammingValue = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (matrix[x, y] != endPos[x, y])
                    {
                        hammingValue++;
                    }
                }
            }
            return hammingValue;
        }
        static int Mannathen(int[,] matrix)
        {
            int mannathenValue = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (matrix[x, y] != 0)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (matrix[x, y] == endPos[k, l])
                                {
                                    int value = Math.Abs(l - y) + Math.Abs(k - x);
                                    mannathenValue += value;
                                }
                            }
                        }
                    }
                }
            }
            return mannathenValue;
        }
        static int Heuristic(int[,] matrix)
        {
            return Hamming(matrix) + Mannathen(matrix);
        }

        static Tuple<int, int> FindBlanks(int[,] matrix)
        {
            int zerorow = 0;
            int zerocol = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (matrix[x, y] == 0)
                    {
                        zerorow = x;
                        zerocol = y;
                    }
                }
            }
            return Tuple.Create(zerorow, zerocol);
        }
        static void Neighbors(int[,] matrix)
        {
            neighborsList.Clear();
            var blankLocation = FindBlanks(matrix);
            int row = blankLocation.Item1;
            int col = blankLocation.Item2;

            if (row > 0)
            {
                neighborsList.Add(new int[] { row - 1, col });
            }
            if (row < 2)
            {
                neighborsList.Add(new int[] { row + 1, col });
            }
            if (col > 0)
            {
                neighborsList.Add(new int[] { row, col - 1 });
            }
            if (col < 2)
            {
                neighborsList.Add(new int[] { row, col + 1 });
            }
        }

        static void ChangeBlanks(int[,] matrix, int row, int col)
        {
            var blankLocation = FindBlanks(matrix);
            int zeroRow = blankLocation.Item1;
            int zeroCol = blankLocation.Item2;

            int newNumber = matrix[row, col];
            int[,] newNode = (int[,])matrix.Clone();
            newNode[zeroRow, zeroCol] = newNumber;
            newNode[row, col] = 0;

            if (!IsMatrixInList(mainList, newNode))
            {
                posList.Add(newNode);
            }
        }
        void changebutontex(int value,int ind)
        {
            Button button = (Button)panel1.Controls[ind];
            button.Text=value.ToString();
        }



        async Task PrintMatrix(int[,]matrix)
        {
            int buttonindex = 1;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    string buttonName = "button" + buttonindex;
                    Button foundbutton = panel1.Controls.Find(buttonName, true).FirstOrDefault() as Button;
                    if (foundbutton != null)
                    {
                        int value = matrix[i, j];
                        foundbutton.Text = " ";
                        foundbutton.Text = value.ToString();
                    }
                    buttonindex++;
                }
            }
            await Task.Delay(1500);
        }
        static void CreateNeighbors(int[,] matrix)
        {
            neighborsList.Clear();
            int[,] copyMatrix = (int[,])matrix.Clone();
            Neighbors(copyMatrix);

            foreach (var neighbor in neighborsList)
            {
                ChangeBlanks(copyMatrix, neighbor[0], neighbor[1]);
            }
        }
        async Task AStarAsync(int[,] matrix)
        {
            posList.Clear();
            neighborsList.Clear();
            List<int> heuristicList = new List<int>();
            int nowheu=Heuristic(matrix);
            button13.Text = nowheu.ToString();
            await PrintMatrix(matrix);
            if (MatrixEquals(matrix, endPos))
            {
                Console.WriteLine("bitti");
            }
            else
            {
                int[,] copyMatrix = (int[,])matrix.Clone();
                CreateNeighbors(copyMatrix);

                foreach (var newPos in posList)
                {
                    if (!IsMatrixInList(mainList, newPos))
                    {
                        heuristicList.Add(Heuristic(newPos));
                    }
                }
                int minHeuristic = heuristicList.Min();
                
                int step = mainList.Count;
                button11.Text = step.ToString();

                int mindidex= heuristicList.IndexOf(minHeuristic);
                mainList.Add(posList[mindidex]);

                
                
                AStarAsync(posList[mindidex]);
                
            }
            
        }

        static bool IsMatrixInList(List<int[,]> matrixList, int[,] matrix)
        {
            foreach (var item in matrixList)
            {
                if (MatrixEquals(item, matrix))
                {
                    return true;
                }
            }
            return false;
        }

        static bool MatrixEquals(int[,] matrix1, int[,] matrix2)
        {
            if (matrix1.GetLength(0) != matrix2.GetLength(0) || matrix1.GetLength(1) != matrix2.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix1.GetLength(1); j++)
                {
                    if (matrix1[i, j] != matrix2[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            mainList.Clear();
            mainList.Add(startPos);
            int lenght =mainList.Count;
            AStarAsync(mainList[lenght-1]);
        }
    }
}
