print('this is ftp')


local ftp = require("socket.ftp")

--此处我没填端口号
file,err = ftp.get("ftp://用户名:密码@patdata1ftp.cnipa.gov.cn/CN-PRSS-30_%D6%D0%B9%FA%CD%E2%B9%DB%C9%E8%BC%C6%D7%A8%C0%FB%B7%A8%C2%C9%D7%B4%CC%AC%B1%EA%D7%BC%BB%AF%CA%FD%BE%DD/20230623/20230623-1-001.ZIP;type=i")

if not file then
	print("ftp出错！" .. err)
else
	local f = io.open("1.zip","wb+")
	f:write(file)
	f:close()
end

