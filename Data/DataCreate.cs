using Newtonsoft.Json;
using System.Data;
using System.IO;
using DSharpPlus.Entities;
using System.Collections.Generic;

namespace SunflowerBot.Data
{
    public class DataCreate
    {
        public static void CreateDB(string path, IReadOnlyDictionary<ulong, DiscordMember> users)
        {
            DataSet usersList = new DataSet("usersList");
            usersList.Namespace = "DiscordUsers";

            DataTable table = new DataTable();
            DataColumn idColumn = new DataColumn("id", typeof(int));
            idColumn.AutoIncrement = true;

            DataColumn discordIdColumn = new DataColumn("discordId", typeof(ulong));
            DataColumn usernameColumn = new DataColumn("username", typeof(string));
            DataColumn sunCountColumn = new DataColumn("sunCount", typeof(int));

            table.Columns.Add(idColumn);
            table.Columns.Add(discordIdColumn);
            table.Columns.Add(usernameColumn);
            table.Columns.Add(sunCountColumn);

            usersList.Tables.Add(table);

            foreach (var item in users)
            {
                DataRow newRow = table.NewRow();
                newRow["discordId"] = item.Key;
                newRow["username"] = item.Value.Username;
                newRow["sunCount"] = 0;

                table.Rows.Add(newRow);
            }

            usersList.AcceptChanges();

            File.WriteAllText(path, JsonConvert.SerializeObject(usersList, Formatting.Indented));
        }
    }
}