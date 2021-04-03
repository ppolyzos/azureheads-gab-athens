using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace gab_athens.Services.Sessionize.Models
{
    public class Speaker
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("firstName")] public string FirstName { get; set; }
        [JsonPropertyName("lastName")] public string LastName { get; set; }
        [JsonPropertyName("fullName")] public string FullName { get; set; }
        [JsonPropertyName("biio")] public string Bio { get; set; }
        [JsonPropertyName("tagLine")] public string TagLine { get; set; }
        [JsonPropertyName("profilePicture")] public string ProfilePicture { get; set; }
        [JsonPropertyName("sessions")] public IList<SessionOverview> Sessions { get; set; }
        [JsonPropertyName("isTopSpeaker")] public bool IsTopSpeaker { get; set; }
        [JsonPropertyName("links")] public IList<Link> Links { get; set; }
    }

    public class Link
    {
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }
        [JsonPropertyName("linkType")] public string LinkType { get; set; }
    }

    public class SessionOverview
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
    }

    public class GroupSession
    {
        [JsonPropertyName("groupId")] public string GroupId { get; set; }
        [JsonPropertyName("groupName")] public string GroupName { get; set; }

        [JsonPropertyName("sessions")] public IEnumerable<Session> Sessions { get; set; }
    }

    public class Session
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("startsAt")] public DateTime StartsAt { get; set; }
        [JsonPropertyName("endsAt")] public DateTime EndsAt { get; set; }
        [JsonPropertyName("isServiceSession")] public bool IsServiceSession { get; set; }
        [JsonPropertyName("isPlenumSession")] public bool IsPlenumSession { get; set; }

        [JsonPropertyName("room")] public string Room { get; set; }
        
        [JsonPropertyName("speakers")]
        public IEnumerable<SessionItem<string>> Speakers { get; set; }
        [JsonPropertyName("categories")] public IEnumerable<SessionCategory> Categories { get; set; }
    }

    public class SessionCategory
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("categoryItems")] public IEnumerable<SessionItem<int>> CategoryItems { get; set; }
    }

    public class SessionItem<T>
    {
        [JsonPropertyName("id")] public T Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
    }
}