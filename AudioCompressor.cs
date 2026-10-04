using FFMpegCore;
using FFMpegCore.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeamngAudioCompressor
{

    public class AudioCompressorSettings
    {
        public int OggQuality { get; set; } = 5;
        public string SourceArtPath { get; set; } = string.Empty;
        public string TargetArtPath { get; set; } = string.Empty;
        public int MaxConcurrentEncodes { get; set; } = Environment.ProcessorCount;
    }

    public struct ModifiedFileInfo
    {
        public string OldFileRelativeToDir { get; set; }
        public string NewFileRelativeToDir { get; set; }
    }

    public class ModifiedPathInfo
    {
        public string Directory { get; set; } = string.Empty;
        public List<ModifiedFileInfo> FilesRelativeToDir { get; set; } = new List<ModifiedFileInfo>();
    }

    internal class AudioCompressor
    {
        long oldSize = 0;
        long newSize = 0;
        private static string GetRelativePath(string relativeTo, string path)
        {
            // Ensure the base directory ends with a directory separator so Uri treats it as a container folder
            if (!relativeTo.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !relativeTo.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                relativeTo += Path.DirectorySeparatorChar;
            }

            Uri baseUri = new Uri(Path.GetFullPath(relativeTo));
            Uri targetUri = new Uri(Path.GetFullPath(path));

            if (baseUri.Scheme != targetUri.Scheme)
            {
                // Different drives (e.g. C:\ vs D:\), relative path is impossible
                return path;
            }

            Uri relativeUri = baseUri.MakeRelativeUri(targetUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            // MakeRelativeUri uses forward slashes '/' — normalize to the OS separator
            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
        private static readonly HashSet<string> CompressibleAudioExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".wav", ".mp3", ".flac", ".m4a", ".aac", ".wma", ".aiff", ".ogg"
        };

        private readonly AudioCompressorSettings _settings;
        private readonly ConcurrentDictionary<string, ModifiedPathInfo> _modifiedPaths = new ConcurrentDictionary<string, ModifiedPathInfo>();
        private readonly List<Task> _tasks = new List<Task>();
        private readonly SemaphoreSlim _throttle;

        private float _currentProgress = 0.0f;
        private Action<string, float> _progressCb = null;

        public AudioCompressor(AudioCompressorSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _throttle = new SemaphoreSlim(Math.Max(1, _settings.MaxConcurrentEncodes));
        }

        public IReadOnlyDictionary<string, ModifiedPathInfo> ModifiedPaths => _modifiedPaths;

        public async Task CompressAndFix(Action<string, float> progressCb)
        {
            if (!Directory.Exists(_settings.SourceArtPath))
                throw new DirectoryNotFoundException($"Source art directory not found: {_settings.SourceArtPath}");

            _progressCb = progressCb;

            // Ensure destination root exists
            Directory.CreateDirectory(_settings.TargetArtPath);
            Directory.CreateDirectory(_settings.TargetArtPath + "/sound");
            Directory.CreateDirectory(_settings.TargetArtPath + "/sound/engine");

            var rootDirs = Directory.EnumerateDirectories(_settings.SourceArtPath + "/sound/engine").ToList();
            float dirCount = rootDirs.Count;

            int i = 0;
            foreach (var dir in rootDirs)
            {
                _progressCb?.Invoke($"Loading {dir}", _currentProgress);
                CompressAllFilesUnderDir(Path.GetFileName(dir));
                _currentProgress = i++ / dirCount;
            }

            // Wait for all async FFmpeg tasks to complete
            Task.WaitAll(_tasks.ToArray());
            _currentProgress = 1.0f;
            _progressCb?.Invoke($"Done (compressed {_tasks.Count} audio files)", _currentProgress);
            _progressCb?.Invoke($"{((double)oldSize/ newSize):F2}x reduction", _currentProgress);
            _progressCb?.Invoke($"{((double)oldSize / (1 << 20)):F1} MiB -> {((double)newSize / (1 << 20)):F1} MiB", _currentProgress);
        }

        private void CompressAllFilesUnderDir(string srcDirName)
        {
            string destDirName = $"{srcDirName}";
            string enumerateDir = $"{_settings.SourceArtPath}/sound/engine/{srcDirName}";
            foreach (var file in Directory.EnumerateFiles(enumerateDir, "*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(file);
                string relativePath = GetRelativePath(enumerateDir, file);

                if (CompressibleAudioExtensions.Contains(extension))
                {
                    _progressCb?.Invoke($"Queuing audio task {file}", _currentProgress);
                    CompressAudioFile(destDirName, file, relativePath);
                }
                else
                {
                    _progressCb?.Invoke($"Copying non-audio file {file}", _currentProgress);
                    // Copy non-audio files (or already .ogg files) directly to preserve file integrity
                    CopyNonAudioFile(destDirName, file, relativePath);
                }
            }
        }

        private void CompressAudioFile(string targetDirName, string sourceFilePath, string fileRelativeToDir)
        {
            // Replace old extension with .ogg
            string targetRelPath = Path.ChangeExtension(fileRelativeToDir, ".ogg");
            string outputFilePath = Path.Combine($"{_settings.TargetArtPath}/sound/engine", targetDirName, targetRelPath);

            // Ensure subdirectories exist inside the target
            string targetDirectory = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Record the path modification mapping
            RecordModification(targetDirName, fileRelativeToDir, targetRelPath);

            // Queue FFmpeg task with semaphore throttling
            _tasks.Add(Task.Run(async () =>
            {
                await _throttle.WaitAsync().ConfigureAwait(false);
                try
                {
                    _progressCb?.Invoke($"Compressing audio task {sourceFilePath}", _currentProgress);
                    await ConvertToOggAsync(sourceFilePath, outputFilePath, _settings.OggQuality).ConfigureAwait(false);
                }
                finally
                {
                    _throttle.Release();
                }
            }));
        }

        private void CopyNonAudioFile(string targetDirName, string sourceFilePath, string fileRelativeToDir)
        {
            string outputFilePath = Path.Combine(_settings.TargetArtPath, targetDirName, fileRelativeToDir);
            string targetDirectory = Path.GetDirectoryName(outputFilePath);

            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            File.Copy(sourceFilePath, outputFilePath, overwrite: true);
        }

        private void RecordModification(string directoryName, string oldRelPath, string newRelPath)
        {
            _modifiedPaths.AddOrUpdate(
                directoryName,
                key => new ModifiedPathInfo
                {
                    Directory = key,
                    FilesRelativeToDir = new List<ModifiedFileInfo>
                    {
                        new ModifiedFileInfo { OldFileRelativeToDir = oldRelPath, NewFileRelativeToDir = newRelPath }
                    }
                },
                (key, existing) =>
                {
                    lock (existing.FilesRelativeToDir)
                    {
                        existing.FilesRelativeToDir.Add(new ModifiedFileInfo
                        {
                            OldFileRelativeToDir = oldRelPath,
                            NewFileRelativeToDir = newRelPath
                        });
                    }
                    return existing;
                });
        }

        private async Task ConvertToOggAsync(string inputPath, string outputPath, int quality = 5)
        {
            if (quality < 0 || quality > 10)
                throw new ArgumentOutOfRangeException(nameof(quality), "Vorbis quality must be between 0 and 10.");

            await FFMpegArguments
                .FromFileInput(inputPath)
                .OutputToFile(outputPath, overwrite: true, options => options
                    .WithAudioCodec("libvorbis")
                    .WithCustomArgument($"-qscale:a {quality}")
                    .DisableChannel(Channel.Video))
                .ProcessAsynchronously()
                .ConfigureAwait(false);
            long oldLength = new FileInfo(inputPath).Length;
            long newLength = new FileInfo(outputPath).Length;
            Interlocked.Add(ref oldSize, oldLength);
            Interlocked.Add(ref newSize, newLength);
        }
    }
}