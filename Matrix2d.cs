using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
Solutions for finding a value in a 2d Matrix

*/
namespace ConsoleAlgorithmsPrep
{
    class Matrix2D
    {
        static void Main(string[] args)
        {

            int[][] matrix2d = new int[][]
            { 
                new int [] {1,2,7,11,13,21,30,68,83},
                new int [] {2,5,8,12,14,23,35,77,86}, 
                new int [] {3,6,9,15,16,24,40,85,95}, 
                new int [] {10,13,18,30,25,45,90,102,110},
                new int [] {18,21,22,45,49,50,100,120,123},
                new int [] {22,26,28,49,54,59,110,140,149}
            };


            //solutions
            //1: linear search O(n^2)
            //2: binary search O(nlgn)
            //3: left down O(n)

            Test2Matrix(matrix2d);

           
        }

        static void Test2Matrix(int [][] matrix2d)
        {
            Console.WriteLine("Test Iteretive Binary search Matrix find");
            Console.WriteLine("does 90 exists = {0}, does 11 exists = {1}, does 110 exists = {2}, does 105 exists = {3}",
                FindInIterativeBinarySorted2Matrix(matrix2d, 90),
                FindInIterativeBinarySorted2Matrix(matrix2d, 11),
                FindInIterativeBinarySorted2Matrix(matrix2d, 110),
                FindInIterativeBinarySorted2Matrix(matrix2d, 105));


            Console.WriteLine("Test Recursive Binary search Matrix find");
            Console.WriteLine("does 90 exists = {0}, does 11 exists = {1}, does 110 exists = {2}, does 105 exists = {3}",
             FindInIterativeBinarySorted2Matrix(matrix2d, 90),
             FindInIterativeBinarySorted2Matrix(matrix2d, 11),
             FindInIterativeBinarySorted2Matrix(matrix2d, 110),
             FindInIterativeBinarySorted2Matrix(matrix2d, 105));

        }

        static void TestBinarySearch()
        {
            int[] b = new int[] { 22, 26, 28, 49, 54, 59, 110, 140, 149, 345, 969, 1245, 3456, 4444, 5555, 6666, 8677, 9999, 10000, 20000, 30000 };

            Console.WriteLine("Test Iteretive Binary search");
            Console.WriteLine("140 exists = {0}, 555 exists {1}, 4444 exists {2}, 8677 exists {3}, 8679 exists {4}",
                RecursiveBinarySearch(b, 0, b.Length - 1, 140),
                RecursiveBinarySearch(b, 0, b.Length - 1, 555),
                RecursiveBinarySearch(b, 0, b.Length - 1, 4444),
                RecursiveBinarySearch(b, 0, b.Length - 1, 8677),
                RecursiveBinarySearch(b, 0, b.Length - 1, 8679));


            Console.WriteLine("Test Recursive Binary search");
            Console.WriteLine("140 exists = {0}, 555 exists {1}, 4444 exists {2}, 8677 exists {3}, 8679 exists {4}",
                 IterativeBinarySearch(b, 0, b.Length - 1, 140),
                 IterativeBinarySearch(b, 0, b.Length - 1, 555),
                 IterativeBinarySearch(b, 0, b.Length - 1, 4444),
                 IterativeBinarySearch(b, 0, b.Length - 1, 8677),
                 IterativeBinarySearch(b, 0, b.Length - 1, 8679));

        }


        static bool FindInIterativeBinarySorted2Matrix(int[][] a, int x)
        {
            for (int n = 0; n < a.Length; n++)
                if (IterativeBinarySearch(a[n], 0, a[n].Length - 1, x)) return true;

            return false;
        }

        static bool FindInRecursiveBinarySorted2Matrix(int[][] a, int x)
        {
            for (int n = 0; n < a.Length; n++)
                if (RecursiveBinarySearch(a[n], 0, a.GetLength(1) - 1, x)) return true;
        
            return false;
        }

        static bool IterativeBinarySearch(int[] a, int start, int end, int x)
        {
            if (x > a[end] || x < a[start]) return false;

            int partition = start + ((end - start) / 2);
            while (partition != end && partition != start)
            {
                if (a[partition] == x) return true;

                if (start == partition || end == partition)
                    return false;
                else if (a[partition] > x)
                    end = partition;
                else if (a[partition] < x)
                    start = partition;

                partition = start + ((end - start) / 2);

            }

            return false;
        }
        static bool RecursiveBinarySearch(int[] a, int start, int end, int x)
        {

            if (x > a[end] || x < a[start]) return false;

            int partition = start + ((end - start) / 2);

            if (a[partition] == x)
                return true;
            else if (partition == start || partition == end)
                return false;
            else if (a[partition] > x)
                return RecursiveBinarySearch(a, 0, partition, x);
            else if (a[partition] < x)
                return RecursiveBinarySearch(a, partition, end, x);

            return false;

        }

    }
}
