using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.Classes
{
    public class DatabaseGenerator
    {
        private readonly IConfiguration _config;
        private readonly ConfigurationWriter _configWriter;

        public DatabaseGenerator(IConfiguration config, ConfigurationWriter configWriter)
        {
            _config = config;
            _configWriter = configWriter;
        }

        public void Run()
        {
            var version = GetDbVersion();
            var lastVersion = "1.0.0";
            switch (version)
            {
                case "0.0.0":
                    CreateDb();
                    break;
            }

            if (version != lastVersion)
            {
                _configWriter.Set("Db:Version", lastVersion);
            }
        }

        private void CreateDb()
        {
            using (var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var tasks = new Task<int>[] {

                connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS users (
  id int(11) NOT NULL AUTO_INCREMENT,
  login varchar(100) NOT NULL,
  password varchar(100) NOT NULL,
  fullname varchar(200) DEFAULT NULL,
  role varchar(100) DEFAULT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
"),

                connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS campaign (
  id int(11) NOT NULL AUTO_INCREMENT,
  name varchar(500) NOT NULL,
  status INT(1) NOT NULL DEFAULT 0,
  abonentscount int(5),
  abonentsfilename varchar(4000) DEFAULT NULL,
  voicefilename varchar(4000) DEFAULT NULL,
  modifierid INT(11) NOT NULL,
  modified DATETIME,
  PRIMARY KEY (id),
  CONSTRAINT fk_campaign_modifier FOREIGN KEY (modifierid) REFERENCES users(id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
"),

                connection.ExecuteAsync(@"CREATE TABLE IF NOT EXISTS campaignabonent (
  id int(11) NOT NULL AUTO_INCREMENT,
  campaignid int(11),
  phone varchar(100) DEFAULT NULL,
  haserrors BOOLEAN DEFAULT NULL,
  modifierid INT(11) NOT NULL,
  modified DATETIME,
  PRIMARY KEY (id),
  CONSTRAINT fk_campaignabonent_campaignid FOREIGN KEY (modifierid) REFERENCES users(id),
  CONSTRAINT fk_campaignabonent_modifier FOREIGN KEY (modifierid) REFERENCES users(id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
"),

                connection.ExecuteAsync(@"create or replace view vcampaigns as
SELECT id,
	name,
    abonentscount abonentstotal,
    abonentsfilename,
    voicefilename,
    status,
    (select count(*) from campaignabonent where campaignid=c.id) abonentsprocessed 
FROM campaign c") };

                Task.WaitAll(tasks);
            }
        }

        private string GetDbVersion()
        {
            var version = _config.GetValue<string>("Db:Version");
            if (string.IsNullOrEmpty(version))
            {
                version = "0.0.0";
            }

            return version;
        }
    }
}
