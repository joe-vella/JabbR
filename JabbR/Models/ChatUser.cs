﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JabbR.Infrastructure;

namespace JabbR.Models
{
    public class ChatUser
    {
        [Key]
        public int Key { get; set; }

        [MaxLength(200)]
        public string Id { get; set; }
        public string Name { get; set; }
        // MD5 email hash for gravatar
        public string Hash { get; set; }
        public string Salt { get; set; }
        public string HashedPassword { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime? LastNudged { get; set; }
        public int Status { get; set; }

        [StringLength(200)]
        public string Note { get; set; }

        [StringLength(200)]
        public string AfkNote { get; set; }

        public bool IsAfk { get; set; }

        [StringLength(2)]
        public string Flag { get; set; }

        // TODO: Migrate everyone off identity and email
        public string Identity { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }

        // List of clients that are currently connected for this user
        public virtual ICollection<ChatUserIdentity> Identities { get; set; }
        public virtual ICollection<ChatClient> ConnectedClients { get; set; }
        public virtual ICollection<ChatRoom> OwnedRooms { get; set; }
        public virtual ICollection<ChatRoom> Rooms { get; set; }
        public virtual ICollection<ChatUserPreference> Preferences { get; set; }

        // Private rooms this user is allowed to go into
        public virtual ICollection<ChatRoom> AllowedRooms { get; set; }

        public ChatUser()
        {
            Identities = new SafeCollection<ChatUserIdentity>();
            ConnectedClients = new SafeCollection<ChatClient>();
            OwnedRooms = new SafeCollection<ChatRoom>();
            Rooms = new SafeCollection<ChatRoom>();
            AllowedRooms = new SafeCollection<ChatRoom>();
        }

        public bool HasUserNameAndPasswordCredentials()
        {
            return !String.IsNullOrEmpty(HashedPassword) && !String.IsNullOrEmpty(Name);
        }

        public T GetPreference<T>(string key, int roomId)
        {
            var pref = Preferences.SingleOrDefault(x => x.Key == key && x.RoomId == roomId);

            if (pref == null)
            {
                return default(T);
            }

            return (T)Convert.ChangeType(pref.Value, typeof (T));
        }

        public void SetPreference(string key, int roomId, object value)
        {
            var pref = Preferences.SingleOrDefault(x => x.Key == key && x.RoomId == roomId);

            if (pref == null)
            {
                pref = new ChatUserPreference()
                {
                    ChatUserId = Key,
                    Key = key,
                    RoomId = roomId
                };

                Preferences.Add(pref);
            }
            else
            {
                pref.Value = value.ToString();
            }
        }
    }

    public class ChatUserPreference
    {
        public const string AudibleNotificationsKey = "jabbr.audible";
        public const string RichContentKey = "jabbr.richcontent";
        public const string PopupNotificationsKey = "jabbr.toast";

        public int ChatUserId { get; set; }

        public int RoomId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}