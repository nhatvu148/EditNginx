tasklist | find /i "nginx_wv.exe" && taskkill /im nginx_wv.exe /F || echo process "nginx_wv.exe" not running.

start nginx_wv.exe