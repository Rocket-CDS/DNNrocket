﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public static class DNNrocketRoles
    {
        public const string Administrators = "Administrators";
        public const string Manager = "Manager";
        public const string Editor = "Editor";
        public const string ClientEditor = "ClientEditor";
        public const string Premium = "Premium";
        public const string RemoteAdmin = "RemoteAdmin";
    }

    public enum ButtonTypes { add, admin, back, cancel, close, create, delete, download, edit, export, import, next, ok, print, refresh, remove, reset, save, search, cancelsearch, setup, send, undo, upload, use, previous, yes, no };


    public class ValuePair
    {
        [JsonProperty("key")]
        public string Key;
        [JsonProperty("value")]
        public string Value;
    }


}