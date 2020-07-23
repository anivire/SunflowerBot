using Newtonsoft.Json;
using System.Data;
using System.IO;

namespace Sunflower.Bot.Data
{
    public class DataLoad
    {
        public static DataTable LoadDB(string path, ulong guildId)
        {
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText(path + $"{guildId}.json"));

            return dataSet.Tables["Table1"];
        }
    }
}