using System.Text.Json.Serialization;

namespace TodoAuthApi.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;

        // FK - Bu görevin hangi kullanıcıya ait olduğunu belirten alan
        public int UserId { get; set; }

        [JsonIgnore]
        public User? user { get; set; }
    }
}