using System;

namespace DataProcessingExam
{
    public struct ServerState
    {
        public DateTime Time { get; set; }

        public eServerState State { get; set; }

        public TimeSpan ProcessTime { get; set; }
    }
}
