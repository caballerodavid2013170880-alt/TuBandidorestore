using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Models.Auth.User
{
    public class LoginResponse
    {
        [JsonPropertyName("Access_Token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("Refresh_Token")]
        public string RefreshToken { get; set;}
        [JsonPropertyName("Due_Date")]
        public DateTime ExpirationDate {  get; set; }
        [JsonPropertyName("Expiry_Time")]
        public int ExpiryTime { get; set; }
    }
}
