using Ipfs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IpfsSample
{
    class Program
    {
        static void Main(string[] args)
        {

            var service = new IpfsService("https://ipfs.infura.io:5001");
            var node = service.AddFileAsync(@"Hello.txt").Result;
            var hash = node.Hash.ToString();
            Process.Start("https://ipfs.infura.io/ipfs/" + hash);
            Console.ReadLine();

        }

        public class IpfsService
        {

            private string ipfsUrl;
            public IpfsService(string ipfsUrl)
            {
                this.ipfsUrl = ipfsUrl;
            }

            public async Task<MerkleNode> AddFileAsync(string filePath)
            {
                var fileName = Path.GetFileName(filePath);
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    return await AddAsync(fileName, fileStream).ConfigureAwait(false);
                }
            }

            public async Task<MerkleNode> AddAsync(string name, Stream stream)
            {
                using (var ipfs = new IpfsClient(ipfsUrl))
                {
                    var inputStream = new IpfsStream(name, stream);

                    var merkleNode = await ipfs.Add(inputStream).ConfigureAwait(false);
                    var multiHash = ipfs.Pin.Add(merkleNode.Hash.ToString());
                    return merkleNode;
                }
            }

            public async Task<HttpContent> PinListAsync()
            {
                using (var ipfs = new IpfsClient(ipfsUrl))
                {
                    return await ipfs.Pin.Ls();
                }
            }
        }
    }
}
