echo D|xcopy "../plugins/client.service.ui/client.service.ui.api.service/public/web" "public/web" /s /f /h /y
xcopy "../client.realize/appsettings.json" "public" /f /y
xcopy "../plugins/client.service.ftp/ftp-appsettings.json" "public" /f /y
xcopy "../plugins/client.service.wakeup/wakeup-appsettings.json" "public" /f /y
xcopy "../plugins/client.service.logger/logger-appsettings.json" "public" /f /y
xcopy "../plugins/client.service.tcpforward/p2p-tcp-forwards.json" "public" /f /y
xcopy "../plugins/client.service.tcpforward/server-tcp-forwards.json" "public" /f /y
xcopy "../plugins/client.service.udpforward/p2p-udp-forwards.json" "public" /f /y
xcopy "../plugins/client.service.udpforward/server-udp-forwards.json" "public" /f /y
xcopy "../plugins/client.service.ui/client.service.ui.api/ui-appsettings.json" "public" /f /y
xcopy "../../common/common.socks5/socks5-appsettings.json" "public" /f /y
xcopy "../../common/common.tcpforward/tcpforward-appsettings.json" "public" /f /y
xcopy "../../common/common.udpforward/udpforward-appsettings.json" "public" /f /y