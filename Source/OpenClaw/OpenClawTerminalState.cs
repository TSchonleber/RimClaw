using System.Collections.Generic;

namespace OpenClaw
{
    public static class OpenClawTerminalState
    {
        public static readonly Queue<string> Logs = new Queue<string>();
        public static int MaxLogs = 100;

        public static void Add(string line)
        {
            Logs.Enqueue(line);
            while (Logs.Count > MaxLogs) Logs.Dequeue();
        }

        public static string Dump()
        {
            return string.Join("\n", Logs.ToArray());
        }
    }
}
