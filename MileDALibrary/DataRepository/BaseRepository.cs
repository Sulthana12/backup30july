using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MileDALibrary.DataRepository
{
    public class BaseRepository
    {
        private readonly IConfiguration config;

        public BaseRepository(IConfiguration config)
        {
            this.config = config;
        }

        /// <summary>
        /// This method is used to retrieve connection string from the configuration
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            string cs = config["DBSettings:ConnectionString"];
            return cs;
        }
    }
}
