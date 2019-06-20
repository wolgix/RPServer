﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using RPServer.Models;

namespace RPServer.Util
{
    public static class Extensions
    {
        #region StringExtensions

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return char.IsUpper(input.First()) ? input : input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static bool IsValidEmail(this string emailString)
        {
            return new EmailAddressAttribute().IsValid(emailString);
        }

        #endregion

        #region MySQLExtensions
        public static string GetSafeString(this MySqlDataReader reader, string column)
        {
            return !reader.IsDBNull(reader.GetOrdinal(column)) ? reader.GetString(column) : null;
        }

        public static DateTime GetSafeDateTime(this MySqlDataReader reader, string column)
        {
            return !reader.IsDBNull(reader.GetOrdinal(column)) ? reader.GetDateTime(column) : DateTime.MinValue;
        }

        #endregion

        #region ClientExtensions

        #endregion
    }
}