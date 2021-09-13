using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace EditNginx
{
    class Program
    {
        public static async Task EditNginxAsync(string portProd, string port, string confFile)
        {
            // // JMU-DT
            // string[] lines = { "", "#user  nobody;", "worker_processes  1;", "", "#error_log  logs/error.log;", "#error_log  logs/error.log  notice;",
            // "#error_log  logs/error.log  info;", "", "#pid        logs/nginx.pid;", "", "", "events {",  "    worker_connections  1024;", "}", "", "",
            // "http {", "    include       mime.types;", "    default_type  application/octet-stream;", "", "    #log_format  main  '$remote_addr - $remote_user [$time_local] \"$request\" '",
            // "    #                  '$status $body_bytes_sent \"$http_referer\" '", "    #                  '\"$http_user_agent\" \"$http_x_forwarded_for\"';",
            // "", "    #access_log  logs/access.log  main;", "", "    sendfile        on;","    #tcp_nopush     on;", "", "    #keepalive_timeout  0;",
            // "    keepalive_timeout  65;", "", "    #gzip  on;", "", "    upstream backend-server {", "        server localhost:4000;",
            // "    }", "", "    server {", $"        listen       {port};", "        server_name  localhost;", "", "        location / {",
            // "            root   ../../client/build;", "            index  index.html;", "", "            try_files $uri /index.html;",
            // "        }", "", "        location /api/ {", "            proxy_pass http://backend-server;", "        }", "",
            // "        error_page  404              /404.html;", "", "        # redirect server error pages to the static page /50x.html",
            // "        #", "        error_page   500 502 503 504  /50x.html;", "        location = /50x.html {", "            root   html;",
            // "        }", "    }", "", "    server {", "        listen       5005;", "        server_name  localhost;", "",
            // "        location / {", "            root ../../cug_viewer/dist/example-cug-viewer;", "		    index index.html;", "",
            // "            try_files $uri /index.html;", "        }", "    }", "}"};

            // WebViewer
            string[] lines = { "", "#user  nobody;", "worker_processes  1;", "", "#error_log  logs/error.log;", "#error_log  logs/error.log  notice;",
            "#error_log  logs/error.log  info;", "", "#pid        logs/nginx.pid;", "", "", "events {",  "    worker_connections  1024;", "}", "", "",
            "http {", "    include       mime.types;", "    default_type  application/octet-stream;", "", "    #log_format  main  '$remote_addr - $remote_user [$time_local] \"$request\" '",
            "    #                  '$status $body_bytes_sent \"$http_referer\" '", "    #                  '\"$http_user_agent\" \"$http_x_forwarded_for\"';",
            "", "    #access_log  logs/access.log  main;", "", "    sendfile        on;","    #tcp_nopush     on;", "", "    #keepalive_timeout  0;",
            "    keepalive_timeout  65;", "", "    #gzip  on;", "", "    upstream api {", $"		server localhost:{portProd};", "	}", "", "    upstream cee-cloud-server {", "        server localhost:8914;", "    }", "",
            "    server {", $"        listen       {port};", "        server_name  localhost;", "", "        client_max_body_size 5000M;", "", "        #charset koi8-r;", "", "        #access_log  logs/host.access.log  main;", "",
            "        location / {", "			proxy_pass http://api; #whatever port your app runs on", "			proxy_http_version 1.1;", "			proxy_set_header Upgrade $http_upgrade;", "			proxy_set_header Connection 'upgrade';",
            "			proxy_set_header Host $host;", "			proxy_cache_bypass $http_upgrade;", "		}", "", "        location /socket {", "			proxy_pass http://cee-cloud-server; #whatever port your app runs on",
            "			proxy_http_version 1.1;", "			proxy_set_header Upgrade $http_upgrade;", "			proxy_set_header Connection 'upgrade';", "			proxy_set_header Host $host;",
            "			proxy_cache_bypass $http_upgrade;", "		}", "", "        error_page   500 502 503 504  /50x.html;", "        location = /50x.html {", "            root   html;", "        }",
            "    }", "}"};

            await File.WriteAllLinesAsync(confFile, lines);
        }

        static void RunCmd(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = false;
            psi.Arguments = $"/C {command}";
            psi.UseShellExecute = false;

            //Start the process
            Process proc = Process.Start(psi);
        }

        static void RunSh(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "sh";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = false;
            psi.Arguments = $"{command}";
            psi.UseShellExecute = false;

            //Start the process
            Process proc = Process.Start(psi);
        }

        public static OSPlatform GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            throw new Exception("Cannot determine operating system!");
        }

        static async Task Main(string[] args)
        {
            string portProd = "";
            string port = "";
            string nginxDir = "";
            if (args == null || args.Length == 0)
            {
                portProd = "60606";
                port = "56789";
                nginxDir = @"D:\AKIYAMA\Work\EditNginx\nginx";
            }
            else
            {
                portProd = Convert.ToString(args[0]);
                port = Convert.ToString(args[1]);
                nginxDir = Convert.ToString(args[2]);
            }

            await EditNginxAsync(portProd, port, $"{nginxDir}/conf/nginx.conf");

            OSPlatform osType = GetOperatingSystem();
            System.Console.WriteLine(osType);
            if (osType == OSPlatform.Windows)
            {
                RunCmd($"cd {nginxDir} && start_nginx_wv.cmd");
            }
            else
            {
                RunSh($"cd {nginxDir} && ./start_nginx_wv.sh");
            }
        }
    }
}
