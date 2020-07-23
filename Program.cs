using System;
using System.IO;
using System.Collections.Generic;
//using System.Windows.Forms;

namespace CSharpStats
{
    class Dataset
    {
        private List<double> dArrL = new List<double>();
        private int numPts = 0;
        private double mean = 0.0;

        public Dataset()
        {
            dArrL.Clear();
            numPts = dArrL.Count;
        }

        public Dataset(List<Double> inArrL)
        {
            dArrL = inArrL;
            numPts = dArrL.Count;
        }

        public int GetNumPts()
        {
            return dArrL.Count;
        }

        public void ClearArray()
        {
            dArrL.Clear();
            numPts = dArrL.Count;
        }

        public List<double> GetArray()
        {
            return dArrL;
        }

        public int AddPoint(double x)
        {
            dArrL.Add(x);
            numPts = dArrL.Count;

            return numPts;
        }

        public int AddPoint(int x)
        {
            dArrL.Add(Convert.ToDouble(x));
            numPts = dArrL.Count;

            return numPts;
        }

        public double GetPoint(int i)
        {
            double x = 0.0;
            numPts = dArrL.Count;
            if (i >= 0 && i < numPts)
            {
                x = dArrL[i];
            }

            return x;
        }

        public double[] ComputeExtremes()
        {
            double[] extremes = {0.0, 0.0};
            numPts = dArrL.Count;
            
            if (numPts > 0)
            {
                double max = -1.0 * Double.PositiveInfinity;
                double min = Double.PositiveInfinity;
                double term;
                for (int i = 0; i < numPts; i++)
                {
                    term = dArrL[i];
                    if (term > max)
                    {
                        max = term;
                    }
                    if (term < min)
                    {
                        min = term;
                    }
                }
                extremes[0] = min;
                extremes[1] = max;
            }
            
            return extremes;
        }

        public double ComputeMean()
        {
            mean = 0.0;
            numPts = dArrL.Count;
            
            if (numPts > 0)    // only defined if numPts > 0
            {
                double sum = 0.0;
                for (int i = 0; i < numPts; i++)
                {
                    sum += dArrL[i];
                }
                mean = sum / (double) numPts;
            }
            
            return mean;    // returns 0 if numPts < 1
        }

        public double ComputeStdev()
        {
            double stdev = 0.0;
            numPts = dArrL.Count;
            
            if (numPts > 1)    // only defined if numPts > 1
            {
                double sum = 0.0;
                double term = 0.0;
                for (int i = 0; i < numPts; i++)
                {
                    term = dArrL[i] - mean;
                    sum += term * term;
                }
                stdev = Math.Sqrt(sum / (double) (numPts - 1));
            }

            return stdev;    // returns 0 if numPts < 2
        }

        public double ComputeMedian()
        {
            double median = 0.0;
            numPts = dArrL.Count;

            if (numPts > 1)    // median only defined for numPts > 1
            {            
                dArrL.Sort();    // first, sort the data points
                int m0 = numPts / 2;
                if (numPts % 2 == 0)    // if even number of pts, take average of the two middles
                {
                    median = (dArrL[m0 - 1] + dArrL[m0]) / 2.0;
                }
                else    // if odd number of pts, take the middle one
                {
                    median = dArrL[m0];
                }
                // for (int i = 0; i < numPts; i++)
                // {
                //     Console.WriteLine("Sorted data point {0}: {1:0.00}", i + 1, dArrL[i]);
                // }
            }
            
            return median;    // returns 0 if numPts < 2
        }
    }    // end of class Dataset

    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                string s = "";
                for (int i = 0; i < args.Length; i++)
                {
                    s += args[i];
                    s += ", ";
                }
                s = "command-line args: " + s;
                Console.WriteLine(s);
                //MessageBox.Show(s, "Info");
                
            }
            else
            {
                CSharpStatsConsoleApp();
            }
        }


        private static void CSharpStatsConsoleApp()
        {
            Dataset ds = new Dataset();

            Console.Write("Enter 1 for keyboard input, 2 for file input: ");
            string inStr = Console.ReadLine();
            int mode = Int32.Parse(inStr);
            switch (mode)
            {
                case 1:
                    ds = GetDataPointsFromConsole();
                    break;
                case 2:
                    ds = GetDataPointsFromFile();
                    PrintDataPoints(ds);
                    break;
                default:
                    Console.WriteLine("\nInvalid mode; exiting program.");
                    System.Environment.Exit(0);
                    break;
            }

            PrintOutStats(ds);
        }


        private static Dataset GetDataPointsFromConsole()
        {
            Dataset ds = new Dataset();
            int n = 0;
            int sLen;

            Console.WriteLine("\nWhen no more data is left to enter, simply hit return.");
            while (true)
            {
                Console.Write("Data point {0}: ", n+1);
                string s = Console.ReadLine();
                sLen = s.Length;
                if (sLen == 0)
                {
                    break;
                }
                double term = Double.Parse(s);
                ds.AddPoint(term);
                n++;
            }

            return ds;
        }


        private static Dataset GetDataPointsFromFile()
        {
            Dataset ds = new Dataset();
            int n = 0;

            Console.Write("\nEnter pathname of data file: ");
            string fName = Console.ReadLine();
            if (File.Exists(fName))
            {
                string[] lines = System.IO.File.ReadAllLines(fName);
                foreach(string line in lines)
                {
                    int sLen = line.Length;
                    if (sLen > 0)
                    {
                        double term = Double.Parse(line);
                        ds.AddPoint(term);
                        n++;
                        //Console.WriteLine("line {0}: {1}, length {2}", n, line, sLen);
                    }
                }
            }
            else
            {
                Console.WriteLine("File {0} does not exist.", fName);
            }

            return ds;
        }

        private static void PrintDataPoints(Dataset ds)
        {
            int n = ds.GetNumPts();
            
            if (n > 0)
            {
                if (n < 10)    // print all data points
                {
                    for (int i = 0; i < n; i++)
                    {
                        Console.WriteLine("Data point {0}: {1:0.00}", i + 1, ds.GetPoint(i));
                    }
                }
                else    // just print first and last five data points
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine("Data point {0}: {1:0.00}", i + 1, ds.GetPoint(i));
                    }
                    Console.WriteLine("     ... ");
                    for (int i = n - 5; i < n; i++)
                    {
                        Console.WriteLine("Data point {0}: {1:0.00}", i + 1, ds.GetPoint(i));
                    }                    
                }
            }
            
            return;
        }


        private static void PrintOutStats(Dataset ds)
        {
            int n = ds.GetNumPts();

            if (n > 0)
            {
                double[] minmax = ds.ComputeExtremes();
                double mean = ds.ComputeMean();
                Console.WriteLine("\nFor {0:F0} data points: ", n);
                Console.WriteLine("   the maximum is {0:0.00}", minmax[1]);
                Console.WriteLine("   the minimum is {0:0.00}", minmax[0]);
                Console.WriteLine("   the mean (average) is {0:0.00}", mean);
                if (n > 1)
                {
                    double med = ds.ComputeMedian();
                    Console.WriteLine("   the median is {0:0.00}", med);
                    double stdev = ds.ComputeStdev();
                    Console.WriteLine("   the std dev is {0:0.00}", stdev);
                }
            }
            else
            {
                Console.WriteLine("\nNo data points to be analyzed.");
            }

            return;
        }

    }
}


