using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using Youtube_Storage;

Console.OutputEncoding = System.Text.Encoding.Unicode;
Console.InputEncoding = System.Text.Encoding.Unicode;

Generic gen = new Generic();

//calculates the video time in seconds(colon seperated time)
int CalculateTime(string time)
{
    int finalTime = 0;
    int colonCount = 0;
    int currentNum = 0;

    foreach (char c in time)
    {
        if(c == ':')
        {
            colonCount++;
        }
    }

    foreach (char c in time)
    {
        if (c == ':')
        {
            finalTime += (int)(currentNum * (Math.Pow(60,colonCount)));
            colonCount--;
            currentNum = 0;
        }

        else
        {
            currentNum = gen.StrToInt($"{currentNum}{char.ToString(c)}");
        }
    }

    return finalTime + currentNum;
}

//Checks if the series or channel exists in their directories
bool CheckExists(string[] lines, string thing)
{
    bool exists = false;

    foreach (string line in lines)
    {
        if (line.ToLower() == thing.ToLower())
        {
            exists = true;
            break;
        }
    }

    return exists;
}

//Writes to the channel directory
void WritePeeps(string peeps)
{
    string contents = "";
    string[] lines = { "" };
    string path = gen.GetPath(Path.Combine("YoutubeStorage", "AAApeeps.txt"));

    if (gen.FindFile(Path.Combine("YoutubeStorage", "AAApeeps.txt")))
    {
        contents = File.ReadAllText(path);
        lines = File.ReadAllLines(path);
    }

    if (!CheckExists(lines,peeps))
    {
        contents += $"{peeps}";
        gen.WriteToFile(contents, "AAApeeps", "YoutubeStorage");
    }
}

//reads from the channel directory
void ReadPeeps()
{
    if (gen.FindFile(Path.Combine("YoutubeStorage", "AAApeeps.txt")))
    {
        Console.WriteLine(File.ReadAllText(gen.GetPath(Path.Combine("YoutubeStorage", "AAApeeps.txt"))));
    }
}

//prints a page for ReadPeepsPages and ReadSeriesPages
void PrintPage(int page, string[] lines)
{
    int pageStart = page * 10;
    int i = pageStart;

    while(i < pageStart + 10 && i < lines.Length)
    {
        Console.WriteLine($"{i - pageStart}:{lines[i]}");
        i++;
    }

    Console.WriteLine($"\nPage {page}");
}

//reads channels within the YoutubeStorage folder
string ReadPeepsPages()
{
    string choice = "";
    string[] lines = { };
    string[] searchLines = { };
    int page = 0;
    int keyNum = 0;
    ConsoleKeyInfo key;
    string keyChar;

    if (gen.FindFile(Path.Combine("YoutubeStorage", "AAApeeps.txt")))
    {
        lines = File.ReadAllLines(gen.GetPath(Path.Combine("YoutubeStorage", "AAApeeps.txt")));
        searchLines = lines;
    }

    while(true)
    {
        Console.Clear();
        PrintPage(page, searchLines);
        gen.Write("\nPress a number to select a channel. \npress the left and right arrow keys to change pages. \npress the up arrow key to search. \npress the down arrow key to type a new channel.");

        int pageStart = page * 10;

        key = Console.ReadKey();
        keyChar = char.ToString(key.KeyChar);
        keyNum = gen.StrToInt(keyChar);

        if(key.Key == ConsoleKey.DownArrow)
        {
            choice = Console.ReadLine();
        }

        else if (key.Key == ConsoleKey.LeftArrow)
        {
            page--;
        }

        else if(key.Key == ConsoleKey.RightArrow)
        {
            page++;
        }

        else if (key.Key == ConsoleKey.UpArrow)
        {
            searchLines = PageSearch(Console.ReadLine(), lines);
        }

        else if(keyChar == "q")
        {
            choice = "q";
        }

        else if(keyChar == "b")
        {
            choice = "b";
        }

        else if(gen.IsInt(keyChar) && searchLines.Length >= keyNum + 1)
        {
            choice = searchLines[pageStart + keyNum];
        }

        if (choice != "")
        {
            break;
        }

        if(page == -1) 
        {
            page = searchLines.Length / 10;
        }

        if (page > searchLines.Length / 10)
        {
            page = 0;
        }
    }

    return choice;
}

