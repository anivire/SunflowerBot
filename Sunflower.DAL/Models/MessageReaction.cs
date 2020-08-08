namespace Sunflower.DAL.Models
{
    class MessageReaction : Entity
    {
        public ulong GuildId { get; set; }
        public ulong MessageId { get; set; }
        public ulong RoleId { get; set; }
    }
}
