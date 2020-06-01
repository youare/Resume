using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Capnp.Rpc;
using CapnpGen;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Resume.Grpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            Person a = new Person();
            TcpRpcServer s = new Capnp.Rpc.TcpRpcServer();
            s.StartAccepting(null, 11);
            TcpRpcClient c = new TcpRpcClient();
            c.Connect("",11);
           
            
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
