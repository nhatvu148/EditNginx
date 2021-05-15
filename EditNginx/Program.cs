using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EditNginx
{
    class Program
    {
        public static async Task EditNginxAsync(string port, string confFile)
        {
            string[] lines = { "", "#user  nobody;", "worker_processes  1;", "", "#error_log  logs/error.log;", "#error_log  logs/error.log  notice;",
            "#error_log  logs/error.log  info;", "", "#pid        logs/nginx.pid;", "", "", "events {",  "    worker_connections  1024;", "}", "", "",
            "http {", "    include       mime.types;", "    default_type  application/octet-stream;", "", "    #log_format  main  '$remote_addr - $remote_user [$time_local] \"$request\" '",
            "    #                  '$status $body_bytes_sent \"$http_referer\" '", "    #                  '\"$http_user_agent\" \"$http_x_forwarded_for\"';",
            "", "    #access_log  logs/access.log  main;", "", "    sendfile        on;","    #tcp_nopush     on;", "", "    #keepalive_timeout  0;",
            "    keepalive_timeout  65;", "", "    #gzip  on;", "", "    upstream backend-server {", "        server localhost:4000;",
            "    }", "", "    server {", $"        listen       {port};", "        server_name  localhost;", "", "        location / {",
            "            root   ../../client/build;", "            index  index.html;", "", "            try_files $uri /index.html;",
            "        }", "", "        location /api/ {", "            proxy_pass http://backend-server;", "        }", "",
            "        error_page  404              /404.html;", "", "        # redirect server error pages to the static page /50x.html",
            "        #", "        error_page   500 502 503 504  /50x.html;", "        location = /50x.html {", "            root   html;",
            "        }", "    }", "}"};

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

        static async Task Main(string[] args)
        {
            string port = "";
            string nginxDir = "";
            if (args == null || args.Length == 0)
            {
                port = "50521";
                nginxDir = @"C:\Users\nhatv\Work\TechnoStar\jmu-dt\bin\nginx";
            }
            else
            {
                port = Convert.ToString(args[0]);
                nginxDir = Convert.ToString(args[1]);
            }

            await EditNginxAsync(port, $"{nginxDir}/conf/nginx.conf");


            RunCmd($"cd {nginxDir} && start_nginx.cmd");
            //RunCmd("\"C:\\Users\\nhatv\\Downloads\\-NSYNC - Tearin' Up My Heart (Official Music Video).mp3\"");
        }
    }
}