void DeletePeeps(string peeps)
{
    string contents = "";
    string[] lines = { "" };
    string path = gen.GetPath(Path.Combine("YoutubeStorage", "AAApeeps.txt"));
    var linesList = new List<string>();

    Console.Clear();

    if(!gen.Sure("Would you like to delete this channel as well?"))
    {
        return;
    }

    if (gen.FindFile(Path.Combine("YoutubeStorage", "AAApeeps.txt")))
    {
        lines = File.ReadAllLines(path);
    }

    if (CheckExists(lines, peeps))
    {
        foreach (string line in lines)
        {
            if (line.ToLower() == peeps.ToLower())
            {
                linesList = lines.ToList();
                linesList.Remove(line);
                lines = linesList.ToArray();
            }
        }

        foreach (string line in lines)
        {
            if (line != lines.Last())
            {
                contents += $"{line}\n";
            }
            else
            {
                contents += line;
            }
        }

        if (contents == "")
        {
            gen.DeleteFile(path);
        }

        else
        {
            gen.WriteToFile(contents, "AAApeeps", "YoutubeStorage");
        }
    }
}

//removes the link from the directory in the channel's folder
void DeleteSeries(string series,string peeps)
{
    string contents = "";
    string[] lines = { "" };
    string path = gen.GetPath(Path.Combine(Path.Combine("YoutubeStorage", peeps), "AAAseries.txt"));
    var linesList = new List<string>();

    if (gen.FindFile(Path.Combine(Path.Combine("YoutubeStorage", peeps), "AAAseries.txt")))
    {
        lines = File.ReadAllLines(path);
    }

    if (CheckExists(lines, series))
    {
        foreach (string line in lines)
        {
            if (line.ToLower() == series.ToLower())
            {
                linesList = lines.ToList();
                linesList.Remove(line);
                lines = linesList.ToArray();
            }
        }

        foreach(string line in lines)
        {
            if(line != lines.Last())
            {
                contents += $"{line}\n";
            }
            else
            {
                contents+= line;
            }
        }

        if (contents == "")
        {
            DeletePeeps(peeps);
            gen.DeleteFile(path);
        }
        else
        {
            gen.WriteToFile(contents, "AAAseries", Path.Combine("YoutubeStorage", peeps));
        }
    }
}

//writes to the series directory within the channel's folder
void WriteSeries(string series, string peeps)
{
    string contents = "";
    string[] lines = { "" };
    string path = gen.GetPath(Path.Combine(Path.Combine("YoutubeStorage", peeps), "AAAseries.txt"));

    if (gen.FindFile(Path.Combine(Path.Combine("YoutubeStorage",peeps),"AAAseries.txt")))
    {
        contents = File.ReadAllText(path);
        lines = File.ReadAllLines(path);
    }

    if (!CheckExists(lines, series))
    {
        contents += $"{series}";
        gen.WriteToFile(contents, "AAAseries", Path.Combine("YoutubeStorage", peeps));
    }
}

//reads from the series directory within the channel's folder
void ReadSeries(string peeps)
{
    if (gen.FindFile(Path.Combine(Path.Combine("YoutubeStorage", peeps), "AAAseries.txt")))
    {
        Console.WriteLine(File.ReadAllText(gen.GetPath(Path.Combine(Path.Combine("YoutubeStorage", peeps), "AAAseries.txt"))));
    }
}

string[] PageSearch(string search, string[] lines)
{
    List<string> returnLines = new List<string>();
    string[] returnArray = { };
    string lineLower;

    search = search.ToLower();

    if(search == "")
    {
        return lines;
    }

    foreach(string line in lines) 
    {
        int i = 0;
        bool add = false;

        lineLower = line.ToLower();

        foreach (char c in lineLower) 
        {
            if (c == search[i])
            {
                i++;
            }

            if(i == search.Length)
            {
                add = true;
                break;
            }
        }

        if (add)
        {
            returnLines.Add(line);
        }
    }
    
    returnArray = returnLines.ToArray();

    return returnArray;
}

