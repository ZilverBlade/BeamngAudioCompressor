using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BeamngAudioCompressor
{
    public class JsonRenamerSettings
    {
        public string PrefixBlendName { get; set; } = string.Empty;
        public string SourceVehiclesPath { get; set; } = string.Empty;
        public string TargetVehiclesPath { get; set; } = string.Empty;
        public string SourceArtPath { get; set; } = string.Empty;
        public string TargetArtPath { get; set; } = string.Empty;
    }

    public class JsonRenamer
    {
        JsonRenamerSettings _settings;
        Action<string, float> _progressCb;

        Dictionary<string, List<string>> _mapOldSfx2dToVehicleNamed = new Dictionary<string, List<string>>();

        public JsonRenamer(JsonRenamerSettings settings)
        {
            _settings = settings;
        }


        public void RenameAll(Action<string, float> progressCb)
        {
            _progressCb = progressCb;
            RenameJBeams();
        }

        class VehicleData
        {
            public string Name;
            public string newSoundNameInt;
            public string newSoundNameExh;
            public string oldSoundNameInt;
            public string oldSoundNameExh;
            public string jbeamTxt;
        }
        private void RenameJBeams()
        {
            if (!Directory.Exists(_settings.SourceArtPath))
                throw new DirectoryNotFoundException($"Source art directory not found: {_settings.SourceArtPath}");

            if (!Directory.Exists(_settings.SourceVehiclesPath))
                throw new DirectoryNotFoundException($"Source vehicle directory not found: {_settings.SourceVehiclesPath}");

            if (!Directory.Exists(_settings.TargetVehiclesPath))
                Directory.CreateDirectory($@"{_settings.TargetVehiclesPath}");

            Regex regexSoundConfigInt = new Regex(@"\""(soundConfig|soundConfigIntake)\""\:\s*\{\s*\""sampleName\""\:\s*\""(.+)\""");
            Regex regexSoundConfigExh = new Regex(@"\""(soundConfigExhaust)\""\:\s*\{\s*\""sampleName\""\:\s*\""(.+)\""");

            Regex regexSoundConfigRear = new Regex(@"\""(soundConfigRear)\""\:\s*\{\s*\""sampleName\""\:\s*\""(.+)\""");
            Regex regexSoundConfigFront = new Regex(@"\""(soundConfigFront)\""\:\s*\{\s*\""sampleName\""\:\s*\""(.+)\""");

            DirectoryInfo dir = new DirectoryInfo(_settings.SourceVehiclesPath);

            List<VehicleData> vehicleDatas = new List<VehicleData>();

            var directories = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).ToArray();
            float maxProg = directories.Length;
            int progCounter = 0;
            foreach (var entry in directories)
            {
                string jbeamFile = $@"{entry.FullName}\camso_engine.jbeam";
                float proggress = 0.375f * progCounter++ / maxProg;
                if (entry.Name == "common") continue;
                if (!File.Exists(jbeamFile))
                {
                    _progressCb($"!!! Skipping jbeam rename for {entry.Name}, camso_engine.jbeam not found!", proggress);
                    continue;
                }
                _progressCb($"Renaming jbeam sound config {entry.Name}\\camso_engine.jbeam", proggress);

                string newSoundName = _settings.PrefixBlendName + entry.Name;
                VehicleData data = new VehicleData
                {
                    Name = entry.Name,
                    jbeamTxt = File.ReadAllText(jbeamFile),
                };
                Match matchInt = regexSoundConfigInt.Match(data.jbeamTxt);
                Match matchExh = regexSoundConfigExh.Match(data.jbeamTxt);
                Match matchRear = regexSoundConfigRear.Match(data.jbeamTxt);
                Match matchFront = regexSoundConfigFront.Match(data.jbeamTxt);
                if (!matchInt.Success && !matchExh.Success && !matchFront.Success && !matchRear.Success)
                {
                    throw new InvalidDataException($"JBeam for {entry.Name} was not able to match soundConfigExhaust together with soundConfig/soundConfigIntake, nor soundConfigRear with soundConfigFront!");
                }

                Match firstMatch = matchInt.Success ? matchInt : (matchFront.Success ? matchFront : null);
                Match secondMatch = matchExh.Success ? matchExh : (matchRear.Success ? matchRear : null);
                string oldStringMatchInt = firstMatch?.Value;
                string oldStringMatchExh = secondMatch?.Value;
                data.oldSoundNameInt = firstMatch?.Groups?[2]?.Value;
                data.oldSoundNameExh = secondMatch?.Groups?[2]?.Value;

                bool isEv = matchFront.Success || matchRear.Success;
                data.newSoundNameInt = $"{newSoundName}_{(isEv ? "front" : "int")}";
                data.newSoundNameExh = $"{newSoundName}_{(isEv ? "rear" : "exh")}";

                if (matchFront.Success || matchInt.Success)
                {
                    string newStringMatchInt = oldStringMatchInt?.Replace(data.oldSoundNameInt, data.newSoundNameInt);
                    List<string> targetInts;
                    if (!_mapOldSfx2dToVehicleNamed.TryGetValue(data.oldSoundNameInt, out targetInts))
                    {
                        targetInts = new List<string>();
                        _mapOldSfx2dToVehicleNamed.Add(data.oldSoundNameInt, targetInts);
                    }
                    targetInts.Add(data.newSoundNameInt);
                    data.jbeamTxt = data.jbeamTxt.Replace(oldStringMatchInt, newStringMatchInt);
                }
                if (matchRear.Success || matchExh.Success)
                {
                    string newStringMatchExh = oldStringMatchExh?.Replace(data.oldSoundNameExh, data.newSoundNameExh);
                    List<string> targetExhs;
                    if (!_mapOldSfx2dToVehicleNamed.TryGetValue(data.oldSoundNameExh, out targetExhs))
                    {
                        targetExhs = new List<string>();
                        _mapOldSfx2dToVehicleNamed.Add(data.oldSoundNameExh, targetExhs);
                    }
                    targetExhs.Add(data.newSoundNameExh);
                    data.jbeamTxt = data.jbeamTxt.Replace(oldStringMatchExh, newStringMatchExh);
                }

                vehicleDatas.Add(data);
            }
            float prog = 0.0f;
            foreach (var data in vehicleDatas)
            {
                prog = Math.Max(0.375f * progCounter++ / maxProg, prog);
                _progressCb($@"Writing jbeam sound config {data.Name}\camso_engine.jbeam", prog);
                string targetDir = $@"{_settings.TargetVehiclesPath}\{data.Name}";
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
                File.WriteAllText($@"{_settings.TargetVehiclesPath}\{data.Name}\camso_engine.beam", data.jbeamTxt);
            }

            if (!Directory.Exists(_settings.TargetArtPath))
                Directory.CreateDirectory(_settings.TargetArtPath);
            if (!Directory.Exists($@"{_settings.TargetArtPath}\sound"))
                Directory.CreateDirectory($@"{_settings.TargetArtPath}\sound");
            if (!Directory.Exists($@"{_settings.TargetArtPath}\sound\blends"))
                Directory.CreateDirectory($@"{_settings.TargetArtPath}\sound\blends");

            foreach (var kv in _mapOldSfx2dToVehicleNamed)
            {
                string sourceFile = $@"{_settings.SourceArtPath}\sound\blends\{kv.Key}.sfxBlend2D.json";
                prog += 0.25f * 1.0f / _mapOldSfx2dToVehicleNamed.Values.Count;
                if (!File.Exists(sourceFile))
                {
                    _progressCb($@"!!! Skipping sound config {kv.Key}, not found... Is this a vanilla sound?", prog);
                    continue;
                }
                foreach (var newName in kv.Value)
                {
                    _progressCb($@"Mapping jbeam {kv.Key} -> {newName}", prog);
                    string targetFile = $@"{_settings.TargetArtPath}\sound\blends\{newName}.sfxBlend2D.json";
                    try
                    {
                        File.Copy(sourceFile, targetFile);
                    }
                    catch (Exception)
                    {
                        _progressCb($@"!!! Skipping {newName}, .sfxBlend2D already exists!", prog);
                    }
                }
            }
        }
    }
}
