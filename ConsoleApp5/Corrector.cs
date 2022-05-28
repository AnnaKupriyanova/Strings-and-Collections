using System;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleApp5 {
  [Serializable]
  public class Сorrector {
    private Dictionary<string, List<string>> Dictionary { get; set; } = new Dictionary<string, List<string>>();
    private List<string> AddErrors(List<string> Errors) {
      string Word;
      Console.WriteLine("Add variations of errors. Enter 'stop' to stop.");

      while (true) {
        Word = Console.ReadLine();

        if (Word == "stop") { 
          break;        
        }

        Errors.Add(Word);     
      }
      return Errors;
    } 

    private void AddWord() { 
      Console.WriteLine("Enter word:");
      var NewWord = Console.ReadLine();

      if (Dictionary.ContainsKey(NewWord)) {
        throw new Exception("The word already added.")   ;   
      }

      var Errors = new List<string>();
      Dictionary.Add(NewWord, Errors);
      Dictionary[NewWord] = AddErrors(Dictionary[NewWord]);
      Console.WriteLine("New word added.");
    }

    private string Correction(string ThisWord) {
      foreach (var Word in Dictionary) {
        foreach (var Variant in Word.Value) { 
          if (ThisWord == Variant) {
            ThisWord = Word.Key;      
            return ThisWord;
          }         
        }
      }
      return ThisWord;
    }

    private string EditSetWords(string Set) {
      var Words = Set.Split();
      var EditedSet = "";
      foreach (var Word in Words) {
        EditedSet += $"{Correction(Word)} ";
      }
      EditedSet = $"{EditedSet}\n";
      return EditedSet;
    }
    
    private void EditWords(string FileName) { 
      var Edited = "";

      using (var TextFile = new StreamReader(FileName)) {
        string Text = "";

        while (!TextFile.EndOfStream) {
          Text = TextFile.ReadLine();
          Edited += EditSetWords(Text);
        }
      }

      using (var TextFile = new StreamWriter(FileName)) { 
        TextFile.WriteLine(Edited);
        TextFile.Flush();
      }

      Console.WriteLine("Congratulations! Editing finished.");
      Console.ReadKey();
    }

    private string EditSetNumbers(string Set) {
      string Number;
      var EditedSet = "";
      var RegexFind = new Regex(@"([(]\d{3}[)]\s{1}\d{3})-(\d{2})-(\d{2})");
      var Dash = new Regex(@"-");
      var BracketLeft = new Regex(@"[(]");
      var BracketRigth = new Regex(@"[)]");
      var Matches = RegexFind.Matches(Set);

      if (Matches.Count > 0) {
        foreach (Match Match in Matches) {
          Number = Match.Value;
          Number = Dash.Replace(Number, " ");
          Number = BracketLeft.Replace(Number, "");
          Number = BracketRigth.Replace(Number, "");
          Number = $"+380 {Number.Substring(1)}";
          Set = Set.Replace(Match.Value, Number);
        }     
      }

      EditedSet = $"{Set}\n";
      return EditedSet;
    }

    private void EditNumbers(string FileName) { 
      var Edited = "";

      using (var TextFile = new StreamReader(FileName)) {
        string Text = "";

        while (!TextFile.EndOfStream) {
          Text = TextFile.ReadLine();
          Edited += EditSetNumbers(Text);
        }
      }

      using (var TextFile = new StreamWriter(FileName)) { 
        TextFile.WriteLine(Edited);
        TextFile.Flush();
      }

      Console.WriteLine("Congratulations! Editing finished.");
      Console.ReadKey();
    }

    private void SerializeBinary(FileStream fileStream) {
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      binaryFormatter.Serialize(fileStream, this);
      fileStream.Close();
    }

    private void DeserializeBinary(FileStream fileStream) {
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      Сorrector deserialized = (Сorrector)binaryFormatter.Deserialize(fileStream);
      Dictionary = deserialized.Dictionary;
      fileStream.Close();
    }

    public void Menu() { 
      string Directory, Name;

      while (true) {
        Console.WriteLine("Enter directory name:");
        Directory = Console.ReadLine();
        Console.WriteLine("Enter file name:");
        Name = Console.ReadLine();
        Name = Directory + "/" + Name;

        if (File.Exists(Name)) {
          break;      
        } else { 
          Console.WriteLine("\nError.\n");
        }
      }

      var FileStream = new FileStream($"{Directory}/dictionary.bin", FileMode.OpenOrCreate, FileAccess.Read);
      
      if (FileStream.Length > 0) {
        DeserializeBinary(FileStream);      
      }

      FileStream.Close();

      while (true) {
        Console.Clear();
        Console.WriteLine("Add new word to dictionary  0");
        Console.WriteLine("Edit text                   1");
        Console.WriteLine("Edit number                 2");
        Console.WriteLine("Exit                        3");

        switch (Console.ReadLine()) {
          case "0":
            Console.Clear ();

            try { AddWord(); }
            catch ( Exception Exception ) { 
              Console.WriteLine(Exception.Message);
              Console.ReadKey();
              break;
            }
                
            FileStream = new FileStream($"{Directory}/dictionary.bin", FileMode.Open, FileAccess.Write);
            SerializeBinary(FileStream);
            FileStream.Close();
            Console.ReadKey();
            break;

          case "1":
            Console.Clear ();
            EditWords(Name);
            break;

          case "2":
            Console.Clear ();
            EditNumbers(Name);
            break;

          case "3":
            return;

          default:
            break;
        }
      }
    }
  }
}