//reads the series from the directory within the channel's folder in pages
string ReadSeriesPages(string peeps)
{
    string choice = "";
    string[] lines = { };
    string[] searchLines = { };
    int page = 0;
    int keyNum = 0;
    ConsoleKeyInfo key;
    string keyChar;

    if (gen.FindFile(Path.Combine(Path.Combine("YoutubeStorage", peeps), "AAAseries.txt")))
    {
        lines = File.ReadAllLines(gen.GetPath(Path.Combine(Path.Combine("YoutubeStorage", peeps), "AAAseries.txt")));
        searchLines = lines;
    }

    while (true)
    {
        Console.Clear();
        PrintPage(page, searchLines);
        gen.Write("\nPress a number to select a series. \npress the left and right arrow keys to change pages. \npress the up arrow key to search. \npress the down arrow key to type a new series.");

        int pageStart = page * 10;

        key = Console.ReadKey();
        keyChar = char.ToString(key.KeyChar);
        keyNum = gen.StrToInt(keyChar);

        if (key.Key == ConsoleKey.DownArrow)
        {
            choice = Console.ReadLine();
        }

        else if (key.Key == ConsoleKey.LeftArrow)
        {
            page--;
        }

        else if (key.Key == ConsoleKey.RightArrow)
        {
            page++;
        }

        else if (key.Key == ConsoleKey.UpArrow)
        {
            searchLines = PageSearch(Console.ReadLine(), lines);
        }

        else if (keyChar == "q")
        {
            choice = "q";
        }

        else if(keyChar == "b")
        {
            choice = "b";
        }

        else if (gen.IsInt(keyChar) && searchLines.Length >= keyNum + 1)
        {
            choice = searchLines[pageStart + keyNum];
        }

        if (choice != "")
        {
            break;
        }

        if (page == -1)
        {
            page = searchLines.Length / 10;
        }

        if (page > searchLines.Length / 10)
        {
            page = 0;
        }
    }

    return choice;
}

//gets info for the link that will be stored
string[] GetLinkInfo(string[] holder)
{
    string exit = "";
    string link = holder[1];
    string peeps = holder[2];
    string series = holder[3];
    string time = holder[4];
    string sect = holder[5];

    Console.Clear();

    if (gen.StrToInt(sect) <= 0)
    {
        gen.Write("What is the video time? (if your link includes the time skip this)");
        time = Console.ReadLine();

        if (time == "q" || time == "b")
        {
            exit = "q";
            return new string[] { exit, link, peeps, series, time, "q" };
        }   
    }

    Console.Clear();

    if (gen.StrToInt(sect) <= 1)
    {
        gen.Write("what is the link?");
        link = Console.ReadLine();

        if (link == "q")
        {
            exit = "q";
            return new string[] { exit, link, peeps, series, time, "q" };
        }

        if (link == "b")
        {
            return new string[] { exit, link, peeps, series, time, "0" };
        }
    }

    Console.Clear();

    if (gen.StrToInt(sect) <= 2)
    {
        gen.Write("Loading channels...");
        peeps = ReadPeepsPages();

        if (peeps == "q")
        {
            exit = "q";
            return new string[] { exit, link, peeps, series, time, "q" };
        }

        if (peeps == "b")
        {
            return new string[] { exit, link, peeps, series, time, "1" };
        }
    }

    Console.Clear();

    gen.Write("Loading series...");
    series = ReadSeriesPages(peeps);

    if (series == "q")
    {
        exit = "q";
        return new string[] { exit, link, peeps, series, time, "q" };
    }

    if (series == "b")
    {
        return new string[] { exit, link, peeps, series, time, "2" };
    }

    return new string[] { exit, link, peeps, series, time, "q" };
}

