using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace gab_athens.Services.Sessionize.Models
{
    public class Speaker
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
        [JsonPropertyName("biio")]
        public string Bio { get; set; }
        [JsonPropertyName("tagLine")]
        public string TagLine { get; set; }
        [JsonPropertyName("profilePicture")]
        public string ProfilePicture { get; set; }
        [JsonPropertyName("sessions")]
        public IList<Session> Sessions { get; set; }
        [JsonPropertyName("isTopSpeaker")]
        public bool IsTopSpeaker { get; set; }
        [JsonPropertyName("links")]
        public IList<Link> Links { get; set; }
    }

    public class Link
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("linkType")]
        public string LinkType { get; set; }
    }

    public class Session
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}