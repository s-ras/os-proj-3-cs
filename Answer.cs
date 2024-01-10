
using writtenincs;

public class Answer
{
	public List<int> Values { get; private set; }
	private readonly Dataset refDataset;
	public Double Diff { get; private set; }
	public readonly int generatorId = 0;

	public Answer(Dataset ds, int genId = 0)
	{
		Values = [];
		refDataset = ds;
		generatorId = genId;
		GenerateNewAnswerSet();
		CalculateDiff();
	}

	public Answer(Answer a)
	{
		Values = [];
		Values.AddRange(a.Values);
		refDataset = a.refDataset;
		generatorId = a.generatorId;
		Diff = a.Diff;
	}

	private void GenerateNewAnswerSet()
	{
		Random rand = new Random();
		for (int i = 0; i < refDataset.Data.Count; i++)
		{
			int newNum = rand.Next(5);
			switch (newNum)
			{
				case 0:
				case 1:
					{
						Values.Add(0);
						break;
					}
				case 2:
				case 3:
					{
						Values.Add(1);
						break;
					}
				case 4:
					{
						Values.Add(2);
						break;
					}
				default:
					break;
			}
		}
	}

	private void CalculateDiff()
	{
		Double c1 = 0, c2 = 0, c3 = 0;
		for (int i = 0; i < Values.Count; i++)
		{
			switch (Values[i])
			{
				case 0:
					{
						c1 += refDataset.Data[i];
						break;
					}
				case 1:
					{
						c2 += refDataset.Data[i];
						break;
					}
				case 2:
					{
						c3 += refDataset.Data[i];
						break;
					}
				default:
					break;
			}
		}

		Double diff = Math.Abs(refDataset.Optimal[0] - c1) + Math.Abs(refDataset.Optimal[1] - c2) + Math.Abs(refDataset.Optimal[2] - c3);

		this.Diff = diff;
	}

	public void DeclareDiff()
	{
		Console.BackgroundColor = ConsoleColor.Yellow;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.Write($"-> {generatorId} generated a new answer set with diff {Diff}.");
		Console.ResetColor();
		Console.WriteLine();
	}

	public void PrintAnswer()
	{
		List<Double> c1 = [];
		List<Double> c2 = [];
		List<Double> c3 = [];
		for (int i = 0; i < Values.Count; i++)
		{
			switch (Values[i])
			{
				case 0:
					{
						c1.Add(refDataset.Data[i]);
						break;
					}
				case 1:
					{
						c2.Add(refDataset.Data[i]);
						break;
					}
				case 2:
					{
						c3.Add(refDataset.Data[i]);
						break;
					}
				default:
					break;
			}
		}

		Console.BackgroundColor = ConsoleColor.Blue;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.Write($"Thread ID = {generatorId}");
		Console.ResetColor();
		Console.WriteLine();
		Console.BackgroundColor = ConsoleColor.DarkBlue;
		Console.Write($"Diff = {Diff}");
		Console.ResetColor();
		Console.WriteLine();

		Program.PrintSeparator();

		Console.BackgroundColor = ConsoleColor.Cyan;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.Write("First Child:");
		Console.ResetColor();
		Console.WriteLine();
		for (int i = 0; i < c1.Count; i++)
		{
			Console.Write(c1[i] + " ");
		}
		Console.WriteLine();

		Console.BackgroundColor = ConsoleColor.Cyan;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.Write("Second Child:");
		Console.ResetColor();
		Console.WriteLine();
		for (int i = 0; i < c2.Count; i++)
		{
			Console.Write(c2[i] + " ");
		}
		Console.WriteLine();

		Console.BackgroundColor = ConsoleColor.Cyan;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.Write("Third Child:");
		Console.ResetColor();
		Console.WriteLine();
		for (int i = 0; i < c3.Count; i++)
		{
			Console.Write(c3[i] + " ");
		}
		Console.WriteLine();

	}
}