//opens the link in the default browser (originally chrome but now supports firefox)
void OpenInChrome(string path)
{
    string[] lines = { };

    lines = File.ReadAllLines(gen.GetPath(path));

    Process.Start(GetBrowser(), lines[0]);
}

//stores a new link in a text file in documents/youtubeStorage/(channel name)/(series name).txt
void StoreLink()
{
    string[] holder = {"","","","","","0"};
    string exit = "";
    string link = "";
    string peeps = "";
    string series = "";
    string time = "";
    string path = "";

    bool sure = false;
    bool sureExist = true;

    while (gen.StrToInt(holder[5]) != -1)
    {
        holder = GetLinkInfo(holder);
    }

    Console.Clear();

    exit = holder[0];
    link = holder[1];
    peeps = holder[2];
    series = holder[3];
    time = holder[4];

    if (exit == "q")
    {
        return;
    }

    gen.Write($"Time:{time}\nLink:{link}\nChannel:{peeps}\nSeries:{series}");
    sure = gen.Sure("Is this correct?");

    Console.Clear();

    if (!String.IsNullOrEmpty(time) && sure)
    {
        gen.Write("Calculating Time...");
        link = $"{link}&t={CalculateTime(time)}s\n({time})";
    }

    path = Path.Combine("YoutubeStorage", Path.Combine(peeps, $"{series}.txt"));

    if (gen.FindFile(path) && sure)
    {
        sureExist = gen.Sure("file exists. do you want to overwrite it?");
        Console.WriteLine("");
    }

    try
    {
        if (!(String.IsNullOrEmpty(link)) && !(String.IsNullOrEmpty(peeps)) && !(String.IsNullOrEmpty(series)) && sure && sureExist)
        {
            gen.Write("Saving Link...");

            gen.WriteToFile(link, series, Path.Combine("YoutubeStorage", peeps));

            WritePeeps(peeps);
            WriteSeries(series, peeps);

            gen.Write("\nSaved!");
            gen.WaitForInput();
        }

        else if (!sure || !sureExist)
        {
            Console.Clear();
            gen.Write("Canceled");
            gen.WaitForInput();
        }

        else
        {
            gen.Write("Something Went wrong (make sure nothing is blank)");
            gen.WaitForInput();
        }
    }

    catch
    {
        gen.Write("Something Went wrong (make sure there are no ? or other special characters)");
        gen.WaitForInput();
    }

    Console.Clear();
}

//gets a link to a video from documents/youtubeStorage/(channel name)/(series name).txt
void GetLink()
{
    string peeps = "";
    string series = "b";
    string path = "";

    Console.Clear();

    while (series == "b")
    {
        gen.Write("Loading channels...");
        peeps = ReadPeepsPages();

        if (peeps == "q" || peeps == "b")
        {
            Console.Clear();
            return;
        }

        Console.Clear();

        gen.Write("Loading series...");
        series = ReadSeriesPages(peeps);

        if (series == "q")
        {
            Console.Clear();
            return;
        }

        Console.Clear();
    }

    path = Path.Combine("YoutubeStorage", Path.Combine(peeps, $"{series}.txt"));

    if (gen.FindFile(path))
    {
        Console.WriteLine(File.ReadAllText(gen.GetPath(path)));
        if(gen.Sure("do you want to open this in browser?"))
        {
            OpenInChrome(path);
        }
        else
        {
            gen.Write("\npress enter to continue");
            Console.ReadLine();
        }
    }
    else
    {
        Console.WriteLine("something was wrong");
        Console.WriteLine(path);
        gen.WaitForInput();
    }

    Console.Clear();
}

