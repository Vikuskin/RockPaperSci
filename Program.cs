using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Linq;
using ConsoleTables;
using System.Data;
using System.Data.Common;

namespace task3_game
{
  class Program
  {
    public static void Main(string[] args)
    {
    Repeat:
      Console.WriteLine("What is your steps?");
      string s = Console.ReadLine();
      string[] steps = s.Split(' ');
      if (steps.Length < 3)
      {
        Console.WriteLine("Error: count of steps should be more 3");
        Console.WriteLine("\nДля повтора наберите Yes, для выхода No");
        string choice = Console.ReadLine();
        if (choice == "Yes" | choice == "yes")
          goto Repeat;
        if (choice == "No" | choice == "no")
          return;
      }
      if (steps.Length % 2 == 0)
      {
        Console.WriteLine("Error: count fo steps should be odd numbers");
        Console.WriteLine("\nДля повтора наберите Yes, для выхода No");
        string choice = Console.ReadLine();
        if (choice == "Yes" | choice == "yes") goto Repeat;
        if (choice == "No" | choice == "no") return;
      }
      string[] copySteps = steps.Clone() as string[];
      Array.Sort(copySteps);
      for (int i = 1; i < copySteps.Length; i++)
      {
        if (copySteps[i] == copySteps[i - 1])
        {
          Console.WriteLine($"Error: {copySteps[i]} repeat");
          goto Repeat;
        }
      }
      //получили арг

      while (true)
      {
        Random rnd = new Random();
        var stepComputer = rnd.Next(1, steps.Length);
        string strStepComputer = stepComputer.ToString();
        int[] arrStepComputer = new int[strStepComputer.Length];
        for (int i = 0; i < strStepComputer.Length; i++)
        {
          arrStepComputer[i] = int.Parse(strStepComputer);
        }
        //rnd stepcomp

        var generationKey = new GenerationKey(steps, arrStepComputer);
        byte[] key = generationKey.Hash(steps, arrStepComputer);

      //key
      Repeat2:
        int counter = 1;
        foreach (var step in steps)
        {
          Console.WriteLine($"{counter}-{step}");
          counter++;
        }
        Console.WriteLine("0-exit");
        Console.WriteLine("?-help");
        //menu

        string menu = Console.ReadLine();
        var table = new TableASCII(steps);
        if (menu == "?")
        {
          table.Table(steps);
          goto Repeat2;
        }
        else
        {
          int stepPlayer = Convert.ToInt32(menu);
          var rulesGame = new RulesGame(stepComputer, stepPlayer, key); // Обращаемся к другому классу, делая его дубль
          rulesGame.Rules(stepComputer, stepPlayer, key);
        }
      }
    }
  }
}
public class GenerationKey
{
  public GenerationKey(string[] steps, int[] arrStepComputer) { }
  public byte[] Hash(string[] steps, int[] arrStepComputer)
  {
    var hash = new SHA256Managed();
    byte[] key = new byte[128];
    RandomNumberGenerator.Create().GetBytes(key);
    byte[] innerData = new byte[key.Length + arrStepComputer.Length];
    Buffer.BlockCopy(key, 0, innerData, 0, key.Length);
    Buffer.BlockCopy(arrStepComputer, 0, innerData, key.Length, arrStepComputer.Length);
    byte[] innerHash = hash.ComputeHash(innerData);
    var result = BitConverter.ToString(innerHash).Replace("-", "").ToLower();
    Console.WriteLine($"HMAC: {result}");
    return key;
  }
}

public class RulesGame
{
  public RulesGame(int stepComputer, int stepPlayer, byte[] key) { }
  public void Rules(int stepComputer, int stepPlayer, byte[] key)
  {

    if (stepPlayer == 0)
      Environment.Exit(0);
    if (stepComputer == stepPlayer)
      Console.WriteLine("Nobody win \n");
    else if ((stepComputer + stepPlayer) % 2 == 0 && (stepComputer > stepPlayer) || ((stepComputer + stepPlayer) % 2 != 0) && (stepComputer < stepPlayer))
      Console.WriteLine("you win \n");
    else
      Console.WriteLine("you lose");
    Console.WriteLine("key: ");
    foreach (int number in key)
    {
      Console.Write(number);
    }
    Console.WriteLine(" ");
  }
}

public class TableASCII
{
  public TableASCII(string[] steps) { }

  public void Table(string[] steps)
  {
    var table = new ConsoleTable($"PC / User");
    for (int k = 0; k < steps.Length; k++)
    {
      var column = new DataColumn();
      column.ColumnName = steps[k];
      table.Columns.Add(column);
    }
    string[] resultGame = new string[steps.Length];
    string str = "draw win lose";
    resultGame = str.Split(' ');
    int count = 0;
    for (int w = 0; w < steps.Length; w++)
    {
      string[] result = new string[steps.Length];
      result[0] = resultGame[0];

      for (int q = 1; q < result.Length; q++)
      {
        int counter = result.Length / 2 + 1;
        if (q < counter && q != counter)
        {
          result[q] = resultGame[1];
        }
        else
        {
          result[q] = resultGame[2];
        }
      }

      var offset = count % result.Length;

      Array.Reverse(result, 0, result.Length);
      Array.Reverse(result, 0, offset);
      Array.Reverse(result, offset, result.Length - offset);

      string[] resultFinal = new string[result.Length + 1];
      resultFinal[0] = steps[w];
      for (int i = 0; i < result.Length; i++)
        resultFinal[i + 1] = result[i];

      count++;
      table.AddRow(resultFinal);
    }

    table.Write();
    Console.WriteLine();
  }
}