﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using GTANetworkAPI;

namespace RPServer.Models
{
    [Table("characters")]
    internal class Character : Model<Character>
    {
        public int CharOwnerID { set; get; }
        public string CharacterName { set; get; }

        public Character()
        {
        }

        /// <summary>
        /// Use to create new character
        /// </summary>
        public static async Task CreateNewAsync(Account charOwner, string newCharName)
        {
            var newChar = new Character()
            {
                CharOwnerID = charOwner.ID,
                CharacterName = newCharName
            };
            await newChar.CreateAsync();
        }
        public static async Task<List<Character>> FetchAllAsync(Account account)
        {
            var result = await ReadByKeyAsync(() => new Character().CharOwnerID, account.ID);
            var charsData = result.ToList();
            return charsData;
        }

        public async Task<Appearance> GetAppearance()
        {
            var app =  await Appearance.ReadByKeyAsync(() => new Appearance().CharacterID, this.ID);
            return app.FirstOrDefault();
        }
    }
}