//deletes a link from the directory but not from the files
void DeleteLink()
{
    string peeps = "";
    string series = "b";
    string path = "";

    Console.Clear();

    while (series == "b")
    {
        gen.Write("Loading channels...");
        peeps = ReadPeepsPages();

        if (peeps == "q" || peeps == "b")
        {
            Console.Clear();
            return;
        }

        Console.Clear();

        gen.Write("Loading series...");
        series = ReadSeriesPages(peeps);

        if (series == "q")
        {
            Console.Clear();
            return;
        }

        Console.Clear();
    }

    path = Path.Combine("YoutubeStorage", Path.Combine(peeps, $"{series}.txt"));

    if (gen.FindFile(path))
    {
        Console.WriteLine(File.ReadAllText(gen.GetPath(path)));
        if(gen.Sure())
        {
            DeleteSeries(series, peeps);
            Console.Clear();
            Console.WriteLine("Deleted");
            gen.WaitForInput();
        }
    }
    else
    {
        Console.WriteLine("something was wrong");
        Console.WriteLine(path);
        gen.WaitForInput();
    }

    Console.Clear();
}

//Sets the default browser in the AAAbrowser text file
void SetBrowser()
{
    ConsoleKeyInfo key;

    Console.Clear();
    gen.Write("1:Chrome\n2:Firefox");
    key = Console.ReadKey();

    if(key.KeyChar == '1')
    {
        if (File.Exists("C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"))
        {
            gen.WriteToFile("C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe", "AAAbrowser", "YoutubeStorage");
        }

        else if (File.Exists("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"))
        {
            gen.WriteToFile("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe", "AAAbrowser", "YoutubeStorage");
        }

        else
        {
            gen.Write("Chrome is not installed");
            gen.WaitForInput();
        }
    }

    else if(key.KeyChar == '2')
    {
        if (File.Exists("C:\\Program Files\\Mozilla Firefox\\firefox.exe"))
        {
            gen.WriteToFile("C:\\Program Files\\Mozilla Firefox\\firefox.exe", "AAAbrowser", "YoutubeStorage");
        }

        else if (File.Exists("C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe"))
        {
            gen.WriteToFile("C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe", "AAAbrowser", "YoutubeStorage");
        }

        else
        {
            gen.Write("Firefox is not installed");
            gen.WaitForInput();
        }
    }

    Console.Clear();
}

//Gets the default browser from the AAAbrowser text file
string GetBrowser()
{
    string browser = "";
    string[] browserLines = { };

    if(gen.FindFile("YoutubeStorage\\AAAbrowser.txt"))
    {
        browserLines = File.ReadAllLines(gen.GetPath("YoutubeStorage\\AAAbrowser.txt"));
        browser = browserLines[0];
    }

    return browser;
}

void TryMovePeeps(string mainDirectory, string peeps, string newPeeps)
{
    try
    {
        gen.Write("Renaming...");

        Directory.Move(Path.Combine(mainDirectory, peeps), Path.Combine(mainDirectory, newPeeps));

        WritePeeps(newPeeps);
        DeletePeepsNoSure(peeps);

        Console.Clear();

        gen.Write("Renamed!");
        gen.WaitForInput();
    }

    catch
    {
        Console.Clear();

        gen.Write("Something Went wrong (make sure there are no special characters and that the file does not already exist)");
        gen.WaitForInput();
    }
}

void TryMoveSeries(string mainDirectory, string series, string newSeries, string peeps)
{
    try
    {
        gen.Write("Renaming...");

        File.Move($"{Path.Combine(mainDirectory, series)}.txt", $"{Path.Combine(mainDirectory, newSeries)}.txt");

        WriteSeries(newSeries, peeps);
        DeleteSeries(series,peeps);

        Console.Clear();

        gen.Write("Renamed!");
        gen.WaitForInput();
    }

    catch
    {
        Console.Clear();

        gen.Write("Something Went wrong (make sure there are no special characters and that the file does not already exist)");
        gen.WaitForInput();
    }
}

void DeletePeepsNoSure (string peeps)
{
    string contents = "";
    string[] lines = { "" };
    string path = gen.GetPath(Path.Combine("YoutubeStorage", "AAApeeps.txt"));
    var linesList = new List<string>();

    Console.Clear();

    if (gen.FindFile(Path.Combine("YoutubeStorage", "AAApeeps.txt")))
    {
        lines = File.ReadAllLines(path);
    }

    if (CheckExists(lines, peeps))
    {
        foreach (string line in lines)
        {
            if (line.ToLower() == peeps.ToLower())
            {
                linesList = lines.ToList();
                linesList.Remove(line);
                lines = linesList.ToArray();
            }
        }

        foreach (string line in lines)
        {
            if (line != lines.Last())
            {
                contents += $"{line}\n";
            }
            else
            {
                contents += line;
            }
        }

        if (contents == "")
        {
            gen.DeleteFile(path);
        }

        else
        {
            gen.WriteToFile(contents, "AAApeeps", "YoutubeStorage");
        }
    }
}

