using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DataProcessingExam
{
    public class DataProcessorService
    {
        private eServerState m_state;
        private Stopwatch stopwatch;
        private readonly ILogger<DataProcessorService> logger;
        private readonly FileConfiguration config;
        private List<WordAnalysis> wordAnalyses;
        public DataProcessorService(ILogger<DataProcessorService> logger, IOptions<FileConfiguration> config)
        {
            this.logger = logger;
            this.config = config.Value;
        }

        public async Task<List<WordAnalysis>> ProcessData()
        {
            m_state = eServerState.Running;
            stopwatch = Stopwatch.StartNew();

            wordAnalyses = await ProcessFile();

            stopwatch.Stop();
            m_state = eServerState.Completed;

            return wordAnalyses;
        }

        public List<WordAnalysis> GetLastResult => wordAnalyses;

        private async Task<List<WordAnalysis>> ProcessFile()
        {
            logger.LogInformation($"Initiating process {config.GetFileForProcessing()}");
            //TODO: Implement!

            await Task.Delay(TimeSpan.FromSeconds(5)); //Simulates the process time

            logger.LogInformation($"Process completed {config.GetFileForProcessing()}. Execution time {stopwatch.Elapsed}");

            return new List<WordAnalysis> {
                new WordAnalysis { Letter = 'B', NumberOfOccurrences = 22 },
                new WordAnalysis { Letter = 'A', NumberOfOccurrences = 50 }
            };
        }

        public ServerState GeteServerState() => new ServerState
        {
            ProcessTime = stopwatch?.Elapsed ?? TimeSpan.Zero,
            State = m_state,
        };
    }
}
