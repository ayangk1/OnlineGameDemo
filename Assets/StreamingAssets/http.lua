print('this is http')

local http = require("socket.http")
local ltn12 = require("ltn12")
 
-- 要发送的HTTP GET请求的URL
local url = "http://1.14.18.29/UnityHotfix/AB/ABCompareInfo.txt"
-- local url = "http://www.baidu.com"
 
-- 存储响应内容的表
local response_body = {}
 
-- 发送HTTP GET请求
local res, code, response_headers = http.request{
    url = url,
    sink = ltn12.sink.table(response_body)
}
 
-- 检查HTTP响应代码
if code == 200 then
    print("请求成功")
    -- 将响应内容转换为字符串
    local body = table.concat(response_body)
    print(body)
else
    print("请求失败，代码：" .. code)
end
