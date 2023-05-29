using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Api.Core.Infrastructure.Extensions;
using EventManagement.Web.Data.Models;

namespace EventManagement.Web.Integrations.Sessionize
{
    public interface IEventSessionizeService
    {
        Task<IEnumerable<Speaker>> FetchSpeakersAsync(string eventId);
        Task<IEnumerable<Session>> FetchSessionsAsync(string eventId);
    }

    public class EventSessionizeService : IEventSessionizeService
    {
        private readonly Dictionary<string, string> _icons = new()
        {
            { "LinkedIn", "fa fa-linkedin" },
            { "Blog", "fa fa-globe" },
            { "Company_Website", "fa fa-globe" },
            { "Other", "fa fa-globe" },
            { "Facebook", "fa fa-facebook" },
            { "Twitter", "fa fa-twitter" },
            { "Instagram", "fa fa-instagram" }
        };

        private readonly ISessionizeService _sessionizeService;

        public EventSessionizeService(ISessionizeService sessionizeService)
        {
            _sessionizeService = sessionizeService;
        }

        public async Task<IEnumerable<Speaker>> FetchSpeakersAsync(string eventId)
        {
            var sessionizeSpeakers = await _sessionizeService.FetchSpeakersAsync(eventId);

            var speakers = sessionizeSpeakers.Select(ss => new Speaker
            {
                Id = $"{ss.FirstName}-{ss.LastName}".ToLowerInvariant(),
                Aliases = new[] { ss.LastName.ToLowerInvariant() },
                Name = ss.FullName,
                Title = ss.TagLine,
                About = ss.Bio,
                ImageUrl = ss.ProfilePicture,
                Links = ss.Links.Select(c => new Link
                    { Url = c.Url, Icon = _icons[c.LinkType] }).ToList()
            }).ToList();

            return speakers;
        }

        public async Task<IEnumerable<Session>> FetchSessionsAsync(string eventId)
        {
            var sessionizeSessions = await _sessionizeService.FetchSessionsAsync(eventId);

            var speakers = (await FetchSpeakersAsync(eventId)).ToArray();
            var sessions = sessionizeSessions.Select(ss => new Session
            {
                Title = ss.Title,
                Description = ss.Description,
                SpeakerIds = ss.Speakers.Select(c => c.Name).ToArray(),
                Time = ss.StartsAt.HasValue ? $"{ss.StartsAt.Value.ToTimeZone("HH:mm")} - {ss.EndsAt.Value.ToTimeZone("HH:mm")}" : "N/A",
                Room = ss.Room,
                Categories = ss.Categories.ToDictionary(c => c.Name,
                    sc => sc.CategoryItems.Select(si => si.Name))
            }).ToList();

            foreach (var session in sessions)
            {
                session.Speakers = new List<Speaker>();
                foreach (var speakerId in session.SpeakerIds)
                {
                    var speaker = speakers.FirstOrDefault(s => s.Name == speakerId);
                    if (speaker == null) continue;

                    session.Speakers.Add(speaker);
                }

                session.SpeakerIds = session.Speakers.Select(c => c.Id).ToArray();
            }

            var slots = sessions.GroupBy(c => c.Room).Select(c => c.Key);
            foreach (var slot in slots)
            {
                var serviceSessions = GetServiceSessions(slot);
                if (slot != null && slot.Equals("Gazarte 1"))
                {
                    serviceSessions = serviceSessions.Where(s => s.Title != "Welcome Keynote").ToArray();
                }

                sessions.AddRange(serviceSessions);
            }

            sessions = sessions.OrderBy(c => c.Room).ThenBy(c => c.Time).ToList();

            return sessions;
        }

        private static Session[] GetServiceSessions(string room)
        {
            return new[]
            {
                new Session
                {
                    Title = "Welcome Keynote",
                    Time = "10:00 - 10:30",
                    Icon = "fa fa-building-o",
                    Room = room,
                    IsGreeting = true,
                },

                new Session
                {
                    Title = "Lunch Break",
                    Time = "13:05 - 14:00",
                    Icon = "fa fa-coffee",
                    Room = room,
                    IsGreeting = true
                },

                new Session
                {
                    Title = "Closing",
                    Description = "",
                    Time = "16:35 - 17:00",
                    Icon = "fa fa-gift",
                    Room = room,
                    IsGreeting = true
                }
            };
        }
    }
}