using System.Diagnostics;

namespace Dom6AiTester
{
    public class Dom6AiTester
    {
        public static void StartServer(string dom6ExeLocation, string gameName, string map, string aiModToTest, IEnumerable<int> aiNations)
        {
            var level1thrones = aiNations.Count();
            var level2thrones = 0;
            var level3thrones = 0;

            var requiredAscenscionPoints = level1thrones / 2;

            var serverProcess = new Process();

            serverProcess.StartInfo.FileName = dom6ExeLocation;
            serverProcess.StartInfo.RedirectStandardOutput = true;

            serverProcess.StartInfo.Arguments = $"-w --enablemod 3140298160\\LA_Omniscience_v1_00.dm --enablemod {aiModToTest} --tcpserver --port 25565 --era 3 ";

            foreach (var nr in aiNations)
            {
                serverProcess.StartInfo.Arguments += $"--normai {nr} ";
            }

            serverProcess.StartInfo.Arguments += $"--mapfile {map} --thrones {level1thrones} {level2thrones} {level3thrones} --requiredap {requiredAscenscionPoints} {gameName}";
            serverProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            _ = serverProcess.Start();
        }

        public static void StartClient(string dom6ExeLocation)
        {
            var clientProcess = new Process();
            clientProcess.StartInfo.FileName = dom6ExeLocation;
            clientProcess.StartInfo.Arguments = "-w --tcpclient --ipadr localhost --port 25565 --nosound";
            clientProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            _ = clientProcess.Start();
        }

        public static async Task StartTurns(string gameName, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Dominions6\\savedgames\\{gameName}\\domcmd"))
                {
                    File.Copy("domcmd", $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Dominions6\\savedgames\\{gameName}\\domcmd");
                }
                await Task.Delay(1500);
            }
        }
    }
}
