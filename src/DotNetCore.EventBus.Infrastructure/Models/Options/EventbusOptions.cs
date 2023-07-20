using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Options
{
    public class EventbusOptions
    {
        /// <summary>
        /// kafka
        /// </summary>
        public string BootstrapServers { get; set; }

        /// <summary>
        /// Subscriber group prefix.
        /// </summary>
        public string? GroupId { get; set; }

        /// <summary>
        /// Topic prefix.
        /// </summary>
        public string? TopicPrefix { get; set; }

        /// <summary>
        /// retry count
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// retry interval
        /// </summary>
        public int RetryInterval { get; set; } = 300;
    }
}
