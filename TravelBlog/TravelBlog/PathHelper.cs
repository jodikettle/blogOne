﻿using System;
using TravelBlog.Models;

namespace TravelBlog
{
    public class PathHelper
    {
        public const string ThemePath = "/Views/";
        public const string VirtualThemePath = "~" + ThemePath;
        private const string VirtualThemePathToken = "~" + ThemePath + "/{0}/";
        private const string VirtualThemeViewPathToken = "~" + ThemePath + "{0}.cshtml";
        private const string VirtualThemePartialViewPathToken = "~" + ThemePath + "/Partials/{0}.cshtml";

        /// <summary>
        /// Get the full domain of the current page
        /// </summary>
        public static string GetDomain(Uri requestUrl)
        {
            return requestUrl.Scheme +
                System.Uri.SchemeDelimiter +
                requestUrl.Authority;
        }
    }
}