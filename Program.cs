using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace Sunflower.Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
