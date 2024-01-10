using System;
using System.Diagnostics;

namespace writtenincs
{
	internal class Program
	{

		public static Semaphore ReaderCountLock = new(1, 1);
		public static Semaphore ActionLock = new(1, 1);
		public static Semaphore ReadAndWriteLock = new(1, 1);
		public static Semaphore PrintLock = new(0, 1);
		public static Semaphore WaitForParentLock = new(0, 1);
		public static Semaphore ActiveThreadCountLock = new(1, 1);

		public static int ActiveThreadCount = 0;
		public static int CoreCount = Environment.ProcessorCount;

		public static int ReaderCount = 0;

		public static Dataset? Data { get; private set; }
		public static int Time { get; private set; }
		public static Answer? Ans { get; set; }

		public static void Main()
		{

			Thread[] ThreadsArray = new Thread[CoreCount];

			SetFile();
			Data?.PrintDataset();
			PrintSeparator();
			SetTime();
			PrintSeparator();
			LaunchThreads(ref ThreadsArray);
			PrintSeparator();
			WaitForAnswers();
			PrintSeparator();
			WaitForThreads(ref ThreadsArray);
			PrintSeparator();
			Ans?.PrintAnswer();
			PrintSeparator();
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();

			Environment.Exit(0);
		}

		static void SetFile()
		{
			while (true)
			{
				try
				{
					Console.BackgroundColor = ConsoleColor.DarkBlue;
					Console.Write("Dataset file name?\t");
					Console.ResetColor();
					string? fileName = Console.ReadLine() ?? throw new Exception("File name cannot be empty!");
					Data = new Dataset("./dataset/" + fileName + ".txt");
					break;
				}
				catch (Exception e)
				{
					if (e is FileLoadException)
					{
						Console.BackgroundColor = ConsoleColor.Red;
						Console.Write("Error: File name is invalid!");
						Console.ResetColor();
						Console.WriteLine();
					}
					else
					{
						Console.BackgroundColor = ConsoleColor.Red;
						Console.Write($"Error: {e.Message}");
						Console.ResetColor();
						Console.WriteLine();
					}
				}
			}
		}

		static void SetTime()
		{
			while (true)
			{
				try
				{
					Console.BackgroundColor = ConsoleColor.DarkBlue;
					Console.Write("Time in seconds?\t");
					Console.ResetColor();
					string? t = Console.ReadLine() ?? throw new Exception("Time cannot be empty!");
					int t_int = Convert.ToInt16(t);
					if (t_int < 1)
					{
						throw new Exception("Time cannot be less than one second!");
					}
					Time = t_int;
					break;
				}
				catch (Exception e)
				{
					Console.BackgroundColor = ConsoleColor.Red;
					Console.Write($"Error: {e.Message}");
					Console.ResetColor();
					Console.WriteLine();
				}
			}

		}

		static void LaunchThreads(ref Thread[] t)
		{
			Console.BackgroundColor = ConsoleColor.Magenta;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write($"{CoreCount} CPU cores detected. Launching {CoreCount} threads...");
			Console.ResetColor();
			Console.WriteLine();
			for (int i = 0; i < t.Length; i++)
			{
				try
				{
					t[i] = new Thread(Child);
					t[i].Start();
					ActiveThreadCount++;
				}
				catch (Exception e)
				{
					Console.BackgroundColor = ConsoleColor.Red;
					Console.Write($"Error: Couldn't create thread! {e.Message}");
					Console.ResetColor();
					Console.WriteLine();
				}
			}
			Console.BackgroundColor = ConsoleColor.DarkMagenta;
			Console.Write($"{ActiveThreadCount} threads launched successfully!");
			Console.ResetColor();
			Console.WriteLine();
		}

		static void WaitForAnswers()
		{
			while (true)
			{
				PrintLock.WaitOne();
				ActiveThreadCountLock.WaitOne();
				if (ActiveThreadCount == 0)
				{
					break;
				}
				ActiveThreadCountLock.Release();
				Ans?.DeclareDiff();
				WaitForParentLock.Release();
			}
		}

		static void WaitForThreads(ref Thread[] t)
		{
			for (int i = 0; i < CoreCount; i++)
			{
				t[i].Join();
			}
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.Write("All threads finished execution.");
			Console.ResetColor();
			Console.WriteLine();
		}

		public static void PrintSeparator()
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write("------------------------------------------------");
			Console.ResetColor();
			Console.WriteLine();
		}

		static void Child()
		{
			if (Data is not null)
			{
				Stopwatch sw = new();
				sw.Start();
				Answer? localAns = null;
				while (sw.Elapsed.TotalSeconds < Time)
				{
					Answer newAns = new(Data, Environment.CurrentManagedThreadId);
					if (localAns is null || newAns.Diff < localAns.Diff)
					{
						localAns = newAns;
						bool isWriter = false;

						ActionLock.WaitOne();
						ReaderCountLock.WaitOne();
						ReaderCount++;
						if (ReaderCount == 1)
						{
							ReadAndWriteLock.WaitOne();
						}
						ReaderCountLock.Release();
						ActionLock.Release();

						if (Ans is null || localAns.Diff < Ans?.Diff)
						{
							isWriter = true;
						}

						ReaderCountLock.WaitOne();
						ReaderCount--;
						if (ReaderCount == 0)
						{
							ReadAndWriteLock.Release();
						}
						ReaderCountLock.Release();

						if (isWriter)
						{
							ActionLock.WaitOne();
							ReadAndWriteLock.WaitOne();
							ActionLock.Release();
							if (Ans is null || localAns.Diff < Ans?.Diff)
							{
								Ans = new Answer(localAns);
								PrintLock.Release();
								WaitForParentLock.WaitOne();
							}
							ReadAndWriteLock.Release();
						}
					}
				}
				sw.Stop();

				ActiveThreadCountLock.WaitOne();
				ActiveThreadCount--;
				if (ActiveThreadCount == 0)
				{
					PrintLock.Release();
				}
				ActiveThreadCountLock.Release();
			}
			else
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.Write("Error: data object not constructed correctly!");
				Console.ResetColor();
				Console.WriteLine();
				Environment.Exit(-1);
			}
		}

	}
}

