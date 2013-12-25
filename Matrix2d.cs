using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
Solutions for finding a value in a 2d Matrix
1: linear search O(n^2)
2: binary search O(nlgn)
3: left down O(n)
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

            //todo: implement left down approach
            Test2Matrix(matrix2d);

           
        }

        static void Test2Matrix(int [][] matrix2d)
        {
            Console.WriteLine("Test Iteretive Binary search Matrix find");
			Console.WriteLine("{0}, {1}, {2},{3}, {4}, {5}, {6}, {7}",
				FindInIterativeBinarySorted2Matrix(matrix2d, 10),
				FindInIterativeBinarySorted2Matrix(matrix2d, 83),
				FindInIterativeBinarySorted2Matrix(matrix2d, 120),
				FindInIterativeBinarySorted2Matrix(matrix2d, 87),
				FindInIterativeBinarySorted2Matrix(matrix2d, 77),
				FindInIterativeBinarySorted2Matrix(matrix2d, 96),
				FindInIterativeBinarySorted2Matrix(matrix2d, 28),
				FindInIterativeBinarySorted2Matrix(matrix2d, 110));

			Console.WriteLine ();
			Console.WriteLine("Test Recursive Binary search Matrix find");
			Console.WriteLine("does 10 exists = {0}, does 95 exists = {1}, does 59 exists = {2}, does 87 exists = {3}",
				FindInRecursiveBinarySorted2Matrix(matrix2d, 10),
				FindInRecursiveBinarySorted2Matrix(matrix2d, 95),
				FindInRecursiveBinarySorted2Matrix(matrix2d, 59),
				FindInRecursiveBinarySorted2Matrix(matrix2d, 87));

			Console.WriteLine ();
			Console.WriteLine("Test Left Down  search Matrix find");
			Console.WriteLine("does 10 exists = {0}, does 95 exists = {1}, does 59 exists = {2}, does 87 exists = {3}",
				FindInLeftDownSorted2DMatrix(matrix2d, 10),
				FindInLeftDownSorted2DMatrix(matrix2d, 95),
				FindInLeftDownSorted2DMatrix(matrix2d, 59),
				FindInLeftDownSorted2DMatrix(matrix2d, 87));


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

		static bool FindInLeftDownSorted2DMatrix(int[][] a,int x){
		
			int n = 0; int m = a [n].Length - 1;

			do {
				if (a[n][m] == x)
					return true;
				else if (a[n][m] > x)
					m--;
				else if (a[n][m] < x)
					n++;

			} while(n < a.Length && m >= 0);

			return false;
		}


        static bool FindInIterativeBinarySorted2Matrix(int[][] a, int x)
        {
            for (int n = 0; n < a.Length; n++)
				if (IterativeBinarySearch(a[n], 0, a[n].Length, x)) return true;

            return false;
        }

        static bool FindInRecursiveBinarySorted2Matrix(int[][] a, int x)
        {
            for (int n = 0; n < a.Length; n++)
				if (RecursiveBinarySearch(a[n], 0, a[n].Length, x)) return true;
        
            return false;
        }

        static bool IterativeBinarySearch(int[] a, int start, int end, int x)
        {
			if (x > a[end - 1 ] || x < a[start]) return false;

            int partition = start + ((end - start) / 2);

			do {
					if (a [partition] == x)
						return true;

					if (start == partition || end == partition)
						return false;
					else if (a [partition] > x)
						end = partition;
					else if (a [partition] < x)
						start = partition;

					partition = start + ((end - start) / 2);

				} while(partition != end || partition != start);

            return false;
        }
        static bool RecursiveBinarySearch(int[] a, int start, int end, int x)
        {

			if (x > a[end - 1] || x < a[start]) return false;

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