/*

EHKs-MacBook-Pro:CSharpStats ehk$ dotnet run
Enter 1 for keyboard input, 2 for file input: 1

When no more data is left to enter, simply hit return.
Data point 1: 7
Data point 2: 1
Data point 3: 6
Data point 4: 2
Data point 5: 5
Data point 6: 3
Data point 7: 4
Data point 8: 

For 7 data points: 
   the maximum is 7.00
   the minimum is 1.00
   the mean (average) is 4.00
   the median is 4.00
   the std dev is 2.16
EHKs-MacBook-Pro:CSharpStats ehk$ dotnet run
Enter 1 for keyboard input, 2 for file input: 1

When no more data is left to enter, simply hit return.
Data point 1: 6
Data point 2: 1
Data point 3: 5
Data point 4: 2
Data point 5: 4
Data point 6: 3
Data point 7: 

For 6 data points: 
   the maximum is 6.00
   the minimum is 1.00
   the mean (average) is 3.50
   the median is 3.50
   the std dev is 1.87
EHKs-MacBook-Pro:CSharpStats ehk$ dotnet run
Enter 1 for keyboard input, 2 for file input: 1

When no more data is left to enter, simply hit return.
Data point 1: 3.5
Data point 2: 

For 1 data points: 
   the maximum is 3.50
   the minimum is 3.50
   the mean (average) is 3.50
EHKs-MacBook-Pro:CSharpStats ehk$ dotnet run
Enter 1 for keyboard input, 2 for file input: 1

When no more data is left to enter, simply hit return.
Data point 1: 

No data points to be analyzed.
EHKs-MacBook-Pro:CSharpStats ehk$ dotnet run
Enter 1 for keyboard input, 2 for file input: 2

Enter pathname of data file: testdata1.txt
Data point 1: 60.00
Data point 2: 62.00
Data point 3: 57.00
Data point 4: 58.00
Data point 5: 68.00
     ... 
Data point 6: 65.00
Data point 7: 63.00
Data point 8: 59.00
Data point 9: 60.00
Data point 10: 58.00

For 10 data points: 
   the maximum is 68.00
   the minimum is 57.00
   the mean (average) is 61.00
   the median is 60.00
   the std dev is 3.50
EHKs-MacBook-Pro:CSharpStats ehk$ dotnet run
Enter 1 for keyboard input, 2 for file input: 2

Enter pathname of data file: testdata2.txt
Data point 1: 23.50

For 1 data points: 
   the maximum is 23.50
   the minimum is 23.50
   the mean (average) is 23.50
EHKs-MacBook-Pro:CSharpStats ehk$ dotnet run
Enter 1 for keyboard input, 2 for file input: 2

Enter pathname of data file: testdata3.txt

No data points to be analyzed.
EHKs-MacBook-Pro:CSharpStats ehk$ dotnet run
Enter 1 for keyboard input, 2 for file input: 2

Enter pathname of data file: testdata4.txt
File testdata4.txt does not exist.

No data points to be analyzed.
EHKs-MacBook-Pro:CSharpStats ehk$ 

*/



