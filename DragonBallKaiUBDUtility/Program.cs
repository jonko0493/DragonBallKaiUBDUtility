using Mono.Options;

namespace DragonBallKaiUBDUtility
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommandSet commands = new("DragonBallKaiUBDUtility")
            {
                new CompressDsoCommand(),
                new ConvertDsoCommand(),
                new DsaUnpackCommand(),
            };
            commands.Run(args);
        }
    }
}