void RenameFile()
{
    bool channelRename;
    string peeps;
    string series;
    string newPeeps;
    string newSeries = "b";
    string peepDirectory;
    string mainDirectory = gen.GetPath("YoutubeStorage");

    Console.Clear();
    channelRename = gen.Sure("Are you renaming a channel?");

    while (newSeries == "b")
    {
        gen.Write("Loading channels...");
        peeps = ReadPeepsPages();

        Console.Clear();

        if (peeps == "q" || peeps == "b")
        {
            return;
        }

        if (channelRename)
        {
            gen.Write("What will the new name be?");
            newPeeps = Console.ReadLine();

            Console.Clear();

            if (newPeeps == "b")
            {

            }

            else if (newPeeps == "q")
            {
                return;
            }

            else
            {
                newSeries = "";

                TryMovePeeps(mainDirectory, peeps, newPeeps);
            }
        }

        else
        {
            gen.Write("Loading series...");
            series = ReadSeriesPages(peeps);

            peepDirectory = Path.Combine(mainDirectory, peeps);

            Console.Clear();

            if (series == "q")
            {
                Console.Clear();
                return;
            }

            gen.Write("What will the new name be?");
            newSeries = Console.ReadLine();

            Console.Clear();

            if (newSeries == "b")
            {

            }

            else if (newSeries == "q")
            {
                return;
            }

            else
            {
                TryMoveSeries(peepDirectory, series, newSeries, peeps);
            }
        }

        Console.Clear();
    }
}

//add a note to an existing series
void AddNote()
{
    string peeps = "";
    string series = "b";
    string path = "";
    string noteToWrite = "";
    string[] currentContents = { "" };

    Console.Clear();

    while (series == "b")
    {
        gen.Write("Loading channels...");
        peeps = ReadPeepsPages();

        if (peeps == "q" || peeps == "b")
        {
            Console.Clear();
            return;
        }

        Console.Clear();

        gen.Write("Loading series...");
        series = ReadSeriesPages(peeps);

        if (series == "q")
        {
            Console.Clear();
            return;
        }

        Console.Clear();
    }

    path = Path.Combine("YoutubeStorage", Path.Combine(peeps, $"{series}.txt"));

    if (gen.FindFile(path))
    {
        noteToWrite = File.ReadAllLines(gen.GetPath(path))[0];

        gen.Write("What note would you like to make?");

        noteToWrite += "\n\n" + Console.ReadLine();

        gen.WriteToFile(noteToWrite, series, Path.Combine("YoutubeStorage", peeps));
    }
    else
    {
        Console.WriteLine("something was wrong");
        Console.WriteLine(path);
        gen.WaitForInput();
    }

    Console.Clear();
}

//main menu
void MainMenu()
{
    while (true)
    {
        ConsoleKeyInfo key;

        gen.Write("1:Get Link\n2:Store New link\n3:Add Note\n4:Delete Link\n5:Rename Series\n6:Set Browser\nQ:exit");
        key = Console.ReadKey();

        if (key.KeyChar == '1')
        {
            GetLink();
        }
        else if (key.KeyChar == '2')
        {
            StoreLink();
        }
        else if (key.KeyChar == '3')
        {
            AddNote();
        }
        else if (key.KeyChar == '4')
        {
            DeleteLink();
        }
        else if (key.KeyChar == '5')
        {
            RenameFile();
        }
        else if (key.KeyChar == '6')
        {
            SetBrowser();
        }
        else
        {
            break;
        }
    }
}

//initial call
MainMenu();