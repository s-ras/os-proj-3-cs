using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

public class Dataset
{
	public List<Double> Data { get; private set; }
	public string FileName { get; private set; }
	public Double[] Optimal = new Double[3];
	public Dataset(string FileName)
	{
		Data = [];
		this.FileName = FileName;
		ValidateFile();
		ReadData();
		PrintSuccessMessage();
	}
	private void ValidateFile()
	{
		if (!File.Exists(FileName))
		{
			throw new FileLoadException();
		}
	}

	private void ReadData()
	{

		Double sum = 0;

		string[] lines = File.ReadAllLines(FileName);
		foreach (string line in lines)
		{
			try
			{
				Double num = Convert.ToDouble(line);
				sum += num;
				Data.Add(num);
			}
			catch
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.Write("Invalid line in database detected, dropping line!");
				Console.ResetColor();
				Console.WriteLine();
			}
		}

		Optimal[0] = Optimal[1] = sum * 2 / 5;
		Optimal[2] = sum * 1 / 5;

	}

	private void PrintSuccessMessage()
	{
		Console.BackgroundColor = ConsoleColor.Green;
		Console.ForegroundColor = ConsoleColor.Black;
		Console.Write($"Successfully read {Data.Count} numbers from {FileName}!");
		Console.ResetColor();
		Console.WriteLine();
	}

	public void PrintDataset()
	{
		foreach (Double num in Data)
		{
			Console.Write(num + " ");
		}
		Console.WriteLine();
	}

}