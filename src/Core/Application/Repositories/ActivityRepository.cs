using Dapper;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Application.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private IConfiguration _configuration;

        public ActivityRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<DataProdutivity>> GetActivityAsync(string userName, string docEntry)
        {
            var query = $@"select * from activity a where username ='{userName}' and reference ='{docEntry}' and action_activity ='FinishPicking'";

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("postgresql")))
            {
                return await connection.QueryAsync<DataProdutivity>(query);
            }
        }

        public async Task<IEnumerable<DataProdutivity>> GetAllActivities(int bplId, DateTime date, string action)
        {
            var query = $@"select username,
                            (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='00' and T0.username = T1.username  and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T0,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='01' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T1,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='02' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T2,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='03' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T3,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='04' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T4,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='05' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T5,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='06' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T6,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='07' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T7,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='08' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T8,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='09' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T9,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='10' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T10,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='11' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T11,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='12' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T12,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='13' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T13,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='14' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T14,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='15' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T15,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='16' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T16,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='17' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T17,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='18' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T18,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='19' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T19,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='20' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T20,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='21' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T21,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='22' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T22,
 	                        (select count(username) from activity T0 where TO_CHAR(date_activity at time zone 'utc' at time zone 'america/sao_paulo','HH24')='23' and T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as T23,
                            (select count(username) from activity T0 where  T0.username = T1.username and cast(T0.date_activity as date) = '{date.ToString("yyyy-MM-dd")}' and action_activity='{action}') as TT
 	                        from activity T1 where cast(date_activity as date) ='{date.ToString("yyyy-MM-dd")}' and branch='{bplId}' and action_activity='{action}'
                             group by username";

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("postgresql")))
            {
                return await connection.QueryAsync<DataProdutivity>(query);
            }
        }

        public async Task SaveActivity(Activity activity)
        {
            var query = $@"insert into activity (date_activity, username, reference,action_activity, branch)
                            values('{activity.Date_activity}','{activity.Username}','{activity.Reference}','{activity.Action_activity}','{activity.Branch}')";

            using (NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString("postgresql")))
            {
                await connection.ExecuteAsync(query);
            }
        }
    }
}
