using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BeeFee.WebApplication.Infrastructure.Extensions
{
    public static class RouteValueDictionaryExtensions
    {
        public static string GetArea(this RouteValueDictionary values)
        {
            if (values.ContainsKey("area"))
                return (string) values["area"];
            return string.Empty;
        }

        public static string GetAction(this RouteValueDictionary values)
        {
            if (values.ContainsKey("action"))
                return (string)values["action"];
            return string.Empty;
        }

        public static string GetController(this RouteValueDictionary values)
        {
            if (values.ContainsKey("controller"))
                return (string)values["controller"];
            return string.Empty;
        }
    }
}
