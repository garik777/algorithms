using System;

namespace AlgorithmsPractice
{
	public class SubArraySum
	{
		static int[] my_a = new int [] { 22, 26, 28, 49, 54, 59, 110,43,20,60,21,98,2,5,7,1,39,86,54,77,123,140,149 };

		static bool IsSubArraySum(int sum){
		
			int n = 0, start = 0, running_sum = 0;
			while (start <= n ) {

				if (running_sum == sum)
					return true;
				else if (running_sum > sum) {
					running_sum -= my_a [start];
					start++;
				} else {
					running_sum += my_a[n];
					n++;
				}
			}

			return false;
		}

		static public void TestIsSubArraySum(){ 
			Console.WriteLine ("is subarrray 147 = {0}", IsSubArraySum (147));
			Console.WriteLine ("is subarrray 489 = {0}", IsSubArraySum (489));
			Console.WriteLine ("is subarrray 15 = {0}", IsSubArraySum (15));
			Console.WriteLine ("is subarrray 48 = {0}", IsSubArraySum (48));
			Console.WriteLine ("is subarrray 173 = {0}", IsSubArraySum (173));
		}
	}
}

