using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    public class Repository<T>
    {
        public readonly string DirectoryName = Path.Combine("Data", typeof(T).Name);
        private readonly ISerializer<T> _serializer;

        public Repository(ISerializer<T> serializer)
        {
            _serializer = serializer;
            EnsureDirectoryExists(DirectoryName);
        }

        public Task<bool> ContainsAsync(uint256 id)
        {
            var fileName = Path.Combine(DirectoryName, id.ToString());
            return Task.FromResult(File.Exists(fileName));
        }

        public async Task SaveAsync(uint256 id, T item)
        {
            var fileName = Path.Combine(DirectoryName, id.ToString());

            var content = _serializer.Serialize(item);
            await File.WriteAllBytesAsync(fileName, content);
        }

        public async Task<T> GetAsync(uint256 id)
        {
            var fileName = Path.Combine(DirectoryName, id.ToString());
            if (File.Exists(fileName))
            {
                var content = await File.ReadAllBytesAsync(fileName);
                var item = _serializer.Deserialize(content);
                return item;
            }
            return default(T);
        }

        internal IEnumerable<T> Enumerate()
        {
            foreach(var id in GetIds())
            {
                yield return GetAsync(id).GetAwaiter().GetResult();
            }
        }

        internal IEnumerable<uint256> GetIds()
        {
            foreach(var file in Directory.EnumerateFiles(DirectoryName))
            {
                yield return uint256.Parse(Path.GetFileName(file));
            }
        }

        private static void EnsureDirectoryExists(string directory)
        {
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }    
    }
}
