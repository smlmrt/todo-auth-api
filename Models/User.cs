using System.Text.Json.Serialization;

namespace TodoAuthApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        //Kullanıcının şifresini düz metin olarak değil hashlenmiş olarak tutacağız
        public string PasswordHash { get; set; } = string.Empty;

        // Bire-Çok ilişkinin Çok tarafı: Bir kullanıcının birden fazla görevi olabilir
        [JsonIgnore] // Önceki projede yaşadığımız o meşhur "Sonsuz Döngü" hatasını baştan engellemek için
        public List<ToDoItem> Tasks { get; set; } = new(); 
    }
}