﻿using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RPServer.Database;
using Dapper.Contrib.Extensions;

namespace RPServer.Models
{
    internal class Base<T> where T: class
    {
        public static async Task<int> CreateAsync(T newEntry)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.InsertAsync(newEntry);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return -1;
            }
        }
        public async Task<int> CreateAsync() => await CreateAsync(this as T);

        public static async Task<T> ReadAsync(int sqlID)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.GetAsync<T>(sqlID);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }

        public static async Task<bool> UpdateAsync(T dbAcc)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.UpdateAsync(dbAcc);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }
        public async Task<bool> UpdateAsync() => await UpdateAsync(this as T);

        public static async Task<bool> DeleteAsync(T dbAcc)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.DeleteAsync(dbAcc);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }
        public async Task<bool> DeleteAsync() => await DeleteAsync(this as T);
    }

    [Table("accounts")]
    internal class AccountModel : Base<AccountModel>
    {
        [Key]
        public int AccountID { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public byte[] Hash { get; set; }
        public string ForumName { get; set; }
        public string NickName { get; set; }
        public string RegSocialClubName { get; set; }
        public string LastSocialClubName { get; set; }
        public string LastIP { get; set; }
        public string LastHWID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool HasEnabledTwoStepByEmail { get; set; }
        public byte[] TwoFactorGASharedKey { get; set; }

        private AccountModel()
        {

        }
        public AccountModel(string username, byte[] hash, string regSocialClubName)
        {
            Username = username;
            Hash = hash;
            RegSocialClubName = regSocialClubName;
            CreationDate = DateTime.Now;
        }

        public static async Task<int> GetSqlIdAsync(string username)
        {
            const string query = "SELECT accountID FROM accounts WHERE username = @username;";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync<int?>(query, new { username = username });
                    var sqlID = result.SingleOrDefault();
                    if (sqlID == null) return -1;
                    return sqlID.Value;

                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
                return -1;
            }
        }



    }
}
