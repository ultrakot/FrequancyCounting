using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataProcessingExam
{
    public class SampleGenerator
    {
        private readonly ILogger<SampleGenerator> logger;

        public SampleGenerator(ILogger<SampleGenerator> logger, IOptions<FileConfiguration> config)
        {
            this.logger = logger;
            var cfg = config.Value;

            var testFile = GenerateSampleFile(cfg.TestFileSize, cfg.TestFileName);
            var fullFile = GenerateSampleFile(cfg.FullFileSize, cfg.FullFileName);

            Task.WaitAll(testFile, fullFile);
        }

        public async Task GenerateSampleFile(int size, string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            int progress = 0;
            if (File.Exists(filePath))
                return;

            logger.LogWarning($"MISSING SAMPLE FILE. Generating sample file {fileName}");

            using (var file = new StreamWriter(filePath))
            {
                var text = new Faker();

                for (int i = 0; i < size; i++)
                { 
                    await file.WriteLineAsync(text.Lorem.Paragraph(3));

                    var tempProgress = (int)(i / (float)size * 100f);
                    if (progress < tempProgress)
                    {
                        progress = tempProgress;
                        logger.LogInformation($"Generating {fileName} [{progress}%]");
                    }
                }

                logger.LogInformation($"{fileName} generation completed");
            }
        }
    }
}
