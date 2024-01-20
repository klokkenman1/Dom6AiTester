using System.Diagnostics;

string dom6ExeLocation = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Dominions6\\Dominions6.exe";

// Will create a folder called Game in %appdata%/Dominions6/savedgames
string gameName = "Game";
// Make sure the map has a UW place for the omni to spawn and spawns for the AIs you want
string map = "Test4.map";
// Dont forget to add the folder the mod is in e.g. %appdata%/Dominions6/mods/[Folder]/[*.dm]
string aiModToTest = "A\\mar.dm";

// AIs to spawn in
int[] AiNations = { 95, 103};


Process serverProcess = new Process();
Process clientProcess = new Process();

serverProcess.StartInfo.FileName = dom6ExeLocation;
serverProcess.StartInfo.RedirectStandardOutput = true;

serverProcess.StartInfo.Arguments = $"-w --enablemod 3140298160\\LA_Omniscience_v1_00.dm --enablemod {aiModToTest} --tcpserver --port 25565 --era 3 ";

foreach (int nr in AiNations)
{
    serverProcess.StartInfo.Arguments += $"--normai {nr} ";
}

serverProcess.StartInfo.Arguments += $"--mapfile {map} --thrones 2 0 0 --requiredap 2 {gameName}";
serverProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
serverProcess.Start();


clientProcess.StartInfo.FileName = dom6ExeLocation;
clientProcess.StartInfo.Arguments = "-w --tcpclient --ipadr localhost --port 25565 --nosound";
clientProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
clientProcess.Start();

// TODO: find a way for the client to upload a pretender. For now you have to manually add a pretender in the UI

await Task.Delay(10000);

while (!serverProcess.HasExited)
{
    if (!File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Dominions6\\savedgames\\{gameName}\\domcmd"))
    {
        File.Copy("domcmd", $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Dominions6\\savedgames\\{gameName}\\domcmd");
    }
    await Task.Delay(1500);
}
