print('this is socket')
local socket = require('socket.core')
local tcp = socket.tcp()
local host = "1.14.18.29"
local port = 6677
local connect = tcp:connect(host,port)
if(connect) then print('连接' .. host .. '成功') 
else print('连接失败') return end

local proto = CS.GameSever.Protocol.LoginProto()
proto.admin = 'admin'
proto.password = '12341'
local network = CS.NetworkManager()
network:SendTcpMsg(proto:ToArray());


local bytes, err = tcp:send("Hello Server")
if not bytes then error("发送失败" .. err) end

-- local response,err = tcpClient:receive()




