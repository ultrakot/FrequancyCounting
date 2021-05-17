using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
           
            //chunks of 4KB
            int bufferSize = 4096;
            List<string> FileChunks = new List<string>();
            int totalChunks;

            using (FileStream SourceStream = File.Open(config.GetFileForProcessing(), FileMode.Open))
            {
                //array of chunks of 4kb 
                //total size is the total length / 4kb
                totalChunks = (int)SourceStream.Length / bufferSize;
                int read = 0;
                //chunk buffer
                var chunkBuffer = new byte[bufferSize];

                //progresses at reading chunks                 
                while ((read = await SourceStream.ReadAsync(chunkBuffer, 0, chunkBuffer.Length)) > 0)
                {
                    FileChunks.Add(System.Text.Encoding.ASCII.GetString(chunkBuffer));
                }
            }

            var answer = prepareForViewing(CountLettersProcess(FileChunks));

            logger.LogInformation($"Process completed {config.GetFileForProcessing()}. Execution time {stopwatch.Elapsed}");

            return answer;

        }


        /// <summary>
        /// run parallel letter countong process on a chunk list
        /// </summary>
        /// <param name="FileChunks">list of tex</param>
        /// <returns></returns>
        public List<Dictionary<char, int>> CountLettersProcess(List<string> FileChunks)
        {
            List<Dictionary<char, int>> SUM = new List<Dictionary<char, int>>();

            //count letters
            Parallel.ForEach(FileChunks, stringChunk =>
            {
                var dictionary = new Dictionary<char, int>();

                foreach (var symbol in stringChunk)
                {
                    //skip non letter characters
                    if (!char.IsLetter(symbol))
                        continue;

                    var key = char.ToLower(symbol);
                    if (dictionary.ContainsKey(key))
                        dictionary[key]++;
                    else
                        dictionary.Add(key, 1);
                }
                SUM.Add(dictionary);
            });

            return SUM;
        }



        /// <summary>
        /// format the result to view friendly format
        /// </summary>
        /// <param name="processRes">dictioneries of letters and their frequency</param>
        /// <returns></returns>
        public List<WordAnalysis> prepareForViewing(List<Dictionary<char, int>> processRes)
        {
            List<WordAnalysis> answer = new List<WordAnalysis>();

            var ConcatedDicts = processRes
                  .SelectMany(dict => dict)
                  .GroupBy(kvp => kvp.Key)
                  .ToDictionary(g => g.Key, g => g.Sum(kvp => kvp.Value));

            foreach (var pair in ConcatedDicts.OrderBy(p => p.Key))
            {
                answer.Add(new WordAnalysis { Letter = pair.Key, NumberOfOccurrences = pair.Value });
            }

            return answer;
        }


        public ServerState GeteServerState() => new ServerState
        {
            ProcessTime = stopwatch?.Elapsed ?? TimeSpan.Zero,
            State = m_state,
        };
    }
}
